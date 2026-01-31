using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleDhcpdDesktop.App.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SimpleDhcpdDesktop.App.Services;

public class DhcpConfigurationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public DhcpConfiguration LoadConfiguration(string filePath)
    {
        if (!File.Exists(filePath))
        {
            System.Diagnostics.Debug.WriteLine($"Configuration file not found: {filePath}");
            return GetDefaultConfiguration();
        }

        try
        {
            var fileContent = File.ReadAllText(filePath);
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            System.Diagnostics.Debug.WriteLine($"Loading configuration from: {filePath}");
            System.Diagnostics.Debug.WriteLine($"File format: {extension}");
            System.Diagnostics.Debug.WriteLine($"File content length: {fileContent.Length} bytes");
            
            DhcpConfiguration? config = null;
            
            // Determine format and parse accordingly
            switch (extension)
            {
                case ".json":
                    config = LoadJsonConfiguration(fileContent);
                    break;
                    
                case ".yaml":
                case ".yml":
                    config = LoadYamlConfiguration(fileContent);
                    break;
                    
                case ".ini":
                case ".conf":
                    config = LoadIniConfiguration(fileContent);
                    break;
                    
                default:
                    // Try to auto-detect format
                    System.Diagnostics.Debug.WriteLine("Unknown extension, attempting auto-detection...");
                    config = TryAutoDetectFormat(fileContent);
                    break;
            }
            
            if (config == null || config.Dhcp == null)
            {
                System.Diagnostics.Debug.WriteLine("Failed to parse configuration, returning default");
                return GetDefaultConfiguration();
            }
            
            // Normalize Listen vs ListenAddresses
            if (config.Dhcp.Listen == null && config.Dhcp.ListenAddresses != null)
            {
                config.Dhcp.Listen = config.Dhcp.ListenAddresses;
            }
            
            System.Diagnostics.Debug.WriteLine($"Configuration loaded successfully.");
            System.Diagnostics.Debug.WriteLine($"  Listen addresses: {config.Dhcp.Listen?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"  Subnets: {config.Dhcp.Subnets?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"  Global options: {config.Dhcp.GlobalOptions?.Count ?? 0}");
            
            return config;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading configuration: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            return GetDefaultConfiguration();
        }
    }
    
    private DhcpConfiguration LoadJsonConfiguration(string content)
    {
        System.Diagnostics.Debug.WriteLine("Parsing as JSON...");
        
        // First try to deserialize as the expected format (with "dhcp" root)
        var config = JsonSerializer.Deserialize<DhcpConfiguration>(content, JsonOptions);
        
        // If config is null or Dhcp section is null, try the legacy format (with "server" root)
        if (config == null || config.Dhcp == null)
        {
            System.Diagnostics.Debug.WriteLine("Trying legacy JSON format with 'server' root...");
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            
            // Check if this is the legacy format with "server" at root
            if (root.TryGetProperty("server", out _) || 
                root.TryGetProperty("listen_addresses", out _) ||
                root.TryGetProperty("subnets", out _))
            {
                config = ConvertLegacyJsonFormat(root);
                System.Diagnostics.Debug.WriteLine("Converted from legacy JSON format");
            }
        }
        
        return config ?? GetDefaultConfiguration();
    }
    
    private DhcpConfiguration LoadYamlConfiguration(string content)
    {
        System.Diagnostics.Debug.WriteLine("Parsing as YAML...");
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        var config = deserializer.Deserialize<DhcpConfiguration>(content);
        
        // If config is null or Dhcp section is null, try legacy format
        if (config == null || config.Dhcp == null)
        {
            System.Diagnostics.Debug.WriteLine("Trying legacy YAML format...");
            var legacyData = deserializer.Deserialize<Dictionary<string, object>>(content);
            if (legacyData != null)
            {
                config = ConvertLegacyYamlFormat(legacyData);
            }
        }
        
        return config ?? GetDefaultConfiguration();
    }
    
    private DhcpConfiguration LoadIniConfiguration(string content)
    {
        System.Diagnostics.Debug.WriteLine("Parsing as INI...");
        
        var config = new DhcpConfiguration
        {
            Dhcp = new DhcpSection
            {
                Listen = new List<string>(),
                Subnets = new List<SubnetConfiguration>(),
                GlobalOptions = new List<DhcpOption>(),
                Security = new SecurityConfiguration(),
                Performance = new PerformanceConfiguration(),
                Logging = new LoggingConfiguration()
            }
        };
        
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        string? currentSection = null;
        SubnetConfiguration? currentSubnet = null;
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            
            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#") || trimmed.StartsWith(";"))
                continue;
            
            // Section header
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                // Save previous subnet if any
                if (currentSubnet != null && currentSection?.StartsWith("subnet:") == true)
                {
                    config.Dhcp.Subnets!.Add(currentSubnet);
                    currentSubnet = null;
                }
                
                currentSection = trimmed.Substring(1, trimmed.Length - 2);
                
                // Start new subnet
                if (currentSection.StartsWith("subnet:"))
                {
                    currentSubnet = new SubnetConfiguration
                    {
                        Name = currentSection.Substring(7),
                        Options = new List<DhcpOption>(),
                        Reservations = new List<Reservation>(),
                        Exclusions = new List<Exclusion>()
                    };
                }
                
                continue;
            }
            
            // Key-value pair
            var parts = trimmed.Split(new[] { '=' }, 2);
            if (parts.Length != 2) continue;
            
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            
            // Parse based on current section
            if (currentSection == "server")
            {
                ParseServerSetting(config.Dhcp, key, value);
            }
            else if (currentSubnet != null)
            {
                ParseSubnetSetting(currentSubnet, key, value);
            }
        }
        
        // Add last subnet if any
        if (currentSubnet != null)
        {
            config.Dhcp.Subnets!.Add(currentSubnet);
        }
        
        return config;
    }
    
    private void ParseServerSetting(DhcpSection dhcp, string key, string value)
    {
        switch (key)
        {
            case "listen_addresses":
                dhcp.Listen = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToList();
                break;
            case "enable_logging":
                if (bool.TryParse(value, out var enableLogging))
                    dhcp.EnableLogging = enableLogging;
                break;
            case "enable_security":
                if (bool.TryParse(value, out var enableSecurity))
                    dhcp.EnableSecurity = enableSecurity;
                break;
            case "max_leases":
                if (int.TryParse(value, out var maxLeases))
                    dhcp.MaxLeases = maxLeases;
                break;
        }
    }
    
    private void ParseSubnetSetting(SubnetConfiguration subnet, string key, string value)
    {
        switch (key)
        {
            case "name":
                subnet.Name = value;
                break;
            case "network":
                subnet.Network = value;
                break;
            case "prefix_length":
                if (int.TryParse(value, out var prefixLength))
                    subnet.PrefixLength = prefixLength;
                break;
            case "range_start":
                subnet.RangeStart = value;
                break;
            case "range_end":
                subnet.RangeEnd = value;
                break;
            case "gateway":
                subnet.Gateway = value;
                break;
            case "dns_servers":
                subnet.DnsServers = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).ToList();
                break;
            case "domain_name":
                subnet.DomainName = value;
                break;
            case "lease_time":
                if (int.TryParse(value, out var leaseTime))
                    subnet.LeaseTime = leaseTime;
                break;
            case "max_lease_time":
                if (int.TryParse(value, out var maxLeaseTime))
                    subnet.MaxLeaseTime = maxLeaseTime;
                break;
        }
    }
    
    private DhcpConfiguration? TryAutoDetectFormat(string content)
    {
        // Try JSON first
        try
        {
            return LoadJsonConfiguration(content);
        }
        catch { }
        
        // Try YAML
        try
        {
            return LoadYamlConfiguration(content);
        }
        catch { }
        
        // Try INI
        try
        {
            return LoadIniConfiguration(content);
        }
        catch { }
        
        return null;
    }
    
    private DhcpConfiguration GetDefaultConfiguration()
    {
        return new DhcpConfiguration
        {
            Dhcp = new DhcpSection
            {
                Listen = new List<string> { "0.0.0.0:67" },
                Subnets = new List<SubnetConfiguration>(),
                GlobalOptions = new List<DhcpOption>(),
                Security = new SecurityConfiguration(),
                Performance = new PerformanceConfiguration(),
                Logging = new LoggingConfiguration()
            }
        };
    }
    
    private DhcpConfiguration ConvertLegacyJsonFormat(JsonElement root)
    {
        var dhcpSection = new DhcpSection
        {
            Subnets = new List<SubnetConfiguration>(),
            GlobalOptions = new List<DhcpOption>(),
            Security = new SecurityConfiguration(),
            Performance = new PerformanceConfiguration(),
            Logging = new LoggingConfiguration()
        };
        
        // Extract server settings
        if (root.TryGetProperty("server", out var serverElement))
        {
            if (serverElement.TryGetProperty("listen_addresses", out var listenElement))
            {
                dhcpSection.Listen = JsonSerializer.Deserialize<List<string>>(listenElement.GetRawText());
            }
        }
        // Handle flat structure (no "server" wrapper)
        else if (root.TryGetProperty("listen_addresses", out var listenElement))
        {
            dhcpSection.Listen = JsonSerializer.Deserialize<List<string>>(listenElement.GetRawText());
        }
        
        // Extract subnets
        if (root.TryGetProperty("subnets", out var subnetsElement))
        {
            dhcpSection.Subnets = JsonSerializer.Deserialize<List<SubnetConfiguration>>(subnetsElement.GetRawText(), JsonOptions);
        }
        
        // Extract global options
        if (root.TryGetProperty("global_options", out var optionsElement))
        {
            dhcpSection.GlobalOptions = JsonSerializer.Deserialize<List<DhcpOption>>(optionsElement.GetRawText(), JsonOptions);
        }
        
        // Extract security settings
        if (root.TryGetProperty("security", out var securityElement))
        {
            dhcpSection.Security = JsonSerializer.Deserialize<SecurityConfiguration>(securityElement.GetRawText(), JsonOptions);
        }
        
        // Extract logging settings
        if (root.TryGetProperty("logging", out var loggingElement))
        {
            dhcpSection.Logging = JsonSerializer.Deserialize<LoggingConfiguration>(loggingElement.GetRawText(), JsonOptions);
        }
        
        return new DhcpConfiguration { Dhcp = dhcpSection };
    }
    
    private DhcpConfiguration ConvertLegacyYamlFormat(Dictionary<string, object> data)
    {
        var dhcpSection = new DhcpSection
        {
            Subnets = new List<SubnetConfiguration>(),
            GlobalOptions = new List<DhcpOption>(),
            Security = new SecurityConfiguration(),
            Performance = new PerformanceConfiguration(),
            Logging = new LoggingConfiguration()
        };
        
        // Try to extract server settings
        if (data.ContainsKey("server") && data["server"] is Dictionary<string, object> server)
        {
            if (server.ContainsKey("listen_addresses") && server["listen_addresses"] is List<object> listenList)
            {
                dhcpSection.Listen = listenList.Select(o => o?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
        }
        else if (data.ContainsKey("listen_addresses") && data["listen_addresses"] is List<object> listenList)
        {
            dhcpSection.Listen = listenList.Select(o => o?.ToString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
        
        // Re-serialize and deserialize to get proper types
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(data);
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        
        // Try to deserialize subnets and other sections
        if (data.ContainsKey("subnets"))
        {
            try
            {
                var subnetsYaml = serializer.Serialize(data["subnets"]);
                dhcpSection.Subnets = deserializer.Deserialize<List<SubnetConfiguration>>(subnetsYaml);
            }
            catch { }
        }
        
        if (data.ContainsKey("global_options"))
        {
            try
            {
                var optionsYaml = serializer.Serialize(data["global_options"]);
                dhcpSection.GlobalOptions = deserializer.Deserialize<List<DhcpOption>>(optionsYaml);
            }
            catch { }
        }
        
        return new DhcpConfiguration { Dhcp = dhcpSection };
    }

    public void SaveConfiguration(DhcpConfiguration config, string filePath)
    {
        if (ElevatedFileService.RequiresElevation(filePath))
        {
            throw new UnauthorizedAccessException("This file requires administrator privileges. Please use SaveConfigurationWithElevationAsync instead.");
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var content = SerializeConfiguration(config, filePath);
        File.WriteAllText(filePath, content);
    }
    
    private string SerializeConfiguration(DhcpConfiguration config, string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".json" => JsonSerializer.Serialize(config, JsonOptions),
            ".yaml" or ".yml" => SerializeToYaml(config),
            ".ini" or ".conf" => SerializeToIni(config),
            _ => JsonSerializer.Serialize(config, JsonOptions) // Default to JSON
        };
    }
    
    private string SerializeToYaml(DhcpConfiguration config)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return serializer.Serialize(config);
    }
    
    private string SerializeToIni(DhcpConfiguration config)
    {
        var lines = new List<string>
        {
            "# Simple DHCP Daemon Configuration",
            "# Generated by SimpleDhcpdDesktop",
            "",
            "[server]"
        };
        
        if (config.Dhcp?.Listen != null && config.Dhcp.Listen.Any())
        {
            lines.Add($"listen_addresses = {string.Join(",", config.Dhcp.Listen)}");
        }
        
        if (config.Dhcp?.EnableLogging != null)
        {
            lines.Add($"enable_logging = {config.Dhcp.EnableLogging.ToString()!.ToLower()}");
        }
        
        if (config.Dhcp?.EnableSecurity != null)
        {
            lines.Add($"enable_security = {config.Dhcp.EnableSecurity.ToString()!.ToLower()}");
        }
        
        if (config.Dhcp?.MaxLeases != null)
        {
            lines.Add($"max_leases = {config.Dhcp.MaxLeases}");
        }
        
        // Add subnets
        if (config.Dhcp?.Subnets != null)
        {
            foreach (var subnet in config.Dhcp.Subnets)
            {
                lines.Add("");
                lines.Add($"[subnet:{subnet.Name}]");
                lines.Add($"name = {subnet.Name}");
                
                if (!string.IsNullOrEmpty(subnet.Network))
                    lines.Add($"network = {subnet.Network}");
                    
                if (subnet.PrefixLength != null)
                    lines.Add($"prefix_length = {subnet.PrefixLength}");
                    
                if (!string.IsNullOrEmpty(subnet.RangeStart))
                    lines.Add($"range_start = {subnet.RangeStart}");
                    
                if (!string.IsNullOrEmpty(subnet.RangeEnd))
                    lines.Add($"range_end = {subnet.RangeEnd}");
                    
                if (!string.IsNullOrEmpty(subnet.Gateway))
                    lines.Add($"gateway = {subnet.Gateway}");
                    
                if (subnet.DnsServers != null && subnet.DnsServers.Any())
                    lines.Add($"dns_servers = {string.Join(",", subnet.DnsServers)}");
                    
                if (!string.IsNullOrEmpty(subnet.DomainName))
                    lines.Add($"domain_name = {subnet.DomainName}");
                    
                if (subnet.LeaseTime != null)
                    lines.Add($"lease_time = {subnet.LeaseTime}");
                    
                if (subnet.MaxLeaseTime != null)
                    lines.Add($"max_lease_time = {subnet.MaxLeaseTime}");
            }
        }
        
        return string.Join(Environment.NewLine, lines);
    }

    public async Task<bool> SaveConfigurationWithElevationAsync(DhcpConfiguration config, string filePath, string? password = null)
    {
        var content = SerializeConfiguration(config, filePath);
        return await ElevatedFileService.SaveFileWithElevationAsync(filePath, content, password);
    }

    public async Task<DhcpConfiguration?> LoadConfigurationWithElevationAsync(string filePath, string? password = null)
    {
        var json = await ElevatedFileService.LoadFileWithElevationAsync(filePath, password);
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<DhcpConfiguration>(json, JsonOptions);
    }

    public string GetDefaultConfigPath()
    {
        var platform = Environment.OSVersion.Platform;
        
        return platform switch
        {
            PlatformID.Win32NT => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Simple DHCP Daemon",
                "simple-dhcpd.conf"),
            PlatformID.Unix when Directory.Exists("/etc") => "/etc/simple-dhcpd/simple-dhcpd.conf",
            PlatformID.MacOSX => "/usr/local/etc/simple-dhcpd/simple-dhcpd.conf",
            _ => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "simple-dhcpd",
                "simple-dhcpd.conf")
        };
    }
}
