# Simple DHCP Daemon Desktop Configuration UI

## Overview

This document summarizes the desktop UI application for configuring the simple-dhcpd daemon. The application is built with Avalonia UI and .NET 8, providing a cross-platform configuration interface for macOS, Linux, and Windows.

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
- `GetDefaultConfigPath()` - Returns platform-specific default config path

### ViewModels
- `MainConfigurationViewModel` - Main coordinator for all configuration sections
- `ServerSettingsViewModel` - Server listen addresses
- `SubnetsViewModel` - Subnet management with nested ViewModels
- `GlobalOptionsViewModel` - Global DHCP options
- `SecurityViewModel` - Security settings with nested rule ViewModels
- `PerformanceViewModel` - Performance settings
- `LoggingViewModel` - Logging settings

### Navigation
The `ShellViewModel` has been updated with navigation items for:
- Overview
- Server Settings
- Subnets
- Global Options
- Security
- Performance
- Logging

## Current Status

### Completed
✅ Configuration models matching JSON schema
✅ Configuration service for reading/writing JSON files
✅ ViewModels for all configuration sections
✅ Navigation structure
✅ Main window integration
✅ Cross-platform support (macOS, Linux, Windows)

### Remaining Work

1. **Fix SetProperty Calls**: Some ViewModels still use incorrect SetProperty syntax. These need to be updated to use the proper pattern:
   ```csharp
   set
   {
       if (_field != value)
       {
           _field = value;
           OnPropertyChanged();
       }
   }
   ```

2. **Create Views**: Create actual UI views for each configuration section:
   - ServerSettingsView.axaml
   - SubnetsView.axaml
   - GlobalOptionsView.axaml
   - SecurityView.axaml
   - PerformanceView.axaml
   - LoggingView.axaml

3. **Update MainWindow**: Implement proper content switching based on selected navigation item

4. **Add Validation**: Add input validation for IP addresses, MAC addresses, ranges, etc.

5. **Error Handling**: Add proper error handling for file I/O operations

## Configuration File Locations

- **Windows**: `C:\ProgramData\Simple DHCP Daemon\simple-dhcpd.conf`
- **Linux**: `/etc/simple-dhcpd/simple-dhcpd.conf`
- **macOS**: `/usr/local/etc/simple-dhcpd/simple-dhcpd.conf`
- **Fallback**: User's application data directory

## Usage

1. The application loads the default configuration file on startup
2. Navigate between sections using the left sidebar
3. Edit configuration values in each section
4. Save changes using the Save button
5. Load a different configuration file using the Load button

## Next Steps

1. Fix remaining SetProperty calls in SecurityViewModel and helper ViewModels
2. Create UI views for each configuration section
3. Implement proper content switching in MainWindow
4. Add validation and error handling
5. Test on all three platforms (macOS, Linux, Windows)
