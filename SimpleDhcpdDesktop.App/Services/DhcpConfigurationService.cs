using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleDhcpdDesktop.App.Models;

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

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<DhcpConfiguration>(json, JsonOptions) 
               ?? new DhcpConfiguration { Dhcp = new DhcpSection() };
    }

    public void SaveConfiguration(DhcpConfiguration config, string filePath)
    {
        var json = JsonSerializer.Serialize(config, JsonOptions);
        
        if (ElevatedFileService.RequiresElevation(filePath))
        {
            throw new UnauthorizedAccessException("This file requires administrator privileges. Please use SaveConfigurationWithElevationAsync instead.");
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, json);
    }

    public async Task<bool> SaveConfigurationWithElevationAsync(DhcpConfiguration config, string filePath, string? password = null)
    {
        var json = JsonSerializer.Serialize(config, JsonOptions);
        return await ElevatedFileService.SaveFileWithElevationAsync(filePath, json, password);
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
