# Desktop Boilerplate Application Documentation

## Overview

Desktop Boilerplate is a cross-platform desktop application starter template built with [Avalonia UI](https://avaloniaui.net/) and .NET 8. It provides a modern, production-ready foundation for building desktop applications that run on Windows, macOS (Intel + Apple Silicon), and Linux.

The application features a clean, modern UI inspired by productivity applications like Spotify, VS Code, and Slack, with a navigation sidebar, theming system, and platform-aware utilities.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Customization](#customization)
- [Building and Publishing](#building-and-publishing)
- [Development Workflow](#development-workflow)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before running this application, ensure you have the following installed:

1. **.NET 8 SDK** or later
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version` (should show 8.0.x or later)

2. **Platform-specific requirements:**
   - **Windows**: Windows 10/11 (x64, x86, or ARM64)
   - **macOS**: macOS 10.15 or later (Intel or Apple Silicon)
   - **Linux**: Modern Linux distribution with GTK3 or Wayland support

### Managing Multiple .NET SDK Versions

If you have multiple .NET SDK versions installed, you can control which one is used for this project:

1. **List all installed SDKs:**
   ```bash
   dotnet --list-sdks
   ```

2. **Check which SDK is currently active:**
   ```bash
   dotnet --version
   ```

3. **Pin SDK version using `global.json`:**
   
   This project includes a `global.json` file that specifies the SDK version. The file uses:
   - `version`: Minimum SDK version required (8.0.0)
   - `rollForward`: How to handle version matching
     - `latestMinor`: Use the latest minor version (e.g., 8.0.0 → 8.0.100)
     - `latestPatch`: Use the latest patch version
     - `latestMajor`: Use the latest major version
     - `disable`: Use exact version only
   
   Example `global.json`:
   ```json
   {
     "sdk": {
       "version": "8.0.0",
       "rollForward": "latestMinor"
     }
   }
   ```

4. **Override SDK version (temporarily):**
   ```bash
   # Use a specific SDK version for a single command
   DOTNET_ROOT=/path/to/specific/sdk dotnet build
   ```

5. **Install additional SDK versions:**
   - Download from: https://dotnet.microsoft.com/download
   - Or use package managers:
     - **macOS**: `brew install dotnet@8`
     - **Windows**: Use the installer
     - **Linux**: Use distribution-specific package managers

## Installation

1. **Clone or navigate to the project directory:**
   ```bash
   cd /path/to/csharp-desktop-app
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

## Running the Application

### Quick Start

Run the application in development mode:

```bash
dotnet run --project DesktopBoilerplate.App
```

### Development with Hot Reload

For a better development experience with XAML Hot Reload:

```bash
dotnet watch --project DesktopBoilerplate.App
```

This will automatically rebuild and restart the application when you make changes to the code.

### Running from Visual Studio / Rider

1. Open `DesktopBoilerplate.sln` in your IDE
2. Set `DesktopBoilerplate.App` as the startup project
3. Press F5 or click Run

## Architecture

### MVVM Pattern

The application follows the Model-View-ViewModel (MVVM) pattern using `CommunityToolkit.Mvvm`:

- **Models**: Data structures (`AppMetadata`, `NavigationItem`)
- **Views**: XAML files (`MainWindow.axaml`, `App.axaml`)
- **ViewModels**: Business logic and data binding (`ShellViewModel`)

### Key Components

#### Application Bootstrap (`Program.cs`)
- Initializes the Avalonia application
- Configures platform detection and fonts
- Entry point for the desktop application

#### Application Class (`App.axaml.cs`)
- Loads XAML resources and styles
- Initializes the main window with the view model
- Loads application configuration from `appsettings.json`

#### Main Window (`MainWindow.axaml`)
- Shell layout with header, navigation sidebar, and content area
- Responsive design with resizable panels
- Modern UI with acrylic blur effects

#### Shell ViewModel (`ShellViewModel.cs`)
- Manages navigation state
- Handles commands for opening documentation, repository, and support
- Loads application metadata from configuration

## Project Structure

```
DesktopBoilerplate.App/
├── App.axaml                    # Application-level XAML resources
├── App.axaml.cs                # Application initialization logic
├── Program.cs                   # Application entry point
├── MainWindow.axaml            # Main window layout
├── MainWindow.axaml.cs         # Main window code-behind
├── Assets/
│   └── appsettings.json        # Application configuration
├── Models/
│   ├── AppMetadata.cs          # Application metadata model
│   └── NavigationItem.cs       # Navigation item model
├── Services/
│   └── AppConfigurationLoader.cs  # Configuration loading service
├── Utilities/
│   └── PlatformLauncher.cs    # Cross-platform URL/browser launcher
├── ViewModels/
│   └── ShellViewModel.cs      # Main shell view model
└── Resources/
    └── Styles/
        ├── Colors.axaml        # Color definitions
        └── Styles.axaml        # Global styles
```

## Configuration

### Application Settings

Edit `Assets/appsettings.json` to customize your application:

```json
{
  "App": {
    "Name": "Your App Name",
    "Company": "Your Company",
    "Description": "Your app description",
    "RepositoryUrl": "https://github.com/your-org/your-repo",
    "DocumentationUrl": "https://your-docs-url.com",
    "SupportEmail": "support@yourcompany.com"
  }
}
```

These settings are automatically loaded at startup and displayed in the application UI.

### Theming

#### Colors

Edit `Resources/Styles/Colors.axaml` to customize the color scheme:

- `BrandColor`: Primary brand color (default: #FF5058F5)
- `AppBackground`: Main application background
- `SidebarBackground`: Navigation sidebar background
- `CardBackground`: Card/panel background
- `AccentForeground`: Text color for accents

#### Styles

Edit `Resources/Styles/Styles.axaml` to customize:
- Typography (heading, subheading styles)
- Spacing values
- Corner radius values
- Button styles

## Customization

### Adding Navigation Items

Edit `ShellViewModel.cs` and modify the `BuildNavigation()` method:

```csharp
private static IEnumerable<NavigationItem> BuildNavigation()
    => new[]
    {
        new NavigationItem("home", "Overview", "Description", "🏠", false),
        new NavigationItem("library", "Library", "Description", "📚", false),
        // Add your navigation items here
    };
```

### Adding New Views

1. Create a new XAML file in a `Views/` folder (create if needed):
   ```xml
   <UserControl xmlns="https://github.com/avaloniaui">
       <!-- Your view content -->
   </UserControl>
   ```

2. Create a corresponding ViewModel:
   ```csharp
   public partial class YourViewModel : ObservableObject
   {
       // Your view model logic
   }
   ```

3. Update navigation to include your new view

### Adding Services

1. Create a service class in the `Services/` folder
2. Register it in `App.axaml.cs` if dependency injection is needed
3. Use it in your ViewModels

## Building and Publishing

### Debug Build

```bash
dotnet build DesktopBoilerplate.App
```

### Release Build

```bash
dotnet build DesktopBoilerplate.App -c Release
```

### Publishing for Specific Platforms

#### Windows (x64)
```bash
dotnet publish DesktopBoilerplate.App -c Release -r win-x64 --self-contained false
```

#### macOS (Intel)
```bash
dotnet publish DesktopBoilerplate.App -c Release -r osx-x64 --self-contained true
```

#### macOS (Apple Silicon)
```bash
dotnet publish DesktopBoilerplate.App -c Release -r osx-arm64 --self-contained true
```

#### Linux (x64)
```bash
dotnet publish DesktopBoilerplate.App -c Release -r linux-x64 --self-contained true
```

#### Linux (ARM64)
```bash
dotnet publish DesktopBoilerplate.App -c Release -r linux-arm64 --self-contained true
```

### Self-Contained vs Framework-Dependent

- **Self-contained** (`--self-contained true`): Includes the .NET runtime. Larger size but no runtime installation required.
- **Framework-dependent** (`--self-contained false`): Requires .NET runtime to be installed on the target machine. Smaller size.

Published files will be in: `DesktopBoilerplate.App/bin/Release/net8.0/{runtime-identifier}/publish/`

## Development Workflow

### Recommended Tools

- **Visual Studio 2022** (Windows/macOS) - Full IDE with Avalonia designer
- **JetBrains Rider** (Cross-platform) - Excellent Avalonia support
- **Visual Studio Code** - Lightweight editor with C# extension

### XAML Hot Reload

The application supports XAML Hot Reload for rapid UI development:

```bash
dotnet watch --project DesktopBoilerplate.App
```

Changes to XAML files will automatically refresh the running application.

### Debugging

1. Set breakpoints in your code
2. Run with F5 in your IDE or `dotnet run`
3. Use Avalonia DevTools (press F12 in debug mode) for UI inspection

### Code Style

The project uses:
- C# nullable reference types
- Implicit usings
- Compiled bindings (enabled by default)
- MVVM pattern with CommunityToolkit.Mvvm

## Troubleshooting

### Application Won't Start

1. **Check .NET SDK version:**
   ```bash
   dotnet --version
   ```
   Should be 8.0.x or later.

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet build
   ```

### UI Not Displaying Correctly

1. Check that `Resources/Styles/Colors.axaml` and `Styles.axaml` are properly referenced in `App.axaml`
2. Verify XAML syntax is correct
3. Check for binding errors in the debug output

### Configuration Not Loading

1. Verify `Assets/appsettings.json` exists and is valid JSON
2. Check that the file is marked as an Avalonia resource in the `.csproj` file
3. Review `AppConfigurationLoader.cs` for loading logic

### Platform-Specific Issues

- **macOS**: Ensure you have the latest Xcode Command Line Tools installed
- **Linux**: Install GTK3 development libraries if missing
- **Windows**: Ensure you're using a supported Windows version

## Additional Resources

- [Avalonia UI Documentation](https://docs.avaloniaui.net/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)

## License

MIT License - See LICENSE file for details
