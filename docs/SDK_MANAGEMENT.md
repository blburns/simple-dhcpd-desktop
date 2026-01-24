# .NET SDK Version Management Guide

## Overview

When you have multiple .NET SDK versions installed, you can control which version is used for each project. This guide explains how to manage SDK versions.

## Checking Installed SDKs

### List All Installed SDKs

```bash
dotnet --list-sdks
```

Example output:
```
8.0.100 [/usr/local/share/dotnet/sdk]
8.0.200 [/usr/local/share/dotnet/sdk]
9.0.100 [/usr/local/share/dotnet/sdk]
10.0.102 [/usr/local/share/dotnet/sdk]
```

### Check Current Active SDK

```bash
dotnet --version
```

This shows which SDK version will be used by default (usually the highest installed version).

## Using global.json to Pin SDK Version

The `global.json` file in the project root controls which SDK version is used for this specific project.

### Current Configuration

This project uses:
```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMinor"
  }
}
```

### Understanding rollForward Options

- **`latestMinor`** (recommended): Uses the latest minor version available
  - Example: If you specify `8.0.0` and have `8.0.100` installed, it will use `8.0.100`
  - Allows flexibility while maintaining compatibility

- **`latestPatch`**: Uses the latest patch version within the same minor version
  - More restrictive than `latestMinor`

- **`latestMajor`**: Uses the latest major version available
  - Example: If you specify `8.0.0` and have `9.0.0` installed, it will use `9.0.0`
  - Use with caution as major versions may have breaking changes

- **`disable`**: Uses only the exact version specified
  - Most restrictive - will fail if exact version is not installed

- **`minor`**: Uses the latest minor version, but won't roll forward to a new major version
  - Example: `8.0.0` will use `8.x.x` but not `9.0.0`

### Modifying global.json

To change the SDK version requirement:

1. Edit `global.json` in the project root
2. Update the `version` field to your desired minimum version
3. Adjust `rollForward` based on your needs

Example - Require exact .NET 8.0.100:
```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "disable"
  }
}
```

Example - Allow any .NET 8.x SDK:
```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMinor"
  }
}
```

## Verifying SDK Selection

After setting up `global.json`, verify it's working:

```bash
# From the project root
dotnet --version
```

This should show the SDK version that matches your `global.json` requirements.

## Installing Additional SDK Versions

### macOS (Homebrew)

```bash
# Install .NET 8 SDK
brew install dotnet@8

# Install .NET 9 SDK
brew install dotnet@9

# Install latest SDK
brew install dotnet
```

### Windows

Download installers from: https://dotnet.microsoft.com/download

Multiple SDK versions can coexist on Windows.

### Linux

Use distribution-specific package managers or download from Microsoft.

## Troubleshooting

### "No .NET SDKs were found matching the version specified in global.json"

**Solution**: Install the required SDK version or adjust `rollForward` policy.

```bash
# Check what you have
dotnet --list-sdks

# Either install the required version, or modify global.json
```

### macOS: Only One SDK Version Shows Up Despite Multiple Installations

**Problem**: You installed SDKs via both Homebrew and manual .pkg files, but `dotnet --list-sdks` only shows one version.

**Solution**: The `dotnet` command is likely pointing to the Homebrew installation. See [FIX_SDK_DETECTION.md](./FIX_SDK_DETECTION.md) for detailed instructions.

Quick fix:
```bash
# Check where dotnet points
which dotnet
ls -la $(which dotnet)

# Check if SDKs exist in the manual installation location
ls -la /usr/local/share/dotnet/sdk

# Update symlink to use manual installation (has all SDKs)
sudo rm /usr/local/bin/dotnet
sudo ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/dotnet
```

### Wrong SDK Version Being Used

**Solution**: Verify `global.json` is in the project root and has correct syntax.

```bash
# Check current directory
pwd

# Verify global.json exists
cat global.json

# Check which SDK will be used
dotnet --version
```

### Multiple Projects Need Different SDK Versions

**Solution**: Each project can have its own `global.json` file. The SDK selection is project-specific.

## Best Practices

1. **Use `latestMinor` rollForward**: Provides flexibility while maintaining compatibility
2. **Pin minimum version**: Specify the minimum SDK version your project requires
3. **Document requirements**: Include SDK version requirements in your README
4. **CI/CD consistency**: Use `global.json` to ensure CI/CD uses the same SDK version

## Example Workflows

### Scenario 1: Project requires .NET 8.0.100 or later

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestMinor"
  }
}
```

### Scenario 2: Project works with any .NET 8.x SDK

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMinor"
  }
}
```

### Scenario 3: Project requires exact SDK version (not recommended)

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "disable"
  }
}
```

## Additional Resources

- [.NET SDK Versioning](https://learn.microsoft.com/dotnet/core/tools/global-json)
- [global.json Schema](https://learn.microsoft.com/dotnet/core/tools/global-json#sdk-properties)
- [.NET Download Page](https://dotnet.microsoft.com/download)
