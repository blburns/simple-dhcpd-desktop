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
            new NavigationItem("home", "Overview", "Stay on top of key metrics and quick actions.", "\uE80F"),
            new NavigationItem("library", "Library", "Browse your content and recent files.", "\uE8B7"),
            new NavigationItem("activity", "Activity", "Review the latest events and notifications.", "\uE7F4"),
            new NavigationItem("settings", "Settings", "Configure preferences and integrations.", "\uE713", false)
        };
}

