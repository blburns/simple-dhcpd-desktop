using System.Reflection;

namespace SimpleDhcpdDesktop.App.Models;

public sealed record AppMetadata(
    string Name,
    string Company,
    string Description,
    string RepositoryUrl,
    string DocumentationUrl,
    string SupportEmail,
    string Version)
{
    public static AppMetadata FromAssembly(Assembly assembly, AppMetadata? overrides = null)
    {
        var name = overrides?.Name ?? assembly.GetName().Name ?? "Desktop Boilerplate";
        var company = overrides?.Company ?? "Contoso Labs";
        var description = overrides?.Description ?? "Starter shell for cross-platform desktop apps.";
        var repositoryUrl = overrides?.RepositoryUrl ?? string.Empty;
        var documentationUrl = overrides?.DocumentationUrl ?? string.Empty;
        var supportEmail = overrides?.SupportEmail ?? string.Empty;
        var version = overrides?.Version ?? assembly.GetName().Version?.ToString() ?? "0.1.0";

        return new AppMetadata(
            name,
            company,
            description,
            repositoryUrl,
            documentationUrl,
            supportEmail,
            version);
    }
}

