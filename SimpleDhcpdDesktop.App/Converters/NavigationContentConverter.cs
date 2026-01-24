using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using SimpleDhcpdDesktop.App.ViewModels;
using SimpleDhcpdDesktop.App.Views;

namespace SimpleDhcpdDesktop.App.Converters;

public class NavigationContentConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
        {
            return new TextBlock { Text = "Loading..." };
        }

        var sectionId = values[0] as string ?? "overview";
        var config = values[1] as MainConfigurationViewModel;

        if (config == null)
        {
            return new TextBlock { Text = "Configuration not available" };
        }

        return sectionId switch
        {
            "overview" => new OverviewView { DataContext = config },
            "server" => new ServerSettingsView { DataContext = config },
            "subnets" => new SubnetsView { DataContext = config },
            "options" => new GlobalOptionsView { DataContext = config },
            "security" => new SecurityView { DataContext = config },
            "performance" => new PerformanceView { DataContext = config },
            "logging" => new LoggingView { DataContext = config },
            _ => new OverviewView { DataContext = config }
        };
    }

    public object[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
