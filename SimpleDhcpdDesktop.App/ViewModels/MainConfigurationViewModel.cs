using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;
using SimpleDhcpdDesktop.App.Services;
using SimpleDhcpdDesktop.App.Views;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class MainConfigurationViewModel : ObservableObject
{
    private readonly DhcpConfigurationService _configService;

    public MainConfigurationViewModel(DhcpConfigurationService configService)
    {
        _configService = configService;
        ConfigFilePath = configService.GetDefaultConfigPath();
        
        var config = _configService.LoadConfiguration(ConfigFilePath);
        Configuration = config;
        
        InitializeSubViewModels();
    }

    [ObservableProperty]
    private DhcpConfiguration configuration = new();

    [ObservableProperty]
    private string configFilePath = string.Empty;

    [ObservableProperty]
    private ServerSettingsViewModel? serverSettings;

    [ObservableProperty]
    private SubnetsViewModel? subnets;

    [ObservableProperty]
    private GlobalOptionsViewModel? globalOptions;

    [ObservableProperty]
    private SecurityViewModel? security;

    [ObservableProperty]
    private PerformanceViewModel? performance;

    [ObservableProperty]
    private LoggingViewModel? logging;

    [ObservableProperty]
    private bool hasUnsavedChanges;

    private void InitializeSubViewModels()
    {
        ServerSettings = new ServerSettingsViewModel(Configuration.Dhcp ?? new DhcpSection());
        Subnets = new SubnetsViewModel(Configuration.Dhcp?.Subnets ?? new List<SubnetConfiguration>());
        GlobalOptions = new GlobalOptionsViewModel(Configuration.Dhcp?.GlobalOptions ?? new List<DhcpOption>());
        Security = new SecurityViewModel(Configuration.Dhcp?.Security ?? new SecurityConfiguration());
        Performance = new PerformanceViewModel(Configuration.Dhcp?.Performance ?? new PerformanceConfiguration());
        Logging = new LoggingViewModel(Configuration.Dhcp?.Logging ?? new LoggingConfiguration());

        // Subscribe to changes
        ServerSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Subnets.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        GlobalOptions.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Security.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Performance.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Logging.PropertyChanged += (s, e) => HasUnsavedChanges = true;
    }

    [RelayCommand]
    private void LoadConfiguration()
    {
        var config = _configService.LoadConfiguration(ConfigFilePath);
        Configuration = config;
        InitializeSubViewModels();
        HasUnsavedChanges = false;
    }

    [RelayCommand]
    private async Task SaveConfigurationAsync()
    {
        // Update configuration from view models
        if (Configuration.Dhcp == null)
        {
            Configuration.Dhcp = new DhcpSection();
        }

        Configuration.Dhcp.Listen = ServerSettings?.ListenAddresses?.ToList();
        Configuration.Dhcp.Subnets = Subnets?.Subnets?.Select(s => s.GetSubnetConfiguration()).ToList();
        Configuration.Dhcp.GlobalOptions = GlobalOptions?.Options?.Select(o => o.GetOption()).ToList();
        
        // Update security collections
        if (Security != null)
        {
            Security.Security.MacFiltering!.Rules = Security.MacFilterRules.Select(r => r.GetRule()).ToList();
            Security.Security.IpFiltering!.Rules = Security.IpFilterRules.Select(r => r.GetRule()).ToList();
            Security.Security.RateLimiting!.Rules = Security.RateLimitRules.Select(r => r.GetRule()).ToList();
            Security.Security.Option82!.Rules = Security.Option82Rules.Select(r => r.GetRule()).ToList();
            Security.Security.Option82!.TrustedRelayAgents = Security.TrustedRelayAgents.Select(a => a.GetAgent()).ToList();
            Security.Security.Authentication!.ClientCredentials = Security.ClientCredentials.Select(c => c.GetCredential()).ToList();
        }
        
        Configuration.Dhcp.Security = Security?.Security;
        Configuration.Dhcp.Performance = Performance?.Performance;
        Configuration.Dhcp.Logging = Logging?.Logging;

        // Check if elevation is needed
        if (ElevatedFileService.RequiresElevation(ConfigFilePath))
        {
            // Prompt for password
            var password = await PasswordDialog.ShowPasswordDialogAsync();
            if (password == null)
            {
                // User cancelled
                return;
            }

            // Save with elevation
            var success = await _configService.SaveConfigurationWithElevationAsync(Configuration, ConfigFilePath, password);
            if (!success)
            {
                // Show error message - you might want to add a proper error dialog here
                System.Diagnostics.Debug.WriteLine("Failed to save configuration with elevated privileges");
                return;
            }
        }
        else
        {
            // Save normally
            try
            {
                _configService.SaveConfiguration(Configuration, ConfigFilePath);
            }
            catch (UnauthorizedAccessException)
            {
                // Fallback to elevated save
                var password = await PasswordDialog.ShowPasswordDialogAsync();
                if (password == null)
                {
                    return;
                }

                var success = await _configService.SaveConfigurationWithElevationAsync(Configuration, ConfigFilePath, password);
                if (!success)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save configuration");
                    return;
                }
            }
        }

        HasUnsavedChanges = false;
    }

    [RelayCommand]
    private void NewConfiguration()
    {
        Configuration = new DhcpConfiguration
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
        InitializeSubViewModels();
        HasUnsavedChanges = false;
    }
}
