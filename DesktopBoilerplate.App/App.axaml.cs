using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DesktopBoilerplate.App.Services;
using DesktopBoilerplate.App.ViewModels;

namespace DesktopBoilerplate.App;

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