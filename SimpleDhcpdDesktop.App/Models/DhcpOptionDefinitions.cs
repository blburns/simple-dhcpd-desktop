using System.Collections.Generic;

namespace SimpleDhcpdDesktop.App.Models;

/// <summary>
/// Standard DHCP option definitions with hints for value formats
/// </summary>
public class DhcpOptionDefinition : IEquatable<DhcpOptionDefinition>
{
    public int Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ValueHint { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    
    public bool Equals(DhcpOptionDefinition? other)
    {
        if (other is null) return false;
        return Code == other.Code && Name == other.Name;
    }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as DhcpOptionDefinition);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Name);
    }
}

public static class DhcpOptionDefinitions
{
    public static List<DhcpOptionDefinition> StandardOptions { get; } = new()
    {
        new DhcpOptionDefinition
        {
            Code = 1,
            Name = "SUBNET_MASK",
            Description = "Subnet mask for the client",
            ValueHint = "IP address format",
            Example = "255.255.255.0"
        },
        new DhcpOptionDefinition
        {
            Code = 2,
            Name = "TIME_OFFSET",
            Description = "Time offset from UTC in seconds",
            ValueHint = "Number (seconds from UTC)",
            Example = "0"
        },
        new DhcpOptionDefinition
        {
            Code = 3,
            Name = "ROUTER",
            Description = "Default gateway IP address(es)",
            ValueHint = "Comma-separated IP addresses",
            Example = "192.168.1.1"
        },
        new DhcpOptionDefinition
        {
            Code = 6,
            Name = "DNS_SERVERS",
            Description = "DNS server addresses",
            ValueHint = "Comma-separated IP addresses",
            Example = "8.8.8.8,8.8.4.4,1.1.1.1"
        },
        new DhcpOptionDefinition
        {
            Code = 12,
            Name = "HOSTNAME",
            Description = "Host name for the client",
            ValueHint = "String (hostname)",
            Example = "workstation-01"
        },
        new DhcpOptionDefinition
        {
            Code = 15,
            Name = "DOMAIN_NAME",
            Description = "Domain name for the client",
            ValueHint = "String (domain format)",
            Example = "example.com"
        },
        new DhcpOptionDefinition
        {
            Code = 28,
            Name = "BROADCAST_ADDRESS",
            Description = "Broadcast address for the subnet",
            ValueHint = "IP address format",
            Example = "192.168.1.255"
        },
        new DhcpOptionDefinition
        {
            Code = 42,
            Name = "NTP_SERVERS",
            Description = "Network Time Protocol servers",
            ValueHint = "Comma-separated hostnames or IPs",
            Example = "pool.ntp.org,time.google.com"
        },
        new DhcpOptionDefinition
        {
            Code = 44,
            Name = "NETBIOS_NAME_SERVERS",
            Description = "NetBIOS over TCP/IP name servers",
            ValueHint = "Comma-separated IP addresses",
            Example = "192.168.1.10,192.168.1.11"
        },
        new DhcpOptionDefinition
        {
            Code = 46,
            Name = "NETBIOS_NODE_TYPE",
            Description = "NetBIOS over TCP/IP node type",
            ValueHint = "1=B-node, 2=P-node, 4=M-node, 8=H-node",
            Example = "8"
        },
        new DhcpOptionDefinition
        {
            Code = 51,
            Name = "IP_ADDRESS_LEASE_TIME",
            Description = "Lease time in seconds",
            ValueHint = "Number (seconds)",
            Example = "86400"
        },
        new DhcpOptionDefinition
        {
            Code = 53,
            Name = "DHCP_MESSAGE_TYPE",
            Description = "DHCP message type",
            ValueHint = "1=DISCOVER, 2=OFFER, 3=REQUEST, 5=ACK, 6=NAK",
            Example = "1"
        },
        new DhcpOptionDefinition
        {
            Code = 54,
            Name = "SERVER_IDENTIFIER",
            Description = "DHCP server identifier",
            ValueHint = "IP address",
            Example = "192.168.1.1"
        },
        new DhcpOptionDefinition
        {
            Code = 58,
            Name = "RENEWAL_TIME",
            Description = "Time until client tries to renew (seconds)",
            ValueHint = "Number (seconds)",
            Example = "43200"
        },
        new DhcpOptionDefinition
        {
            Code = 59,
            Name = "REBINDING_TIME",
            Description = "Time until client tries to rebind (seconds)",
            ValueHint = "Number (seconds)",
            Example = "75600"
        },
        new DhcpOptionDefinition
        {
            Code = 66,
            Name = "TFTP_SERVER_NAME",
            Description = "TFTP server name for boot files",
            ValueHint = "String (hostname or IP)",
            Example = "tftp.example.com"
        },
        new DhcpOptionDefinition
        {
            Code = 67,
            Name = "BOOTFILE_NAME",
            Description = "Boot file name for network boot",
            ValueHint = "String (file path)",
            Example = "pxelinux.0"
        },
        new DhcpOptionDefinition
        {
            Code = 69,
            Name = "SMTP_SERVERS",
            Description = "SMTP server addresses",
            ValueHint = "Comma-separated IP addresses",
            Example = "192.168.1.25,192.168.1.26"
        },
        new DhcpOptionDefinition
        {
            Code = 70,
            Name = "POP3_SERVERS",
            Description = "POP3 server addresses",
            ValueHint = "Comma-separated IP addresses",
            Example = "192.168.1.30"
        },
        new DhcpOptionDefinition
        {
            Code = 119,
            Name = "DOMAIN_SEARCH",
            Description = "Domain search list",
            ValueHint = "Comma-separated domain names",
            Example = "example.com,local.example.com"
        },
        new DhcpOptionDefinition
        {
            Code = 121,
            Name = "CLASSLESS_STATIC_ROUTES",
            Description = "Classless static route option",
            ValueHint = "Complex format (see RFC 3442)",
            Example = "192.168.2.0/24,192.168.1.1"
        },
        new DhcpOptionDefinition
        {
            Code = 150,
            Name = "TFTP_SERVER_ADDRESS",
            Description = "TFTP server IP address (Cisco)",
            ValueHint = "IP address",
            Example = "192.168.1.100"
        },
        new DhcpOptionDefinition
        {
            Code = 252,
            Name = "WPAD",
            Description = "Web Proxy Auto-Discovery",
            ValueHint = "URL",
            Example = "http://proxy.example.com/wpad.dat"
        }
    };

    public static DhcpOptionDefinition? GetDefinition(int code)
    {
        return StandardOptions.Find(o => o.Code == code);
    }

    public static DhcpOptionDefinition? GetDefinitionByName(string name)
    {
        return StandardOptions.Find(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
