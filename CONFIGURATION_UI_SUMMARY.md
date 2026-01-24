# Simple DHCP Daemon Desktop Configuration UI

## Overview

This document summarizes the desktop UI application for configuring the simple-dhcpd daemon. The application is built with Avalonia UI and .NET 8, providing a cross-platform configuration interface for macOS, Linux, and Windows.

**Project**: Simple DHCP Daemon Desktop Application  
**Company**: SimpleDaemons  
**Repository**: [https://github.com/simpledaemons/simple-dhcpd-desktop](https://github.com/simpledaemons/simple-dhcpd-desktop)

## Architecture

### Models (`Models/DhcpConfiguration.cs`)
Complete C# models matching the JSON configuration schema:
- `DhcpConfiguration` - Root configuration object
- `DhcpSection` - Main DHCP configuration section
- `SubnetConfiguration` - Subnet settings with options, reservations, and exclusions
- `SecurityConfiguration` - All security settings (snooping, filtering, rate limiting, etc.)
- `PerformanceConfiguration` - Performance and lease database settings
- `LoggingConfiguration` - Logging configuration

### Services (`Services/DhcpConfigurationService.cs`)
- `LoadConfiguration(string filePath)` - Loads JSON configuration from file
- `SaveConfiguration(DhcpConfiguration config, string filePath)` - Saves configuration to JSON file
- `SaveConfigurationWithElevationAsync()` - Saves with sudo/admin elevation when needed
- `LoadConfigurationWithElevationAsync()` - Loads with sudo/admin elevation when needed
- `GetDefaultConfigPath()` - Returns platform-specific default config path

### Services (`Services/ElevatedFileService.cs`)
- `RequiresElevation(string filePath)` - Checks if a file path requires admin privileges
- `SaveFileWithElevationAsync()` - Saves files using sudo on Unix systems
- `LoadFileWithElevationAsync()` - Loads files using sudo on Unix systems
- Cross-platform support for Windows (UAC) and Unix (sudo)

### ViewModels
- `MainConfigurationViewModel` - Main coordinator for all configuration sections
- `ServerSettingsViewModel` - Server listen addresses
- `SubnetsViewModel` - Subnet management with nested ViewModels
- `GlobalOptionsViewModel` - Global DHCP options
- `SecurityViewModel` - Security settings with nested rule ViewModels
- `PerformanceViewModel` - Performance settings
- `LoggingViewModel` - Logging settings
- `ShellViewModel` - Main shell with navigation

### Views
- `OverviewView` - Dashboard with statistics and quick actions
- `ServerSettingsView` - Listen addresses configuration
- `SubnetsView` - Two-panel subnet editor (list + configuration)
- `GlobalOptionsView` - Global DHCP options management
- `SecurityView` - Security settings editor
- `PerformanceView` - Performance configuration
- `LoggingView` - Logging settings
- `PasswordDialog` - Secure password input for sudo/admin operations

### Navigation
The `ShellViewModel` provides navigation items for:
- Overview
- Server Settings
- Subnets
- Global Options
- Security
- Performance
- Logging

## Current Status

### Completed ✅
- Configuration models matching JSON schema
- Configuration service for reading/writing JSON files
- ViewModels for all configuration sections with proper MVVM patterns
- Complete UI views for all configuration sections
- Navigation structure with dynamic view switching
- Main window with clean, pixel-perfect UI
- Cross-platform support (macOS, Linux, Windows)
- Elevated permissions support with password dialog
- Sudo password prompt for system directory saves
- Namespace refactored from DesktopBoilerplate to SimpleDhcpdDesktop
- SimpleDaemons branding throughout

### Features

1. **Automatic Elevation Detection**: The application automatically detects when a file requires administrator privileges and prompts for password
2. **Secure Password Input**: Password dialog with masked input for sudo/admin operations
3. **Real-time Change Tracking**: Tracks unsaved changes and enables/disables Save button accordingly
4. **Platform-Specific Paths**: Automatically uses correct default config paths for each platform
5. **Comprehensive Configuration**: Supports all DHCP configuration sections from the daemon

## Configuration File Locations

- **Windows**: `C:\ProgramData\Simple DHCP Daemon\simple-dhcpd.conf`
- **Linux**: `/etc/simple-dhcpd/simple-dhcpd.conf`
- **macOS**: `/usr/local/etc/simple-dhcpd/simple-dhcpd.conf`
- **Fallback**: User's application data directory

## Usage

1. The application loads the default configuration file on startup
2. Navigate between sections using the left sidebar
3. Edit configuration values in each section
4. Save changes using the Save button (will prompt for password if saving to system directory)
5. Load a different configuration file using the Load button

## Building and Running

```bash
# Build
dotnet build SimpleDhcpdDesktop.App/SimpleDhcpdDesktop.App.csproj

# Run
dotnet run --project SimpleDhcpdDesktop.App/SimpleDhcpdDesktop.App.csproj

# Run with hot reload
dotnet watch --project SimpleDhcpdDesktop.App/SimpleDhcpdDesktop.App.csproj
```

## Technology Stack

- **.NET 8**: Runtime and framework
- **Avalonia UI 11.3.8**: Cross-platform UI framework
- **CommunityToolkit.Mvvm 8.2.2**: MVVM pattern support
- **System.Text.Json**: JSON serialization

## Support

- **Repository**: [https://github.com/simpledaemons/simple-dhcpd-desktop](https://github.com/simpledaemons/simple-dhcpd-desktop)
- **Documentation**: [https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md](https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md)
- **Support Email**: support@simpledaemons.com
- **Issues**: [GitHub Issues](https://github.com/simpledaemons/simple-dhcpd-desktop/issues)
