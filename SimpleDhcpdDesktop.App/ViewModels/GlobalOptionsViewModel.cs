using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class GlobalOptionsViewModel : ObservableObject
{
    public GlobalOptionsViewModel(List<DhcpOption> options)
    {
        Options = new ObservableCollection<DhcpOptionViewModel>(
            options.Select(o => new DhcpOptionViewModel(o)));
    }

    [ObservableProperty]
    private ObservableCollection<DhcpOptionViewModel> options;

    [RelayCommand]
    private void AddOption()
    {
        Options.Add(new DhcpOptionViewModel(new DhcpOption
        {
            Name = "",
            Value = ""
        }));
    }

    [RelayCommand]
    private void RemoveOption(DhcpOptionViewModel? option)
    {
        if (option != null)
        {
            Options.Remove(option);
        }
    }
}
