using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SimpleDhcpdDesktop.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public async Task<string?> ShowOpenFileDialogAsync(string? title = null, string? initialDirectory = null)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title ?? "Select Configuration File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                FilePickerFileTypes.All,
                new FilePickerFileType("Configuration Files")
                {
                    Patterns = new[] { "*.conf", "*.ini", "*.json", "*.yaml", "*.yml" },
                    MimeTypes = new[] { "application/json", "application/x-yaml", "text/plain", "text/ini" }
                }
            },
            SuggestedStartLocation = initialDirectory != null ? await StorageProvider.TryGetFolderFromPathAsync(initialDirectory) : null
        });

        return files.Count >= 1 ? files[0].Path.LocalPath : null;
    }

    public async Task<string?> ShowSaveFileDialogAsync(string? title = null, string? suggestedFileName = null, string? initialDirectory = null)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title ?? "Save Configuration File",
            SuggestedFileName = suggestedFileName ?? "simple-dhcpd.conf",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("INI Configuration (*.ini, *.conf)")
                {
                    Patterns = new[] { "*.ini", "*.conf" }
                },
                new FilePickerFileType("JSON Configuration (*.json)")
                {
                    Patterns = new[] { "*.json" }
                },
                new FilePickerFileType("YAML Configuration (*.yaml, *.yml)")
                {
                    Patterns = new[] { "*.yaml", "*.yml" }
                }
            },
            SuggestedStartLocation = initialDirectory != null ? await StorageProvider.TryGetFolderFromPathAsync(initialDirectory) : null
        });

        return file?.Path.LocalPath;
    }
}