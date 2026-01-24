# Simple DHCP Daemon Desktop Application Documentation

## Overview

Simple DHCP Daemon Desktop Application is a cross-platform desktop configuration tool for the [Simple DHCP Daemon](https://github.com/simpledaemons/simple-dhcpd) built with [Avalonia UI](https://avaloniaui.net/) and .NET 8. It provides a modern, intuitive graphical interface for configuring all aspects of the DHCP server on Windows, macOS (Intel + Apple Silicon), and Linux.

The application features a clean, modern UI with comprehensive configuration management for:
- Server settings and listen addresses
- Subnet configuration with IP ranges, reservations, and exclusions
- Global DHCP options
- Security settings (snooping, filtering, rate limiting, authentication)
- Performance and lease database configuration
- Logging configuration

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Building and Publishing](#building-and-publishing)
- [Development Workflow](#development-workflow)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before running this application, ensure you have the following installed:

1. **.NET 8 SDK** or later
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version` (should show 8.0.x or later)

2. **Simple DHCP Daemon** (optional, for testing)
   - The desktop app can configure the daemon even if it's not currently running
   - See [Simple DHCP Daemon repository](https://github.com/simpledaemons/simple-dhcpd) for installation

3. **Platform-specific requirements:**
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

## Installation

1. **Clone or navigate to the project directory:**
   ```bash
   git clone https://github.com/simpledaemons/simple-dhcpd-desktop.git
   cd simple-dhcpd-desktop
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
dotnet run --project SimpleDhcpdDesktop.App
```

### Development with Hot Reload

For a better development experience with XAML Hot Reload:

```bash
dotnet watch --project SimpleDhcpdDesktop.App
```

This will automatically rebuild and restart the application when you make changes to the code.

### Running from Visual Studio / Rider

1. Open `DesktopBoilerplate.sln` (or the solution file) in your IDE
2. Set `SimpleDhcpdDesktop.App` as the startup project
3. Press F5 or click Run

## Architecture

### MVVM Pattern

The application follows the Model-View-ViewModel (MVVM) pattern using `CommunityToolkit.Mvvm`:

- **Models**: Data structures (`DhcpConfiguration`, `SubnetConfiguration`, `SecurityConfiguration`, etc.)
- **Views**: XAML files (`MainWindow.axaml`, `ServerSettingsView.axaml`, `SubnetsView.axaml`, etc.)
- **ViewModels**: Business logic and data binding (`MainConfigurationViewModel`, `ServerSettingsViewModel`, etc.)

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
- Dynamic view switching based on navigation selection
- Modern UI with consistent styling and spacing

#### Configuration Service (`DhcpConfigurationService.cs`)
- Loads and saves DHCP configuration files
- Handles platform-specific default paths
- Supports elevated permissions for system directories

#### Elevated File Service (`ElevatedFileService.cs`)
- Detects when files require administrator privileges
- Handles sudo password prompts on Unix systems
- Cross-platform support for Windows (UAC) and Unix (sudo)

#### ViewModels
- `MainConfigurationViewModel`: Coordinates all configuration sections
- `ServerSettingsViewModel`: Manages listen addresses
- `SubnetsViewModel`: Handles subnet configuration
- `GlobalOptionsViewModel`: Manages global DHCP options
- `SecurityViewModel`: Handles all security settings
- `PerformanceViewModel`: Performance configuration
- `LoggingViewModel`: Logging configuration

## Project Structure

```
SimpleDhcpdDesktop.App/
├── App.axaml                    # Application-level XAML resources
├── App.axaml.cs                # Application initialization logic
├── Program.cs                   # Application entry point
├── MainWindow.axaml            # Main window layout
├── MainWindow.axaml.cs         # Main window code-behind
├── Assets/
│   └── appsettings.json        # Application metadata
├── Models/
│   ├── AppMetadata.cs          # Application metadata model
│   ├── NavigationItem.cs       # Navigation item model
│   └── DhcpConfiguration.cs    # DHCP configuration models
├── Services/
│   ├── AppConfigurationLoader.cs  # Application config loading
│   ├── DhcpConfigurationService.cs  # DHCP config file I/O
│   └── ElevatedFileService.cs  # Elevated permissions handling
├── ViewModels/
│   ├── ShellViewModel.cs       # Main shell view model
│   ├── MainConfigurationViewModel.cs  # Main config coordinator
│   ├── ServerSettingsViewModel.cs
│   ├── SubnetsViewModel.cs
│   ├── GlobalOptionsViewModel.cs
│   ├── SecurityViewModel.cs
│   ├── PerformanceViewModel.cs
│   └── LoggingViewModel.cs
├── Views/
│   ├── OverviewView.axaml
│   ├── ServerSettingsView.axaml
│   ├── SubnetsView.axaml
│   ├── GlobalOptionsView.axaml
│   ├── SecurityView.axaml
│   ├── PerformanceView.axaml
│   ├── LoggingView.axaml
│   └── PasswordDialog.axaml
├── Converters/
│   ├── ValueConverters.cs      # Value converters
│   └── NavigationContentConverter.cs  # View switching
└── Resources/
    └── Styles/
        ├── Colors.axaml         # Color definitions
        └── Styles.axaml        # Global styles
```

## Configuration

### Application Settings

Edit `Assets/appsettings.json` to customize application metadata:

```json
{
  "App": {
    "Name": "Simple DHCP - Desktop App",
    "Company": "SimpleDaemons",
    "Description": "Simple DHCP - Desktop Application",
    "RepositoryUrl": "https://github.com/simpledaemons/simple-dhcpd-desktop",
    "DocumentationUrl": "https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md",
    "SupportEmail": "support@simpledaemons.com"
  }
}
```

### DHCP Configuration Files

The application automatically detects the default configuration file location:

- **Windows**: `C:\ProgramData\Simple DHCP Daemon\simple-dhcpd.conf`
- **Linux**: `/etc/simple-dhcpd/simple-dhcpd.conf`
- **macOS**: `/usr/local/etc/simple-dhcpd/simple-dhcpd.conf`

When saving to system directories, the application will prompt for administrator/sudo password.

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

## Building and Publishing

### Debug Build

```bash
dotnet build SimpleDhcpdDesktop.App
```

### Release Build

```bash
dotnet build SimpleDhcpdDesktop.App -c Release
```

### Publishing for Specific Platforms

#### Windows (x64)
```bash
dotnet publish SimpleDhcpdDesktop.App -c Release -r win-x64 --self-contained false
```

#### macOS (Intel)
```bash
dotnet publish SimpleDhcpdDesktop.App -c Release -r osx-x64 --self-contained true
```

#### macOS (Apple Silicon)
```bash
dotnet publish SimpleDhcpdDesktop.App -c Release -r osx-arm64 --self-contained true
```

#### Linux (x64)
```bash
dotnet publish SimpleDhcpdDesktop.App -c Release -r linux-x64 --self-contained true
```

#### Linux (ARM64)
```bash
dotnet publish SimpleDhcpdDesktop.App -c Release -r linux-arm64 --self-contained true
```

### Self-Contained vs Framework-Dependent

- **Self-contained** (`--self-contained true`): Includes the .NET runtime. Larger size but no runtime installation required.
- **Framework-dependent** (`--self-contained false`): Requires .NET runtime to be installed on the target machine. Smaller size.

Published files will be in: `SimpleDhcpdDesktop.App/bin/Release/net8.0/{runtime-identifier}/publish/`

## Development Workflow

### Recommended Tools

- **Visual Studio 2022** (Windows/macOS) - Full IDE with Avalonia designer
- **JetBrains Rider** (Cross-platform) - Excellent Avalonia support
- **Visual Studio Code** - Lightweight editor with C# extension

### XAML Hot Reload

The application supports XAML Hot Reload for rapid UI development:

```bash
dotnet watch --project SimpleDhcpdDesktop.App
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

### Sudo Password Prompt Issues

1. Ensure you have sudo access on your system
2. Check that the file path requires elevation (system directories like `/etc/`)
3. Verify the password dialog is appearing correctly

### Platform-Specific Issues

- **macOS**: Ensure you have the latest Xcode Command Line Tools installed
- **Linux**: Install GTK3 development libraries if missing
- **Windows**: Ensure you're using a supported Windows version

## Additional Resources

- [Simple DHCP Daemon](https://github.com/simpledaemons/simple-dhcpd) - The DHCP server this app configures
- [Avalonia UI Documentation](https://docs.avaloniaui.net/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)

## Support

- **Repository**: [https://github.com/simpledaemons/simple-dhcpd-desktop](https://github.com/simpledaemons/simple-dhcpd-desktop)
- **Documentation**: [https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md](https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md)
- **Support Email**: support@simpledaemons.com
- **Issues**: [GitHub Issues](https://github.com/simpledaemons/simple-dhcpd-desktop/issues)

## License

Apache License 2.0 - See LICENSE file for details
