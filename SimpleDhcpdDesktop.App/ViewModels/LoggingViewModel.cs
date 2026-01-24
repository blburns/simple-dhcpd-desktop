using CommunityToolkit.Mvvm.ComponentModel;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.ViewModels;

public partial class LoggingViewModel : ObservableObject
{
    private readonly LoggingConfiguration _logging;

    public LoggingViewModel(LoggingConfiguration logging)
    {
        _logging = logging;
    }

    public bool? Enable
    {
        get => _logging.Enable;
        set
        {
            if (_logging.Enable != value)
            {
                _logging.Enable = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Level
    {
        get => _logging.Level;
        set
        {
            if (_logging.Level != value)
            {
                _logging.Level = value;
                OnPropertyChanged();
            }
        }
    }

    public string? LogFile
    {
        get => _logging.LogFile;
        set
        {
            if (_logging.LogFile != value)
            {
                _logging.LogFile = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Format
    {
        get => _logging.Format;
        set
        {
            if (_logging.Format != value)
            {
                _logging.Format = value;
                OnPropertyChanged();
            }
        }
    }

    public bool? Rotation
    {
        get => _logging.Rotation;
        set
        {
            if (_logging.Rotation != value)
            {
                _logging.Rotation = value;
                OnPropertyChanged();
            }
        }
    }

    public string? MaxSize
    {
        get => _logging.MaxSize;
        set
        {
            if (_logging.MaxSize != value)
            {
                _logging.MaxSize = value;
                OnPropertyChanged();
            }
        }
    }

    public int? MaxFiles
    {
        get => _logging.MaxFiles;
        set
        {
            if (_logging.MaxFiles != value)
            {
                _logging.MaxFiles = value;
                OnPropertyChanged();
            }
        }
    }

    public LoggingConfiguration Logging => _logging;
}
