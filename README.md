# Desktop Boilerplate (Avalonia UI)

A cross-platform desktop starter built with [Avalonia UI](https://avaloniaui.net/) and .NET 8. The layout and project structure are opinionated toward productivity apps like Spotify, VS Code, or Slack—complete with navigation, theming, and platform-aware helpers.

> 📖 **For comprehensive documentation, see [docs/DOCUMENTATION.md](./docs/DOCUMENTATION.md)**

## Prerequisites

- **.NET 8 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- Verify installation: `dotnet --version` (should show 8.0.x or later)

## Features

- **True cross platform**: single `net8.0` target with runtime identifiers for Windows, macOS (Intel + Apple Silicon), and Linux (x64/ARM64).
- **MVVM ready**: leverages `CommunityToolkit.Mvvm` with a `ShellViewModel` wired up for compiled bindings.
- **Theming + resources**: shared resource dictionaries for colors, spacing, and component styles.
- **Config-driven branding**: update `Assets/appsettings.json` for app name, description, URLs, and support info.
- **Platform helpers**: utility for opening browser links or mail clients using native mechanisms.

## Structure

- `App.axaml` / `App.axaml.cs`: application bootstrap and DI-light wiring for the shell.
- `MainWindow.axaml`: shell layout with header, navigation rail, and content panels.
- `ViewModels/`: view models ready for expansion.
- `Models/`: shared data contracts such as `AppMetadata` and `NavigationItem`.
- `Services/`: lightweight configuration loader with Avalonia asset support.
- `Resources/Styles/`: Fluent-inspired resource dictionaries for branding and spacing.
- `Assets/`: configuration and future static assets (icons, images, etc.).
- `templates/`: customized `dotnet new` template that packages this boilerplate (metadata sourced from the upstream `avalonia.app` template).

## Quick Start

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project DesktopBoilerplate.App
```

> 💡 **Tip**: Use `dotnet watch --project DesktopBoilerplate.App` for hot reload during development

## Cross-platform publishing

```bash
# Windows
dotnet publish DesktopBoilerplate.App -c Release -r win-x64 --self-contained false

# macOS (Universal)
dotnet publish DesktopBoilerplate.App -c Release -r osx-x64 --self-contained true
dotnet publish DesktopBoilerplate.App -c Release -r osx-arm64 --self-contained true

# Linux
dotnet publish DesktopBoilerplate.App -c Release -r linux-x64 --self-contained true
dotnet publish DesktopBoilerplate.App -c Release -r linux-arm64 --self-contained true
```

Use `--self-contained true` when you want to ship the runtime; omit (or set false) when targeting environments with the .NET runtime pre-installed.

## Customize

- Update `Assets/appsettings.json` for branding and support links.
- Replace or extend the navigation items in `ViewModels/ShellViewModel`.
- Add new views and view models under `Views/` and `ViewModels/` respectively, then register them with navigation.
- Drop your own icons, fonts, or imagery under `Assets/` (they're already marked as Avalonia resources in the project file).

## Testing the layout

Avalonia supports XAML Hot Reload along with `dotnet watch`:

```bash
dotnet watch --project DesktopBoilerplate.App
```

## Documentation

For detailed information about:
- Architecture and design patterns
- Configuration options
- Customization guide
- Building and publishing
- Troubleshooting

See [docs/DOCUMENTATION.md](./docs/DOCUMENTATION.md)

### Managing Multiple .NET SDK Versions

If you have multiple .NET SDK versions installed, see [docs/SDK_MANAGEMENT.md](./docs/SDK_MANAGEMENT.md) for guidance on:
- Selecting which SDK version to use
- Using `global.json` to pin SDK versions
- Troubleshooting SDK version issues

## License

MIT – tweak to suit your organization.

