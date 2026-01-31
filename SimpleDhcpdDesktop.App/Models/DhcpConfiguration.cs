using System.Collections.Generic;

namespace SimpleDhcpdDesktop.App.Models;

/// <summary>
/// Root DHCP Configuration Model
/// 
/// This is the top-level configuration object that represents the entire DHCP server
/// configuration. It contains a single DhcpSection that holds all configuration data.
/// 
/// This model is serialized to/from JSON, YAML, and INI formats by the configuration
/// parsers (DhcpConfigurationService, YamlConfigurationParser, IniConfigurationParser).
/// </summary>
public class DhcpConfiguration
{
    /// <summary>
    /// The main DHCP configuration section containing all server settings.
    /// </summary>
    public DhcpSection? Dhcp { get; set; }
}

/// <summary>
/// DHCP Section Model - Main configuration container
/// 
/// Contains all DHCP server configuration including:
/// - Server settings (listen addresses, ports, file paths)
/// - Subnet configurations
/// - Global DHCP options
/// - Security settings
/// - Performance settings
/// - Logging configuration
/// - Monitoring configuration
/// 
/// This is the primary configuration object that gets serialized to configuration files.
/// </summary>
public class DhcpSection
{
    /// <summary>
    /// List of network addresses and ports the server listens on.
    /// Format: "IP:Port" (e.g., "0.0.0.0:67", "192.168.1.1:67").
    /// Supports both "Listen" and "ListenAddresses" property names for backward compatibility.
    /// </summary>
    public List<string>? Listen { get; set; }
    
    /// <summary>
    /// Alternative property name for Listen addresses (backward compatibility).
    /// If both are set, Listen takes precedence.
    /// </summary>
    public List<string>? ListenAddresses { get; set; }
    
    /// <summary>
    /// Default DHCP port number (typically 67 for server).
    /// If null, uses the port specified in individual listen addresses.
    /// </summary>
    public int? Port { get; set; }
    
    /// <summary>
    /// List of subnet configurations. Each subnet defines a network segment
    /// with its own IP range, gateway, DNS servers, and DHCP options.
    /// </summary>
    public List<SubnetConfiguration>? Subnets { get; set; }
    
    /// <summary>
    /// Global DHCP options that apply to all clients regardless of subnet.
    /// Examples: domain-name-servers, router, time-servers.
    /// </summary>
    public List<DhcpOption>? GlobalOptions { get; set; }
    
    /// <summary>
    /// Security configuration including MAC filtering, IP filtering, rate limiting,
    /// Option 82, authentication, and DHCP snooping.
    /// </summary>
    public SecurityConfiguration? Security { get; set; }
    
    /// <summary>
    /// Performance configuration including lease database settings, connection pooling,
    /// and caching options.
    /// </summary>
    public PerformanceConfiguration? Performance { get; set; }
    
    /// <summary>
    /// Logging configuration including log levels, file paths, formats, and rotation settings.
    /// </summary>
    public LoggingConfiguration? Logging { get; set; }
    
    /// <summary>
    /// Monitoring configuration including metrics collection and health checks.
    /// </summary>
    public MonitoringConfiguration? Monitoring { get; set; }
    
    /// <summary>
    /// Optional override for the default configuration file path.
    /// If null, uses the default platform-specific path.
    /// </summary>
    public string? ConfigFile { get; set; }
    
    /// <summary>
    /// Path to the DHCP lease database file.
    /// If null, uses the default platform-specific lease file location.
    /// </summary>
    public string? LeaseFile { get; set; }
    
    /// <summary>
    /// Enable or disable logging functionality globally.
    /// When true, the server logs DHCP events. Logging configuration is in Logging section.
    /// </summary>
    public bool? EnableLogging { get; set; }
    
    /// <summary>
    /// Enable or disable security features globally.
    /// When true, security features are enabled. Security configuration is in Security section.
    /// </summary>
    public bool? EnableSecurity { get; set; }
    
    /// <summary>
    /// Maximum number of concurrent IP address leases across all subnets.
    /// If null, no limit is enforced.
    /// </summary>
    public int? MaxLeases { get; set; }
}

public class SubnetConfiguration
{
    public string? Name { get; set; }
    public string? Network { get; set; }
    public string? Range { get; set; }
    
    // Alternative properties for range (used in JSON/YAML/INI formats)
    public string? RangeStart { get; set; }
    public string? RangeEnd { get; set; }
    public int? PrefixLength { get; set; }
    
    public string? Gateway { get; set; }
    public List<string>? DnsServers { get; set; }
    public string? DomainName { get; set; }
    public int? LeaseTime { get; set; }
    public int? MaxLeaseTime { get; set; }
    public List<DhcpOption>? Options { get; set; }
    public List<Reservation>? Reservations { get; set; }
    public List<Exclusion>? Exclusions { get; set; }
    
    // Helper to get range in unified format
    public string GetRangeString()
    {
        if (!string.IsNullOrEmpty(Range))
            return Range;
        if (!string.IsNullOrEmpty(RangeStart) && !string.IsNullOrEmpty(RangeEnd))
            return $"{RangeStart}-{RangeEnd}";
        return string.Empty;
    }
}

public class DhcpOption
{
    /// <summary>
    /// Option code (e.g., 6 for DNS, 3 for Router)
    /// </summary>
    public int? Code { get; set; }
    
