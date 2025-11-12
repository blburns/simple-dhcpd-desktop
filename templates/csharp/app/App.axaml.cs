using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaAppTemplate.Services;
using AvaloniaAppTemplate.ViewModels;

namespace AvaloniaAppTemplate;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var metadata = AppConfigurationLoader.Load();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new ShellViewModel(metadata)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}