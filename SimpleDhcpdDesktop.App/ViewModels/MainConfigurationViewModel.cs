using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;
using SimpleDhcpdDesktop.App.Services;
using SimpleDhcpdDesktop.App.Views;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class MainConfigurationViewModel : ObservableObject
{
    private readonly DhcpConfigurationService _configService;
    private readonly ConfigurationValidator _validator;
    private MainWindow? _mainWindow;

    public MainConfigurationViewModel(DhcpConfigurationService configService)
    {
        _configService = configService;
        _validator = new ConfigurationValidator();
        ConfigFilePath = configService.GetDefaultConfigPath();
        
        var config = _configService.LoadConfiguration(ConfigFilePath);
        Configuration = config;
        
        InitializeSubViewModels();
    }

    public void SetMainWindow(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
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

    // TODO: MonitoringViewModel requires SimpleDaemons.Desktop.Common library
    // [ObservableProperty]
    // private MonitoringViewModel? monitoring;

    [ObservableProperty]
    private bool hasUnsavedChanges;

    [ObservableProperty]
    private string statusMessage = "Ready";

    private void InitializeSubViewModels()
    {
        ServerSettings = new ServerSettingsViewModel(Configuration.Dhcp ?? new DhcpSection());
        Subnets = new SubnetsViewModel(Configuration.Dhcp?.Subnets ?? new List<SubnetConfiguration>());
        GlobalOptions = new GlobalOptionsViewModel(Configuration.Dhcp?.GlobalOptions ?? new List<DhcpOption>());
        Security = new SecurityViewModel(Configuration.Dhcp?.Security ?? new SecurityConfiguration());
        Performance = new PerformanceViewModel(Configuration.Dhcp?.Performance ?? new PerformanceConfiguration());
        Logging = new LoggingViewModel(Configuration.Dhcp?.Logging ?? new LoggingConfiguration());
        // TODO: MonitoringViewModel requires SimpleDaemons.Desktop.Common library
        // Monitoring = new MonitoringViewModel(Configuration.Dhcp?.Monitoring ?? new MonitoringConfiguration());

        // Subscribe to changes
        ServerSettings.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Subnets.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        GlobalOptions.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Security.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Performance.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        Logging.PropertyChanged += (s, e) => HasUnsavedChanges = true;
        // Monitoring.PropertyChanged += (s, e) => HasUnsavedChanges = true;
    }

    [RelayCommand]
    private void LoadConfiguration()
    {
        try
        {
            StatusMessage = "Loading configuration...";
            System.Diagnostics.Debug.WriteLine($"LoadConfiguration called for: {ConfigFilePath}");
            
            var config = _configService.LoadConfiguration(ConfigFilePath);
            Configuration = config;
            
            System.Diagnostics.Debug.WriteLine($"Configuration object set. Dhcp section is null: {Configuration.Dhcp == null}");
            if (Configuration.Dhcp != null)
            {
                System.Diagnostics.Debug.WriteLine($"Listen addresses: {Configuration.Dhcp.Listen?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"Subnets: {Configuration.Dhcp.Subnets?.Count ?? 0}");
            }
            
            InitializeSubViewModels();
            System.Diagnostics.Debug.WriteLine("SubViewModels initialized");
            
            HasUnsavedChanges = false;
            
            // Show success message with stats
            var subnetCount = Configuration.Dhcp?.Subnets?.Count ?? 0;
            var optionCount = Configuration.Dhcp?.GlobalOptions?.Count ?? 0;
            StatusMessage = $"Loaded: {subnetCount} subnet(s), {optionCount} option(s)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading configuration: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in LoadConfiguration: {ex}");
        }
    }

    [RelayCommand]
    private async Task SelectConfigurationFileAsync()
    {
        if (_mainWindow == null)
        {
            // Fallback to default path if window not available
            LoadConfiguration();
            return;
        }

        var selectedPath = await _mainWindow.ShowOpenFileDialogAsync("Select DHCP Configuration File");
        if (!string.IsNullOrEmpty(selectedPath))
        {
            ConfigFilePath = selectedPath;
            LoadConfiguration();
        }
    }

    [RelayCommand]
    private async Task SaveAsConfigurationAsync()
    {
        if (_mainWindow == null)
        {
            StatusMessage = "Cannot show file dialog";
            return;
        }

        var selectedPath = await _mainWindow.ShowSaveFileDialogAsync("Save DHCP Configuration", Path.GetFileName(ConfigFilePath));
        if (string.IsNullOrEmpty(selectedPath))
        {
            StatusMessage = "Save cancelled";
            return;
        }

        ConfigFilePath = selectedPath;
        await SaveConfigurationAsync();
    }

    [RelayCommand]
    private async Task SaveConfigurationAsync()
    {
        try
        {
            StatusMessage = "Saving configuration...";
            System.Diagnostics.Debug.WriteLine($"SaveConfiguration called for: {ConfigFilePath}");
            
            // Update configuration from view models
            if (Configuration.Dhcp == null)
            {
                Configuration.Dhcp = new DhcpSection();
            }

            Configuration.Dhcp.Listen = ServerSettings?.ListenAddresses?.ToList();
            Configuration.Dhcp.Subnets = Subnets?.Subnets?.Select(s => s.GetSubnetConfiguration()).ToList();
            Configuration.Dhcp.GlobalOptions = GlobalOptions?.Options?.Select(o => o.GetOption()).ToList();
            
            System.Diagnostics.Debug.WriteLine($"Collected {Configuration.Dhcp.Listen?.Count ?? 0} listen addresses");
            System.Diagnostics.Debug.WriteLine($"Collected {Configuration.Dhcp.Subnets?.Count ?? 0} subnets");
            System.Diagnostics.Debug.WriteLine($"Collected {Configuration.Dhcp.GlobalOptions?.Count ?? 0} global options");
            
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
            // TODO: Monitoring requires SimpleDaemons.Desktop.Common library
            // if (Monitoring != null)
            // {
            //     Monitoring.Monitoring.HealthChecks!.Checks = Monitoring.HealthChecks.Select(c => c.GetHealthCheck()).ToList();
            // }
            // Configuration.Dhcp.Monitoring = Monitoring?.Monitoring;

            // Check if elevation is needed
            if (ElevatedFileService.RequiresElevation(ConfigFilePath))
            {
                System.Diagnostics.Debug.WriteLine("Elevation required for file path");
                // Prompt for password
                var password = await PasswordDialog.ShowPasswordDialogAsync();
                if (password == null)
                {
                    // User cancelled
                    StatusMessage = "Save cancelled";
                    System.Diagnostics.Debug.WriteLine("User cancelled password prompt");
                    return;
                }

                // Save with elevation
                var success = await _configService.SaveConfigurationWithElevationAsync(Configuration, ConfigFilePath, password);
                if (!success)
                {
                    StatusMessage = "Failed to save configuration (elevated)";
                    System.Diagnostics.Debug.WriteLine("Failed to save configuration with elevated privileges");
                    return;
                }
            }
            else
            {
                // Save normally
                try
                {
                    System.Diagnostics.Debug.WriteLine("Attempting normal save...");
                    _configService.SaveConfiguration(Configuration, ConfigFilePath);
                    System.Diagnostics.Debug.WriteLine("Save completed successfully");
                }
                catch (UnauthorizedAccessException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"UnauthorizedAccessException: {ex.Message}");
                    // Fallback to elevated save
                    var password = await PasswordDialog.ShowPasswordDialogAsync();
                    if (password == null)
                    {
                        StatusMessage = "Save cancelled";
                        return;
                    }

                    var success = await _configService.SaveConfigurationWithElevationAsync(Configuration, ConfigFilePath, password);
                    if (!success)
                    {
                        StatusMessage = "Failed to save configuration";
                        System.Diagnostics.Debug.WriteLine("Failed to save configuration");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error saving: {ex.Message}";
                    System.Diagnostics.Debug.WriteLine($"Error saving configuration: {ex}");
                    return;
                }
            }

            HasUnsavedChanges = false;
            StatusMessage = "Configuration saved successfully";
            System.Diagnostics.Debug.WriteLine("Configuration saved and HasUnsavedChanges set to false");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in SaveConfigurationAsync: {ex}");
        }
    }

    [RelayCommand]
    private void ValidateConfiguration()
    {
        try
        {
            StatusMessage = "Validating configuration...";
            System.Diagnostics.Debug.WriteLine("Starting configuration validation");
            
            // Update configuration from view models first
            if (Configuration.Dhcp == null)
            {
                Configuration.Dhcp = new DhcpSection();
            }

            Configuration.Dhcp.Listen = ServerSettings?.ListenAddresses?.ToList();
            Configuration.Dhcp.Subnets = Subnets?.Subnets?.Select(s => s.GetSubnetConfiguration()).ToList();
            Configuration.Dhcp.GlobalOptions = GlobalOptions?.Options?.Select(o => o.GetOption()).ToList();
            
            var result = _validator.Validate(Configuration);
            
            System.Diagnostics.Debug.WriteLine($"Validation complete. Valid: {result.IsValid}");
            System.Diagnostics.Debug.WriteLine($"Errors: {result.Errors.Count}, Warnings: {result.Warnings.Count}");
            
            if (result.Errors.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Errors:");
                foreach (var error in result.Errors)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {error}");
                }
            }
            
            if (result.Warnings.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Warnings:");
                foreach (var warning in result.Warnings)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {warning}");
                }
            }
            
            StatusMessage = result.GetSummary();
            
            // Show validation results in a dialog
            if (_mainWindow != null)
            {
                Views.ValidationResultsDialog.ShowValidationResults(_mainWindow, result);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Validation error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Error in ValidateConfiguration: {ex}");
        }
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
                // Monitoring = new MonitoringConfiguration()
            }
        };
        InitializeSubViewModels();
        HasUnsavedChanges = false;
        StatusMessage = "New configuration created";
    }
}
