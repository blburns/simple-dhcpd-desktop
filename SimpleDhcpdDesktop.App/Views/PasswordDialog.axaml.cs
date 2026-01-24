using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SimpleDhcpdDesktop.App.Views;

public partial class PasswordDialog : Window
{
    public string? Password { get; private set; }
    public bool DialogResult { get; private set; }

    public PasswordDialog()
    {
        InitializeComponent();
        PasswordBox.Focus();
        
        // Handle Enter key
        PasswordBox.KeyDown += (s, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Enter)
            {
                OkButton_Click(s, e);
            }
        };
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Password = PasswordBox.Text;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Password = null;
        DialogResult = false;
        Close();
    }

    public static async Task<string?> ShowPasswordDialogAsync(Window? parent = null)
    {
        var dialog = new PasswordDialog();
        
        // Try to get the main window if parent is not provided
        Window? owner = parent;
        if (owner == null && Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            owner = desktop.MainWindow;
        }
        
        // ShowDialog requires an owner, so use the owner if available
        if (owner != null)
        {
            dialog.Owner = owner;
            await dialog.ShowDialog(owner);
        }
        else
        {
            // If no owner available, show as a standalone window
            dialog.Show();
            await Task.Delay(100); // Give it time to show
            // Wait for the dialog to close
            while (dialog.IsVisible)
            {
                await Task.Delay(50);
            }
        }
        
        return dialog.DialogResult ? dialog.Password : null;
    }
}
