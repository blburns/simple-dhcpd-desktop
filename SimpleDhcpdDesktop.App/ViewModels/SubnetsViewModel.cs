using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class SubnetsViewModel : ObservableObject
{
    public SubnetsViewModel(List<SubnetConfiguration> subnets)
    {
        Subnets = new ObservableCollection<SubnetViewModel>(
            subnets.Select(s => new SubnetViewModel(s)));
    }

    [ObservableProperty]
    private ObservableCollection<SubnetViewModel> subnets;

    [ObservableProperty]
    private SubnetViewModel? selectedSubnet;

    [RelayCommand]
    private void AddSubnet()
    {
        var newSubnet = new SubnetConfiguration
        {
            Name = "New Subnet",
            Network = "192.168.1.0/24",
            Range = "192.168.1.100-192.168.1.200",
            Gateway = "192.168.1.1",
            DnsServers = new List<string> { "8.8.8.8", "8.8.4.4" },
            DomainName = "local",
            LeaseTime = 86400,
            MaxLeaseTime = 172800,
            Options = new List<DhcpOption>(),
            Reservations = new List<Reservation>(),
            Exclusions = new List<Exclusion>()
        };
        Subnets.Add(new SubnetViewModel(newSubnet));
        SelectedSubnet = Subnets.Last();
    }

    [RelayCommand]
    private void RemoveSubnet(SubnetViewModel? subnet)
    {
        if (subnet != null)
        {
            Subnets.Remove(subnet);
            if (SelectedSubnet == subnet)
            {
                SelectedSubnet = Subnets.FirstOrDefault();
            }
        }
    }
}

public partial class SubnetViewModel : ObservableObject
{
    private readonly SubnetConfiguration _subnet;

    public SubnetViewModel(SubnetConfiguration subnet)
    {
        _subnet = subnet;
        Options = new ObservableCollection<DhcpOptionViewModel>(
            (subnet.Options ?? new List<DhcpOption>()).Select(o => new DhcpOptionViewModel(o)));
        Reservations = new ObservableCollection<ReservationViewModel>(
            (subnet.Reservations ?? new List<Reservation>()).Select(r => new ReservationViewModel(r)));
        Exclusions = new ObservableCollection<ExclusionViewModel>(
            (subnet.Exclusions ?? new List<Exclusion>()).Select(e => new ExclusionViewModel(e)));
    }

    public string? Name
    {
        get => _subnet.Name;
        set
        {
            if (_subnet.Name != value)
            {
                _subnet.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Network
    {
        get => _subnet.Network;
        set
        {
            if (_subnet.Network != value)
            {
                _subnet.Network = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Range
    {
        get => _subnet.Range;
        set
        {
            if (_subnet.Range != value)
            {
                _subnet.Range = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Gateway
    {
        get => _subnet.Gateway;
        set
        {
            if (_subnet.Gateway != value)
            {
                _subnet.Gateway = value;
                OnPropertyChanged();
            }
        }
    }

    public string? DomainName
    {
        get => _subnet.DomainName;
        set
        {
            if (_subnet.DomainName != value)
            {
                _subnet.DomainName = value;
                OnPropertyChanged();
            }
        }
    }

    public int? LeaseTime
    {
        get => _subnet.LeaseTime;
        set
        {
            if (_subnet.LeaseTime != value)
            {
                _subnet.LeaseTime = value;
                OnPropertyChanged();
            }
        }
    }

    public int? MaxLeaseTime
    {
        get => _subnet.MaxLeaseTime;
        set
        {
            if (_subnet.MaxLeaseTime != value)
            {
                _subnet.MaxLeaseTime = value;
                OnPropertyChanged();
            }
        }
    }

    public string DnsServersText
    {
        get => string.Join(", ", _subnet.DnsServers ?? new List<string>());
        set
        {
            var servers = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            _subnet.DnsServers = servers;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private ObservableCollection<DhcpOptionViewModel> options;

    [ObservableProperty]
    private ObservableCollection<ReservationViewModel> reservations;

    [ObservableProperty]
    private ObservableCollection<ExclusionViewModel> exclusions;

    public SubnetConfiguration GetSubnetConfiguration()
    {
        _subnet.Options = Options.Select(o => o.GetOption()).ToList();
        _subnet.Reservations = Reservations.Select(r => r.GetReservation()).ToList();
        _subnet.Exclusions = Exclusions.Select(e => e.GetExclusion()).ToList();
        return _subnet;
    }
}

public partial class DhcpOptionViewModel : ObservableObject
{
    private readonly DhcpOption _option;

    public DhcpOptionViewModel(DhcpOption option)
    {
        _option = option;
    }

    public string? Name
    {
        get => _option.Name;
        set
        {
            if (_option.Name != value)
            {
                _option.Name = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Value
    {
        get => _option.Value;
        set
        {
            if (_option.Value != value)
            {
                _option.Value = value;
                OnPropertyChanged();
            }
        }
    }

    public DhcpOption GetOption() => _option;
}

public partial class ReservationViewModel : ObservableObject
{
    private readonly Reservation _reservation;

    public ReservationViewModel(Reservation reservation)
    {
        _reservation = reservation;
    }

    public string? MacAddress
    {
        get => _reservation.MacAddress;
        set
        {
            if (_reservation.MacAddress != value)
            {
                _reservation.MacAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public string? IpAddress
    {
        get => _reservation.IpAddress;
        set
        {
            if (_reservation.IpAddress != value)
            {
                _reservation.IpAddress = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Hostname
    {
        get => _reservation.Hostname;
        set
        {
            if (_reservation.Hostname != value)
            {
                _reservation.Hostname = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Description
    {
        get => _reservation.Description;
        set
        {
            if (_reservation.Description != value)
            {
                _reservation.Description = value;
                OnPropertyChanged();
            }
        }
    }

    public Reservation GetReservation() => _reservation;
}

public partial class ExclusionViewModel : ObservableObject
{
    private readonly Exclusion _exclusion;

    public ExclusionViewModel(Exclusion exclusion)
    {
        _exclusion = exclusion;
    }

    public string? Start
    {
        get => _exclusion.Start;
        set
        {
            if (_exclusion.Start != value)
            {
                _exclusion.Start = value;
                OnPropertyChanged();
            }
        }
    }

    public string? End
    {
        get => _exclusion.End;
        set
        {
            if (_exclusion.End != value)
            {
                _exclusion.End = value;
                OnPropertyChanged();
            }
        }
    }

    public Exclusion GetExclusion() => _exclusion;
}
