# Quick Start Guide

## Prerequisites Check

First, verify you have .NET SDK installed:

```bash
dotnet --version
```

If the command is not found, install .NET 8 SDK:
- **macOS**: `brew install dotnet` or download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Windows**: Download installer from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
- **Linux**: Follow distribution-specific instructions at [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

## Running the Application

### Option 1: Standard Run
```bash
cd DesktopBoilerplate.App
dotnet run
```

### Option 2: From Project Root
```bash
dotnet run --project DesktopBoilerplate.App
```

### Option 3: With Hot Reload (Recommended for Development)
```bash
dotnet watch --project DesktopBoilerplate.App
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
   dotnet run --project DesktopBoilerplate.App
   ```

## What to Expect

When you run the application, you should see:
- A window with a dark theme
- A header showing the app name and version
- A navigation sidebar on the left with items: Overview, Library, Activity, Settings
- A main content area with placeholder cards
- Buttons in the header for Docs, Source, and Support

## Troubleshooting

**"dotnet: command not found"**
- Install .NET 8 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)

**Build errors**
- Run `dotnet restore` to restore NuGet packages
- Run `dotnet clean` then `dotnet build` to rebuild

**Application won't start**
- Check that you're in the correct directory
- Verify .NET SDK version: `dotnet --version` (should be 8.0.x or later)

For more detailed information, see [DOCUMENTATION.md](./DOCUMENTATION.md)
