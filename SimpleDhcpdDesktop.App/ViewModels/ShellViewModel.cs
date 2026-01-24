using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;
using SimpleDhcpdDesktop.App.Services;
using SimpleDhcpdDesktop.App.Utilities;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    private readonly DhcpConfigurationService _configService;

    public ShellViewModel()
        : this(AppMetadata.FromAssembly(typeof(ShellViewModel).Assembly), new DhcpConfigurationService())
    {
    }

    public ShellViewModel(AppMetadata metadata, DhcpConfigurationService configService)
    {
        _configService = configService;
        Metadata = metadata;
        NavigationItems = new ObservableCollection<NavigationItem>(BuildNavigation());
        selectedItem = NavigationItems.FirstOrDefault();
        
        Configuration = new MainConfigurationViewModel(_configService);

        OpenDocumentationCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync(Metadata.DocumentationUrl));
        OpenRepositoryCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync(Metadata.RepositoryUrl));
        ContactSupportCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync($"mailto:{Metadata.SupportEmail}"));
    }

    public AppMetadata Metadata { get; }

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    [ObservableProperty]
    private NavigationItem? selectedItem;

    [ObservableProperty]
    private MainConfigurationViewModel configuration;

    public IAsyncRelayCommand OpenDocumentationCommand { get; }

    public IAsyncRelayCommand OpenRepositoryCommand { get; }

    public IAsyncRelayCommand ContactSupportCommand { get; }

    public string HeaderSubtitle => $"{Metadata.Company} · v{Metadata.Version}";

    private static IEnumerable<NavigationItem> BuildNavigation()
        => new[]
        {
            new NavigationItem("overview", "Overview", "DHCP server overview and status", "📊", true),
            new NavigationItem("server", "Server Settings", "Configure listen addresses and basic settings", "🖥️", true),
            new NavigationItem("subnets", "Subnets", "Manage DHCP subnets and IP ranges", "🌐", true),
            new NavigationItem("options", "Global Options", "Configure global DHCP options", "⚙️", true),
            new NavigationItem("security", "Security", "Security settings and filtering", "🔒", true),
            new NavigationItem("performance", "Performance", "Performance and lease database settings", "⚡", true),
            new NavigationItem("logging", "Logging", "Logging configuration", "📝", true)
        };
}

