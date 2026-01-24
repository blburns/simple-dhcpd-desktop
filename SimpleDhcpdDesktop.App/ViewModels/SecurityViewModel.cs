using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class SecurityViewModel : ObservableObject
{
    private readonly SecurityConfiguration _security;

    public SecurityViewModel(SecurityConfiguration security)
    {
        _security = security;
        
        if (_security.DhcpSnooping == null)
            _security.DhcpSnooping = new DhcpSnoopingConfiguration();
        if (_security.MacFiltering == null)
            _security.MacFiltering = new MacFilteringConfiguration();
        if (_security.IpFiltering == null)
            _security.IpFiltering = new IpFilteringConfiguration();
        if (_security.RateLimiting == null)
            _security.RateLimiting = new RateLimitingConfiguration();
        if (_security.Option82 == null)
            _security.Option82 = new Option82Configuration();
        if (_security.Authentication == null)
            _security.Authentication = new AuthenticationConfiguration();
        if (_security.SecurityEvents == null)
            _security.SecurityEvents = new SecurityEventsConfiguration();

        MacFilterRules = new ObservableCollection<MacFilterRuleViewModel>(
            (_security.MacFiltering.Rules ?? new List<MacFilterRule>()).Select(r => new MacFilterRuleViewModel(r)));
        IpFilterRules = new ObservableCollection<IpFilterRuleViewModel>(
            (_security.IpFiltering.Rules ?? new List<IpFilterRule>()).Select(r => new IpFilterRuleViewModel(r)));
        RateLimitRules = new ObservableCollection<RateLimitRuleViewModel>(
            (_security.RateLimiting.Rules ?? new List<RateLimitRule>()).Select(r => new RateLimitRuleViewModel(r)));
        Option82Rules = new ObservableCollection<Option82RuleViewModel>(
            (_security.Option82.Rules ?? new List<Option82Rule>()).Select(r => new Option82RuleViewModel(r)));
        TrustedRelayAgents = new ObservableCollection<TrustedRelayAgentViewModel>(
            (_security.Option82.TrustedRelayAgents ?? new List<TrustedRelayAgent>()).Select(a => new TrustedRelayAgentViewModel(a)));
        ClientCredentials = new ObservableCollection<ClientCredentialViewModel>(
            (_security.Authentication.ClientCredentials ?? new List<ClientCredential>()).Select(c => new ClientCredentialViewModel(c)));
        
    }

    public bool? Enable
    {
        get => _security.Enable;
        set
        {
            if (_security.Enable != value)
            {
                _security.Enable = value;
                OnPropertyChanged();
            }
        }
    }

    public SecurityConfiguration Security => _security;

    // DHCP Snooping
    public bool? DhcpSnoopingEnabled
    {
        get => _security.DhcpSnooping?.Enabled;
        set => SetProperty(_security.DhcpSnooping!.Enabled, value, _security.DhcpSnooping, (s, v) => s.Enabled = v);
    }

    public string? TrustedInterfacesText
    {
        get => string.Join(", ", _security.DhcpSnooping?.TrustedInterfaces ?? new List<string>());
        set
        {
            var interfaces = value?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>();
            _security.DhcpSnooping!.TrustedInterfaces = interfaces;
            OnPropertyChanged();
        }
    }

    public bool? DhcpSnoopingLogging
    {
        get => _security.DhcpSnooping?.Logging;
        set => SetProperty(_security.DhcpSnooping!.Logging, value, _security.DhcpSnooping, (s, v) => s.Logging = v);
    }

    public bool? DhcpSnoopingValidation
    {
        get => _security.DhcpSnooping?.Validation;
        set => SetProperty(_security.DhcpSnooping!.Validation, value, _security.DhcpSnooping, (s, v) => s.Validation = v);
    }

    // MAC Filtering
    public bool? MacFilteringEnabled
    {
        get => _security.MacFiltering?.Enabled;
        set => SetProperty(_security.MacFiltering!.Enabled, value, _security.MacFiltering, (m, v) => m.Enabled = v);
    }

    public string? MacFilteringMode
    {
        get => _security.MacFiltering?.Mode;
        set => SetProperty(_security.MacFiltering!.Mode, value, _security.MacFiltering, (m, v) => m.Mode = v);
    }

    [ObservableProperty]
    private ObservableCollection<MacFilterRuleViewModel> macFilterRules;

    // IP Filtering
    public bool? IpFilteringEnabled
    {
        get => _security.IpFiltering?.Enabled;
        set => SetProperty(_security.IpFiltering!.Enabled, value, _security.IpFiltering, (i, v) => i.Enabled = v);
    }

    public string? IpFilteringMode
    {
        get => _security.IpFiltering?.Mode;
        set => SetProperty(_security.IpFiltering!.Mode, value, _security.IpFiltering, (i, v) => i.Mode = v);
    }

    [ObservableProperty]
    private ObservableCollection<IpFilterRuleViewModel> ipFilterRules;

    // Rate Limiting
    public bool? RateLimitingEnabled
    {
        get => _security.RateLimiting?.Enabled;
        set => SetProperty(_security.RateLimiting!.Enabled, value, _security.RateLimiting, (r, v) => r.Enabled = v);
    }

    [ObservableProperty]
    private ObservableCollection<RateLimitRuleViewModel> rateLimitRules;

    // Option 82
    public bool? Option82Enabled
    {
        get => _security.Option82?.Enabled;
        set => SetProperty(_security.Option82!.Enabled, value, _security.Option82, (o, v) => o.Enabled = v);
    }

    public bool? Option82Validation
    {
        get => _security.Option82?.Validation;
        set => SetProperty(_security.Option82!.Validation, value, _security.Option82, (o, v) => o.Validation = v);
    }

    [ObservableProperty]
    private ObservableCollection<Option82RuleViewModel> option82Rules;

    [ObservableProperty]
    private ObservableCollection<TrustedRelayAgentViewModel> trustedRelayAgents;

    // Authentication
    public bool? AuthenticationEnabled
    {
        get => _security.Authentication?.Enabled;
        set => SetProperty(_security.Authentication!.Enabled, value, _security.Authentication, (a, v) => a.Enabled = v);
    }

    public string? AuthenticationKey
    {
        get => _security.Authentication?.Key;
        set => SetProperty(_security.Authentication!.Key, value, _security.Authentication, (a, v) => a.Key = v);
    }

    [ObservableProperty]
    private ObservableCollection<ClientCredentialViewModel> clientCredentials;

    // Security Events
    public bool? SecurityEventsEnableLogging
    {
        get => _security.SecurityEvents?.EnableLogging;
        set => SetProperty(_security.SecurityEvents!.EnableLogging, value, _security.SecurityEvents, (s, v) => s.EnableLogging = v);
    }

    public string? SecurityEventsLogFile
    {
        get => _security.SecurityEvents?.LogFile;
        set => SetProperty(_security.SecurityEvents!.LogFile, value, _security.SecurityEvents, (s, v) => s.LogFile = v);
    }

    public int? SecurityEventsRetentionDays
    {
        get => _security.SecurityEvents?.RetentionDays;
        set => SetProperty(_security.SecurityEvents!.RetentionDays, value, _security.SecurityEvents, (s, v) => s.RetentionDays = v);
    }
}