    /// <summary>
    /// Option name (e.g., "domain-name-servers", "routers")
    /// This is the primary identifier used by simple-dhcpd
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Option value/data (e.g., "8.8.8.8,8.8.4.4")
    /// In simple-dhcpd, this can be called "value", "data", or derived from other fields
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Alternative property name for value (used in some simple-dhcpd configs)
    /// </summary>
    public string? Data { get; set; }
    
    /// <summary>
    /// Get the effective value (prefers Value, falls back to Data)
    /// </summary>
    public string? GetEffectiveValue() => Value ?? Data;
    
    /// <summary>
    /// Set the effective value (sets both Value and Data for compatibility)
    /// </summary>
    public void SetEffectiveValue(string? value)
    {
        Value = value;
        Data = value;
    }
}

public class Reservation
{
    public string? MacAddress { get; set; }
    public string? IpAddress { get; set; }
    public string? Hostname { get; set; }
    public string? Description { get; set; }
}

public class Exclusion
{
    public string? Start { get; set; }
    public string? End { get; set; }
}

public class SecurityConfiguration
{
    public bool? Enable { get; set; }
    public DhcpSnoopingConfiguration? DhcpSnooping { get; set; }
    public MacFilteringConfiguration? MacFiltering { get; set; }
    public IpFilteringConfiguration? IpFiltering { get; set; }
    public RateLimitingConfiguration? RateLimiting { get; set; }
    public Option82Configuration? Option82 { get; set; }
    public AuthenticationConfiguration? Authentication { get; set; }
    public SecurityEventsConfiguration? SecurityEvents { get; set; }
}

public class DhcpSnoopingConfiguration
{
    public bool? Enabled { get; set; }
    public List<string>? TrustedInterfaces { get; set; }
    public bool? Logging { get; set; }
    public bool? Validation { get; set; }
}

public class MacFilteringConfiguration
{
    public bool? Enabled { get; set; }
    public string? Mode { get; set; }
    public List<MacFilterRule>? Rules { get; set; }
}

public class MacFilterRule
{
    public string? MacAddress { get; set; }
    public bool? Allow { get; set; }
    public string? Description { get; set; }
    public bool? Enabled { get; set; }
}

public class IpFilteringConfiguration
{
    public bool? Enabled { get; set; }
    public string? Mode { get; set; }
    public List<IpFilterRule>? Rules { get; set; }
}

public class IpFilterRule
{
    public string? IpAddress { get; set; }
    public bool? Allow { get; set; }
    public string? Description { get; set; }
    public bool? Enabled { get; set; }
}

public class RateLimitingConfiguration
{
    public bool? Enabled { get; set; }
    public List<RateLimitRule>? Rules { get; set; }
}

public class RateLimitRule
{
    public string? Identifier { get; set; }
    public string? IdentifierType { get; set; }
    public int? MaxRequests { get; set; }
    public int? TimeWindow { get; set; }
    public int? BlockDuration { get; set; }
    public bool? Enabled { get; set; }
}

public class Option82Configuration
{
    public bool? Enabled { get; set; }
    public bool? Validation { get; set; }
    public List<Option82Rule>? Rules { get; set; }
    public List<TrustedRelayAgent>? TrustedRelayAgents { get; set; }
}

public class Option82Rule
{
    public string? Interface { get; set; }
    public bool? Required { get; set; }
    public bool? Enabled { get; set; }
}

public class TrustedRelayAgent
{
    public string? CircuitId { get; set; }
    public string? RemoteId { get; set; }
    public bool? Enabled { get; set; }
}

public class AuthenticationConfiguration
{
    public bool? Enabled { get; set; }
    public string? Key { get; set; }
    public List<ClientCredential>? ClientCredentials { get; set; }
}

public class ClientCredential
{
    public string? ClientId { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    public bool? Enabled { get; set; }
}

public class SecurityEventsConfiguration
{
    public bool? EnableLogging { get; set; }
    public string? LogFile { get; set; }
    public string? EventCallback { get; set; }
    public int? RetentionDays { get; set; }
}

public class PerformanceConfiguration
{
    public int? MaxLeases { get; set; }
    public LeaseDatabaseConfiguration? LeaseDatabase { get; set; }
}

public class LeaseDatabaseConfiguration
{
    public string? Type { get; set; }
    public string? Path { get; set; }
    public bool? Backup { get; set; }
    public int? BackupInterval { get; set; }
    public int? BackupRetention { get; set; }
}

public class LoggingConfiguration
{
    public bool? Enable { get; set; }
    public string? Level { get; set; }
    public string? LogFile { get; set; }
    public string? Format { get; set; }
    public bool? Rotation { get; set; }
    public string? MaxSize { get; set; }
    public int? MaxFiles { get; set; }
}

public class MonitoringConfiguration
{
    public MetricsConfiguration? Metrics { get; set; }
    public HealthChecksConfiguration? HealthChecks { get; set; }
}

public class MetricsConfiguration
{
    public bool? Enabled { get; set; }
    public string? Endpoint { get; set; }
    public string? Format { get; set; }
    public int? Interval { get; set; }
    public bool? CustomMetrics { get; set; }
    public bool? BusinessMetrics { get; set; }
}

public class HealthChecksConfiguration
{
    public bool? Enabled { get; set; }
    public string? Endpoint { get; set; }
    public int? Interval { get; set; }
    public int? Timeout { get; set; }
    public List<HealthCheck>? Checks { get; set; }
}

public class HealthCheck
{
    public string? Name { get; set; }
    public int? Threshold { get; set; }
    public string? Action { get; set; }
}
