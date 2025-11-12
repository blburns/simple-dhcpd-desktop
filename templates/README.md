# Avalonia Boilerplate Template

This folder packages our opinionated desktop shell as a reusable `dotnet new` template. It keeps the metadata and host configuration from the official [`AvaloniaUI/avalonia-dotnet-templates`](https://github.com/AvaloniaUI/avalonia-dotnet-templates) repo, but swaps in the richer layout, MVVM setup, and configuration helpers we built for the boilerplate app.

Key additions over the stock `avalonia.app` template:

- Fluent-inspired shell with navigation rail, resizable sidebar, and dashboard-style cards
- `CommunityToolkit.Mvvm`-based `ShellViewModel` wired for compiled bindings
- JSON-driven branding via `Assets/appsettings.json` and `AppConfigurationLoader`
- Shared resource dictionaries for colours, spacing, and reusable styles
- Cross-platform runtime identifiers (Windows x64/x86, macOS x64, Linux x64) baked into the project file

## Try it out

Install the template locally and spin up a sample project:

```bash
dotnet new install ./templates/csharp/app
dotnet new avalonia.app -o MyPrototype
```

You can now iterate on `MyPrototype` just like any other Avalonia project.

> The upstream documentation for managing and publishing Avalonia templates lives in the official repository README.[^1]

[^1]: AvaloniaUI, “Avalonia Templates for `dotnet new`,” GitHub, accessed Nov 2025, https://github.com/AvaloniaUI/avalonia-dotnet-templates. 

