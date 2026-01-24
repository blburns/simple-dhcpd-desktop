using System;
using System.Reflection;
using System.Text.Json;
using Avalonia.Platform;
using SimpleDhcpdDesktop.App.Models;

namespace SimpleDhcpdDesktop.App.Services;

public static class AppConfigurationLoader
{
    public static AppMetadata Load()
    {
        var assembly = Assembly.GetExecutingAssembly();

        try
        {
            var uri = new Uri("avares://SimpleDhcpdDesktop.App/Assets/appsettings.json");

            if (!AssetLoader.Exists(uri))
            {
                return AppMetadata.FromAssembly(assembly);
            }

            using var stream = AssetLoader.Open(uri);
            using var document = JsonDocument.Parse(stream);
            var root = document.RootElement;

            if (!root.TryGetProperty("App", out var appElement))
            {
                return AppMetadata.FromAssembly(assembly);
            }

            var metadata = new AppMetadata(
                appElement.GetPropertyOrDefault("Name", "Desktop Boilerplate"),
                appElement.GetPropertyOrDefault("Company", "Dreamlike Labs"),
                appElement.GetPropertyOrDefault("Description", "Starter shell for cross-platform desktop apps."),
                appElement.GetPropertyOrDefault("RepositoryUrl", string.Empty),
                appElement.GetPropertyOrDefault("DocumentationUrl", string.Empty),
                appElement.GetPropertyOrDefault("SupportEmail", string.Empty),
                assembly.GetName().Version?.ToString() ?? "0.1.0");

            return AppMetadata.FromAssembly(assembly, metadata);
        }
        catch (Exception)
        {
            return AppMetadata.FromAssembly(assembly);
        }
    }

    private static string GetPropertyOrDefault(this JsonElement element, string propertyName, string defaultValue)
        => element.TryGetProperty(propertyName, out var value) ? value.GetString() ?? defaultValue : defaultValue;
}