// Helper ViewModels for nested security objects
public partial class MacFilterRuleViewModel : ObservableObject
{
    private readonly MacFilterRule _rule;

    public MacFilterRuleViewModel(MacFilterRule rule)
    {
        _rule = rule;
    }

    public string? MacAddress
    {
        get => _rule.MacAddress;
        set
        {
            if (_rule.MacAddress != value)
            {
                _rule.MacAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Allow
    {
        get => _rule.Allow;
        set
        {
            if (_rule.Allow != value)
            {
                _rule.Allow = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Description
    {
        get => _rule.Description;
        set
        {
            if (_rule.Description != value)
            {
                _rule.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Enabled
    {
        get => _rule.Enabled;
        set
        {
            if (_rule.Enabled != value)
            {
                _rule.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public MacFilterRule GetRule() => _rule;
}

public partial class IpFilterRuleViewModel : ObservableObject
{
    private readonly IpFilterRule _rule;

    public IpFilterRuleViewModel(IpFilterRule rule)
    {
        _rule = rule;
    }

    public string? IpAddress
    {
        get => _rule.IpAddress;
        set
        {
            if (_rule.IpAddress != value)
            {
                _rule.IpAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Allow
    {
        get => _rule.Allow;
        set
        {
            if (_rule.Allow != value)
            {
                _rule.Allow = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Description
    {
        get => _rule.Description;
        set
        {
            if (_rule.Description != value)
            {
                _rule.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Enabled
    {
        get => _rule.Enabled;
        set
        {
            if (_rule.Enabled != value)
            {
                _rule.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public IpFilterRule GetRule() => _rule;
}

public partial class RateLimitRuleViewModel : ObservableObject
{
    private readonly RateLimitRule _rule;

    public RateLimitRuleViewModel(RateLimitRule rule)
    {
        _rule = rule;
    }

    public string? Identifier
    {
        get => _rule.Identifier;
        set => SetProperty(_rule.Identifier, value, _rule, (r, v) => r.Identifier = v);
    }

    public string? IdentifierType
    {
        get => _rule.IdentifierType;
        set => SetProperty(_rule.IdentifierType, value, _rule, (r, v) => r.IdentifierType = v);
    }

    public int? MaxRequests
    {
        get => _rule.MaxRequests;
        set => SetProperty(_rule.MaxRequests, value, _rule, (r, v) => r.MaxRequests = v);
    }

    public int? TimeWindow
    {
        get => _rule.TimeWindow;
        set => SetProperty(_rule.TimeWindow, value, _rule, (r, v) => r.TimeWindow = v);
    }

    public int? BlockDuration
    {
        get => _rule.BlockDuration;
        set => SetProperty(_rule.BlockDuration, value, _rule, (r, v) => r.BlockDuration = v);
    }

    public bool? Enabled
    {
        get => _rule.Enabled;
        set
        {
            if (_rule.Enabled != value)
            {
                _rule.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public RateLimitRule GetRule() => _rule;
}

public partial class Option82RuleViewModel : ObservableObject
{
    private readonly Option82Rule _rule;

    public Option82RuleViewModel(Option82Rule rule)
    {
        _rule = rule;
    }

    public string? Interface
    {
        get => _rule.Interface;
        set => SetProperty(_rule.Interface, value, _rule, (r, v) => r.Interface = v);
    }

    public bool? Required
    {
        get => _rule.Required;
        set => SetProperty(_rule.Required, value, _rule, (r, v) => r.Required = v);
    }

    public bool? Enabled
    {
        get => _rule.Enabled;
        set
        {
            if (_rule.Enabled != value)
            {
                _rule.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public Option82Rule GetRule() => _rule;
}

public partial class TrustedRelayAgentViewModel : ObservableObject
{
    private readonly TrustedRelayAgent _agent;

    public TrustedRelayAgentViewModel(TrustedRelayAgent agent)
    {
        _agent = agent;
    }

    public string? CircuitId
    {
        get => _agent.CircuitId;
        set => SetProperty(_agent.CircuitId, value, _agent, (a, v) => a.CircuitId = v);
    }

    public string? RemoteId
    {
        get => _agent.RemoteId;
        set => SetProperty(_agent.RemoteId, value, _agent, (a, v) => a.RemoteId = v);
    }

    public bool? Enabled
    {
        get => _agent.Enabled;
        set
        {
            if (_agent.Enabled != value)
            {
                _agent.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public TrustedRelayAgent GetAgent() => _agent;
}

public partial class ClientCredentialViewModel : ObservableObject
{
    private readonly ClientCredential _credential;

    public ClientCredentialViewModel(ClientCredential credential)
    {
        _credential = credential;
    }

    public string? ClientId
    {
        get => _credential.ClientId;
        set => SetProperty(_credential.ClientId, value, _credential, (c, v) => c.ClientId = v);
    }

    public string? PasswordHash
    {
        get => _credential.PasswordHash;
        set => SetProperty(_credential.PasswordHash, value, _credential, (c, v) => c.PasswordHash = v);
    }

    public string? Salt
    {
        get => _credential.Salt;
        set => SetProperty(_credential.Salt, value, _credential, (c, v) => c.Salt = v);
    }

    public bool? Enabled
    {
        get => _credential.Enabled;
        set
        {
            if (_credential.Enabled != value)
            {
                _credential.Enabled = value;
                OnPropertyChanged();
            }
        }
    }

    public ClientCredential GetCredential() => _credential;
}
