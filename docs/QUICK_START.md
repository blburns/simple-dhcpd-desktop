# Quick Start Guide

## Prerequisites Check

First, verify you have .NET SDK installed:

```bash
dotnet --version
```

If the command is not found, install .NET 9 SDK version 9.0.310:
- **macOS**: `brew install dotnet` or download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Windows**: Download installer from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Linux**: Follow distribution-specific instructions at [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

## Running the Application

### Option 1: Standard Run
```bash
cd SimpleDhcpdDesktop.App
dotnet run
```

### Option 2: From Project Root
```bash
dotnet run --project SimpleDhcpdDesktop.App
```

### Option 3: With Hot Reload (Recommended for Development)
```bash
dotnet watch --project SimpleDhcpdDesktop.App
```

## First Time Setup

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build the project:**
   ```bash
   dotnet build
   ```

3. **Run the application:**
   ```bash
   dotnet run --project SimpleDhcpdDesktop.App
   ```

## What to Expect

When you run the application, you should see:
- A window with a dark theme
- A header showing "Simple DHCP Daemon Configuration" with Save and Load buttons
- A navigation sidebar on the left with sections:
  - Overview
  - Server Settings
  - Subnets
  - Global Options
  - Security
  - Performance
  - Logging
- A main content area that changes based on the selected navigation item
- Configuration file path displayed in the Overview section

## Configuration

The application automatically loads the default configuration file based on your platform:
- **Windows**: `C:\ProgramData\Simple DHCP Daemon\simple-dhcpd.conf`
- **Linux**: `/etc/simple-dhcpd/simple-dhcpd.conf`
- **macOS**: `/usr/local/etc/simple-dhcpd/simple-dhcpd.conf`

When saving to system directories, you'll be prompted for your administrator/sudo password.

## Troubleshooting

**"dotnet: command not found"**
- Install .NET 9 SDK version 9.0.310 from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

**Build errors**
- Run `dotnet restore` to restore NuGet packages
- Run `dotnet clean` then `dotnet build` to rebuild

**Application won't start**
- Check that you're in the correct directory
- Verify .NET SDK version: `dotnet --version` (should be 9.0.310 or later)

**Sudo password prompt not appearing**
- Ensure you're saving to a system directory (like `/etc/`)
- Check that you have sudo access configured

For more detailed information, see [DOCUMENTATION.md](./DOCUMENTATION.md)
