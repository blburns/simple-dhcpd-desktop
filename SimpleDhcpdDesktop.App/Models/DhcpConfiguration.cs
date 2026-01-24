using System.Collections.Generic;

namespace SimpleDhcpdDesktop.App.Models;

public class DhcpConfiguration
{
    public DhcpSection? Dhcp { get; set; }
}

public class DhcpSection
{
    public List<string>? Listen { get; set; }
    public List<SubnetConfiguration>? Subnets { get; set; }
    public List<DhcpOption>? GlobalOptions { get; set; }
    public SecurityConfiguration? Security { get; set; }
    public PerformanceConfiguration? Performance { get; set; }
    public LoggingConfiguration? Logging { get; set; }
}

public class SubnetConfiguration
{
    public string? Name { get; set; }
    public string? Network { get; set; }
    public string? Range { get; set; }
    public string? Gateway { get; set; }
    public List<string>? DnsServers { get; set; }
    public string? DomainName { get; set; }
    public int? LeaseTime { get; set; }
    public int? MaxLeaseTime { get; set; }
    public List<DhcpOption>? Options { get; set; }
    public List<Reservation>? Reservations { get; set; }
    public List<Exclusion>? Exclusions { get; set; }
}

public class DhcpOption
{
    public string? Name { get; set; }
    public string? Value { get; set; }
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
