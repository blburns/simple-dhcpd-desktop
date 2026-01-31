using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using SimpleDhcpdDesktop.App.ViewModels;
using SimpleDhcpdDesktop.App.Views;

namespace SimpleDhcpdDesktop.App.Converters;

/// <summary>
/// Navigation Content Converter
/// 
/// A multi-value converter that dynamically creates the appropriate view based on
/// the selected navigation section ID. Used in the main window to display different
/// configuration views in the content area based on sidebar selection.
/// 
/// Input values:
/// - values[0]: Section ID string (e.g., "overview", "server", "subnets", etc.)
/// - values[1]: MainConfigurationViewModel instance
/// 
/// Output: A UserControl view instance with the appropriate DataContext set.
/// 
/// This converter enables dynamic view switching without maintaining a dictionary
/// of views or using a view locator pattern.
/// </summary>
public class NavigationContentConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts navigation section ID and configuration ViewModel into the appropriate view.
    /// 
    /// Creates a new instance of the view corresponding to the section ID and sets
    /// its DataContext to the MainConfigurationViewModel. If the section ID is invalid
    /// or the configuration is null, returns a placeholder TextBlock.
    /// 
    /// Supported section IDs:
    /// - "overview": OverviewView (dashboard)
    /// - "server": ServerSettingsView (server configuration)
    /// - "subnets": SubnetsView (subnet management)
    /// - "options": GlobalOptionsView (global DHCP options)
    /// - "security": SecurityView (security settings)
    /// - "performance": PerformanceView (performance settings)
    /// - "logging": LoggingView (logging configuration)
    /// - "monitoring": MonitoringView (monitoring and health checks)
    /// 
    /// Default: Returns OverviewView if section ID is unrecognized.
    /// </summary>
    /// <param name="values">Array containing [sectionId, MainConfigurationViewModel]</param>
    /// <param name="targetType">Target type (not used)</param>
    /// <param name="parameter">Converter parameter (not used)</param>
    /// <param name="culture">Culture info (not used)</param>
    /// <returns>UserControl view instance or placeholder TextBlock</returns>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        // Validate input - need at least section ID and configuration
        if (values.Count < 2)
        {
            return new TextBlock { Text = "Loading..." };
        }

        // Extract section ID (default to "overview" if not provided)
        var sectionId = values[0] as string ?? "overview";
        // Extract configuration ViewModel
        var config = values[1] as MainConfigurationViewModel;

        // If configuration is not available, show error message
        if (config == null)
        {
            return new TextBlock { Text = "Configuration not available" };
        }

        // Create and return the appropriate view based on section ID
        return sectionId switch
        {
            "overview" => new OverviewView { DataContext = config },
            "server" => new ServerSettingsView { DataContext = config },
            "subnets" => new SubnetsView { DataContext = config },
            "options" => new GlobalOptionsView { DataContext = config },
            "security" => new SecurityView { DataContext = config },
            "performance" => new PerformanceView { DataContext = config },
            "logging" => new LoggingView { DataContext = config },
            // TODO: MonitoringView requires SimpleDaemons.Desktop.Common library
            // "monitoring" => new MonitoringView { DataContext = config },
            // Default to overview for unknown section IDs
            _ => new OverviewView { DataContext = config }
        };
    }

    /// <summary>
    /// Reverse conversion (not implemented).
    /// 
    /// This converter is one-way only (view creation from navigation selection).
    /// Reverse conversion (extracting section ID from a view) is not needed.
    /// </summary>
    /// <param name="value">The value to convert back</param>
    /// <param name="targetTypes">Target types</param>
    /// <param name="parameter">Converter parameter</param>
    /// <param name="culture">Culture info</param>
    /// <returns>Not implemented - throws NotImplementedException</returns>
    /// <exception cref="NotImplementedException">Always thrown - reverse conversion not supported</exception>
    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
