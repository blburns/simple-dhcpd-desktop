using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopBoilerplate.App.Models;
using DesktopBoilerplate.App.Utilities;

namespace DesktopBoilerplate.App.ViewModels;

public partial class ShellViewModel : ObservableObject
{
    public ShellViewModel()
        : this(AppMetadata.FromAssembly(typeof(ShellViewModel).Assembly))
    {
    }

    public ShellViewModel(AppMetadata metadata)
    {
        Metadata = metadata;
        NavigationItems = new ObservableCollection<NavigationItem>(BuildNavigation());
        selectedItem = NavigationItems.FirstOrDefault();

        OpenDocumentationCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync(Metadata.DocumentationUrl));
        OpenRepositoryCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync(Metadata.RepositoryUrl));
        ContactSupportCommand = new AsyncRelayCommand(() => PlatformLauncher.OpenUrlAsync($"mailto:{Metadata.SupportEmail}"));
    }

    public AppMetadata Metadata { get; }

    public ObservableCollection<NavigationItem> NavigationItems { get; }

    [ObservableProperty]
    private NavigationItem? selectedItem;

    public IAsyncRelayCommand OpenDocumentationCommand { get; }

    public IAsyncRelayCommand OpenRepositoryCommand { get; }

    public IAsyncRelayCommand ContactSupportCommand { get; }

    public string HeaderSubtitle => $"{Metadata.Company} · v{Metadata.Version}";

    private static IEnumerable<NavigationItem> BuildNavigation()
        => new[]
        {
            new NavigationItem("home", "Overview", "Stay on top of key metrics and quick actions.", "", false),
            new NavigationItem("library", "Library", "Browse your content and recent files.", "", false),
            new NavigationItem("activity", "Activity", "Review the latest events and notifications.", "", false),
            new NavigationItem("settings", "Settings", "Configure preferences and integrations.", "", false)
        };
}

