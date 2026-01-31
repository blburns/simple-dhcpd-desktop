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
            var shellViewModel = new ShellViewModel(metadata, configService);
            
            var mainWindow = new MainWindow
            {
                DataContext = shellViewModel
            };
            
            // Pass MainWindow reference to MainConfigurationViewModel for file dialogs
            shellViewModel.Configuration.SetMainWindow(mainWindow);
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}