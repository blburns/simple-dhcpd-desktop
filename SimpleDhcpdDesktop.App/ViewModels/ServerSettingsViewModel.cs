using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class ServerSettingsViewModel : ObservableObject
{
    public ServerSettingsViewModel(DhcpSection dhcpSection)
    {
        ListenAddresses = new ObservableCollection<string>(
            dhcpSection.Listen ?? new List<string> { "0.0.0.0:67" });
    }

    [ObservableProperty]
    private ObservableCollection<string> listenAddresses;

    [RelayCommand]
    private void AddListenAddress()
    {
        ListenAddresses.Add("0.0.0.0:67");
    }

    [RelayCommand]
    public void RemoveListenAddress(string? address)
    {
        if (address != null)
        {
            ListenAddresses.Remove(address);
        }
    }
}
