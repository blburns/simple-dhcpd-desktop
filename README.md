# Simple DHCP Daemon - Desktop Configuration Application

A cross-platform desktop application for configuring the [Simple DHCP Daemon](https://github.com/simpledaemons/simple-dhcpd) built with [Avalonia UI](https://avaloniaui.net/) and .NET 8.

> 📖 **For comprehensive documentation, see [docs/DOCUMENTATION.md](./docs/DOCUMENTATION.md)**

## Overview

Simple DHCP Daemon Desktop Application provides a user-friendly graphical interface for configuring all aspects of the simple-dhcpd DHCP server, including:

- Server settings and listen addresses
- Subnet configuration with IP ranges, reservations, and exclusions
- Global DHCP options
- Security settings (snooping, filtering, rate limiting, authentication)
- Performance and lease database configuration
- Logging configuration

## Prerequisites

- **.NET 8 SDK** or later ([Download](https://dotnet.microsoft.com/download))
- Verify installation: `dotnet --version` (should show 8.0.x or later)
- **Simple DHCP Daemon** installed on the target system

## Features

- **True cross-platform**: Runs on Windows, macOS (Intel + Apple Silicon), and Linux (x64/ARM64)
- **Complete configuration management**: Configure all DHCP server settings through an intuitive UI
- **Elevated permissions support**: Automatically prompts for sudo/admin password when saving to system directories
- **Real-time validation**: Visual feedback for configuration changes
- **Modern UI**: Clean, pixel-perfect interface with consistent styling

## Quick Start

```bash
# Clone the repository
git clone https://github.com/simpledaemons/simple-dhcpd-desktop.git
cd simple-dhcpd-desktop

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project SimpleDhcpdDesktop.App
```

> 💡 **Tip**: Use `dotnet watch --project SimpleDhcpdDesktop.App` for hot reload during development

## Configuration

The application automatically detects the default configuration file location based on your platform:

- **Windows**: `C:\ProgramData\Simple DHCP Daemon\simple-dhcpd.conf`
- **Linux**: `/etc/simple-dhcpd/simple-dhcpd.conf`
- **macOS**: `/usr/local/etc/simple-dhcpd/simple-dhcpd.conf`

When saving to system directories, the application will prompt for administrator/sudo password.

## Cross-platform Publishing

```bash
# Windows
dotnet publish SimpleDhcpdDesktop.App -c Release -r win-x64 --self-contained false

# macOS (Universal)
dotnet publish SimpleDhcpdDesktop.App -c Release -r osx-x64 --self-contained true
dotnet publish SimpleDhcpdDesktop.App -c Release -r osx-arm64 --self-contained true

# Linux
dotnet publish SimpleDhcpdDesktop.App -c Release -r linux-x64 --self-contained true
dotnet publish SimpleDhcpdDesktop.App -c Release -r linux-arm64 --self-contained true
```

Use `--self-contained true` when you want to ship the runtime; omit (or set false) when targeting environments with the .NET runtime pre-installed.

## Project Structure

- `SimpleDhcpdDesktop.App/`: Main application project
  - `Models/`: Configuration data models
  - `ViewModels/`: MVVM view models for all configuration sections
  - `Views/`: UI views for each configuration section
  - `Services/`: Configuration file I/O and elevated permissions handling
  - `Converters/`: Value converters for data binding
  - `Resources/`: Styles and themes
  - `Assets/`: Application metadata and configuration

## Documentation

For detailed information about:
- Architecture and design patterns
- Configuration options
- Building and publishing
- Troubleshooting

See [docs/DOCUMENTATION.md](./docs/DOCUMENTATION.md)

### Managing Multiple .NET SDK Versions

If you have multiple .NET SDK versions installed, see [docs/SDK_MANAGEMENT.md](./docs/SDK_MANAGEMENT.md) for guidance.

## Contributing

Contributions are welcome! Please see the [SimpleDaemons contributing guidelines](https://github.com/simpledaemons/simple-dhcpd/blob/main/CONTRIBUTING.md).

## License

Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Support

- **Repository**: [https://github.com/simpledaemons/simple-dhcpd-desktop](https://github.com/simpledaemons/simple-dhcpd-desktop)
- **Documentation**: [https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md](https://github.com/simpledaemons/simple-dhcpd-desktop/blob/main/docs/DOCUMENTATION.md)
- **Support Email**: support@simpledaemons.com
- **Issues**: [GitHub Issues](https://github.com/simpledaemons/simple-dhcpd-desktop/issues)

---

**Simple DHCP Daemon Desktop Application** - Modern configuration UI for the Simple DHCP Daemon

Made with ❤️ by [SimpleDaemons](https://github.com/simpledaemons)