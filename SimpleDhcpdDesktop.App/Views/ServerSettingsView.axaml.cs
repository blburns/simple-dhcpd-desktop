using Avalonia.Controls;
using Avalonia.Interactivity;
using SimpleDhcpdDesktop.App.ViewModels;

namespace SimpleDhcpdDesktop.App.Views;

public partial class ServerSettingsView : UserControl
{
    public ServerSettingsView()
    {
        InitializeComponent();
    }

    private void RemoveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is string address && DataContext is MainConfigurationViewModel vm)
        {
            vm.ServerSettings?.RemoveListenAddress(address);
        }
    }
}
