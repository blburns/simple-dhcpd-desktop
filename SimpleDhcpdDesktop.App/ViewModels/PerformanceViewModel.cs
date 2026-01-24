using CommunityToolkit.Mvvm.ComponentModel;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class PerformanceViewModel : ObservableObject
{
    private readonly PerformanceConfiguration _performance;

    public PerformanceViewModel(PerformanceConfiguration performance)
    {
        _performance = performance;
        
        if (_performance.LeaseDatabase == null)
            _performance.LeaseDatabase = new LeaseDatabaseConfiguration();
    }

    public int? MaxLeases
    {
        get => _performance.MaxLeases;
        set
        {
            if (_performance.MaxLeases != value)
            {
                _performance.MaxLeases = value;
                OnPropertyChanged();
            }
        }
    }

    public string? LeaseDatabaseType
    {
        get => _performance.LeaseDatabase?.Type;
        set
        {
            if (_performance.LeaseDatabase!.Type != value)
            {
                _performance.LeaseDatabase!.Type = value;
                OnPropertyChanged();
            }
        }
    }

    public string? LeaseDatabasePath
    {
        get => _performance.LeaseDatabase?.Path;
        set
        {
            if (_performance.LeaseDatabase!.Path != value)
            {
                _performance.LeaseDatabase!.Path = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? LeaseDatabaseBackup
    {
        get => _performance.LeaseDatabase?.Backup;
        set
        {
            if (_performance.LeaseDatabase!.Backup != value)
            {
                _performance.LeaseDatabase!.Backup = value;
                OnPropertyChanged();
            }
        }
    }

    public int? LeaseDatabaseBackupInterval
    {
        get => _performance.LeaseDatabase?.BackupInterval;
        set
        {
            if (_performance.LeaseDatabase!.BackupInterval != value)
            {
                _performance.LeaseDatabase!.BackupInterval = value;
                OnPropertyChanged();
            }
        }
    }

    public int? LeaseDatabaseBackupRetention
    {
        get => _performance.LeaseDatabase?.BackupRetention;
        set
        {
            if (_performance.LeaseDatabase!.BackupRetention != value)
            {
                _performance.LeaseDatabase!.BackupRetention = value;
                OnPropertyChanged();
            }
        }
    }

    public PerformanceConfiguration Performance => _performance;
}
