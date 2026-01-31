using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.Services;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public string GetSummary()
    {
        if (IsValid && Warnings.Count == 0)
            return "✓ Configuration is valid";
        
        var summary = new List<string>();
        
        if (Errors.Count > 0)
            summary.Add($"✗ {Errors.Count} error(s)");
        
        if (Warnings.Count > 0)
            summary.Add($"⚠ {Warnings.Count} warning(s)");
        
        return string.Join(", ", summary);
    }
}

public class ConfigurationValidator
{
    public ValidationResult Validate(DhcpConfiguration config)
    {
        var result = new ValidationResult { IsValid = true };

        if (config.Dhcp == null)
        {
            result.Errors.Add("Configuration is missing DHCP section");
            result.IsValid = false;
            return result;
        }

        ValidateListenAddresses(config.Dhcp, result);
        ValidateSubnets(config.Dhcp, result);
        ValidateGlobalOptions(config.Dhcp, result);
        ValidateSecurity(config.Dhcp, result);
        ValidatePerformance(config.Dhcp, result);

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    private void ValidateListenAddresses(DhcpSection dhcp, ValidationResult result)
    {
        if (dhcp.Listen == null || dhcp.Listen.Count == 0)
        {
            result.Errors.Add("No listen addresses configured");
            return;
        }

        foreach (var address in dhcp.Listen)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                result.Errors.Add("Empty listen address found");
                continue;
            }

            // Validate format: IP:Port
            var parts = address.Split(':');
            if (parts.Length != 2)
            {
                result.Errors.Add($"Invalid listen address format: {address} (expected IP:Port)");
                continue;
            }

            if (!IPAddress.TryParse(parts[0], out _))
            {
                result.Errors.Add($"Invalid IP address in listen address: {parts[0]}");
            }

            if (!int.TryParse(parts[1], out var port) || port < 1 || port > 65535)
            {
                result.Errors.Add($"Invalid port in listen address: {parts[1]} (must be 1-65535)");
            }
        }
    }

    private void ValidateSubnets(DhcpSection dhcp, ValidationResult result)
    {
        if (dhcp.Subnets == null || dhcp.Subnets.Count == 0)
        {
            result.Warnings.Add("No subnets configured");
            return;
        }

        var subnetNames = new HashSet<string>();

        foreach (var subnet in dhcp.Subnets)
        {
            if (string.IsNullOrWhiteSpace(subnet.Name))
            {
                result.Errors.Add("Subnet with empty name found");
            }
            else if (subnetNames.Contains(subnet.Name))
            {
                result.Errors.Add($"Duplicate subnet name: {subnet.Name}");
            }
            else
            {
                subnetNames.Add(subnet.Name);
            }

            // Validate network address
            if (string.IsNullOrWhiteSpace(subnet.Network))
            {
                result.Errors.Add($"Subnet '{subnet.Name}' has no network address");
            }
            else if (!IPAddress.TryParse(subnet.Network, out _))
            {
                result.Errors.Add($"Subnet '{subnet.Name}' has invalid network address: {subnet.Network}");
            }

            // Validate range
            if (string.IsNullOrWhiteSpace(subnet.Range) && 
                (string.IsNullOrWhiteSpace(subnet.RangeStart) || string.IsNullOrWhiteSpace(subnet.RangeEnd)))
            {
                result.Warnings.Add($"Subnet '{subnet.Name}' has no IP range configured");
            }
            else
            {
                ValidateIpRange(subnet, result);
            }

            // Validate gateway
            if (string.IsNullOrWhiteSpace(subnet.Gateway))
            {
                result.Warnings.Add($"Subnet '{subnet.Name}' has no gateway configured");
            }
            else if (!IPAddress.TryParse(subnet.Gateway, out _))
            {
                result.Errors.Add($"Subnet '{subnet.Name}' has invalid gateway: {subnet.Gateway}");
            }

            // Validate DNS servers
            if (subnet.DnsServers != null)
            {
                foreach (var dns in subnet.DnsServers)
                {
                    if (!IPAddress.TryParse(dns, out _))
                    {
                        result.Errors.Add($"Subnet '{subnet.Name}' has invalid DNS server: {dns}");
                    }
                }
            }

            // Validate lease times
            if (subnet.LeaseTime.HasValue && subnet.LeaseTime.Value < 60)
            {
                result.Warnings.Add($"Subnet '{subnet.Name}' has very short lease time: {subnet.LeaseTime}s");
            }

            if (subnet.MaxLeaseTime.HasValue && subnet.LeaseTime.HasValue && 
                subnet.MaxLeaseTime.Value < subnet.LeaseTime.Value)
            {
                result.Errors.Add($"Subnet '{subnet.Name}' max lease time is less than lease time");
            }
        }
    }

    private void ValidateIpRange(SubnetConfiguration subnet, ValidationResult result)
    {
        string? rangeStart = subnet.RangeStart;
        string? rangeEnd = subnet.RangeEnd;

        // Parse Range if RangeStart/RangeEnd not set
        if (string.IsNullOrWhiteSpace(rangeStart) && !string.IsNullOrWhiteSpace(subnet.Range))
        {
            var parts = subnet.Range.Split('-');
            if (parts.Length == 2)
            {
                rangeStart = parts[0].Trim();
                rangeEnd = parts[1].Trim();
            }
        }

        if (!string.IsNullOrWhiteSpace(rangeStart) && !IPAddress.TryParse(rangeStart, out _))
        {
            result.Errors.Add($"Subnet '{subnet.Name}' has invalid range start: {rangeStart}");
        }

        if (!string.IsNullOrWhiteSpace(rangeEnd) && !IPAddress.TryParse(rangeEnd, out _))
        {
            result.Errors.Add($"Subnet '{subnet.Name}' has invalid range end: {rangeEnd}");
        }
    }

    private void ValidateGlobalOptions(DhcpSection dhcp, ValidationResult result)
    {
        if (dhcp.GlobalOptions == null) return;

        foreach (var option in dhcp.GlobalOptions)
        {
            if (string.IsNullOrWhiteSpace(option.Name))
            {
                result.Warnings.Add("Global option with empty name found");
            }

            if (string.IsNullOrWhiteSpace(option.Value))
            {
                result.Warnings.Add($"Global option '{option.Name}' has empty value");
            }
        }
    }

    private void ValidateSecurity(DhcpSection dhcp, ValidationResult result)
    {
        if (dhcp.Security == null) return;

        // Add security-specific validations here if needed
        if (dhcp.Security.Enable == true)
        {
            if (dhcp.Security.MacFiltering?.Enabled == true && 
                (dhcp.Security.MacFiltering.Rules == null || dhcp.Security.MacFiltering.Rules.Count == 0))
            {
                result.Warnings.Add("MAC filtering is enabled but no rules are configured");
            }

            if (dhcp.Security.IpFiltering?.Enabled == true && 
                (dhcp.Security.IpFiltering.Rules == null || dhcp.Security.IpFiltering.Rules.Count == 0))
            {
                result.Warnings.Add("IP filtering is enabled but no rules are configured");
            }
        }
    }

    private void ValidatePerformance(DhcpSection dhcp, ValidationResult result)
    {
        if (dhcp.Performance == null) return;

        if (dhcp.MaxLeases.HasValue && dhcp.MaxLeases.Value < 10)
        {
            result.Warnings.Add($"Max leases is very low: {dhcp.MaxLeases}");
        }

        // Add more performance validations as needed
    }
}
