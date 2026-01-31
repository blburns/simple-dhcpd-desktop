using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleDhcpdDesktop.App.Services;

namespace SimpleDhcpdDesktop.App.Views;

public partial class ValidationResultsDialog : Window
{
    public ValidationResultsDialog()
    {
        InitializeComponent();
    }

    public static async void ShowValidationResults(Window owner, ValidationResult result)
    {
        var dialog = new ValidationResultsDialog
        {
            DataContext = new ValidationResultsViewModel(result)
        };
        
        ((ValidationResultsViewModel)dialog.DataContext).CloseRequested += () => dialog.Close();
        
        await dialog.ShowDialog(owner);
    }
}

public partial class ValidationResultsViewModel : ObservableObject
{
    public event Action? CloseRequested;

    public ValidationResultsViewModel(ValidationResult result)
    {
        IsValid = result.IsValid && result.Warnings.Count == 0;
        HasErrors = result.Errors.Count > 0;
        HasWarnings = result.Warnings.Count > 0;
        ErrorCount = result.Errors.Count;
        WarningCount = result.Warnings.Count;
        Errors = result.Errors;
        Warnings = result.Warnings;
        Summary = result.GetSummary();
    }

    [ObservableProperty]
    private bool isValid;

    [ObservableProperty]
    private bool hasErrors;

    [ObservableProperty]
    private bool hasWarnings;

    [ObservableProperty]
    private int errorCount;

    [ObservableProperty]
    private int warningCount;

    [ObservableProperty]
    private List<string> errors = new();

    [ObservableProperty]
    private List<string> warnings = new();

    [ObservableProperty]
    private string summary = string.Empty;

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }
}
