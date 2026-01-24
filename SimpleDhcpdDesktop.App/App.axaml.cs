using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SimpleDhcpdDesktop.App.Services;
using SimpleDhcpdDesktop.App.ViewModels;

namespace SimpleDhcpdDesktop.App;

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
            var configService = new SimpleDhcpdDesktop.App.Services.DhcpConfigurationService();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new ShellViewModel(metadata, configService)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}