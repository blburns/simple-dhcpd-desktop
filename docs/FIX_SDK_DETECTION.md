# Fix: .NET SDK Detection Issue on macOS

## Problem

You have multiple .NET SDK versions installed:
- **Homebrew installation**: `/usr/local/Cellar/dotnet/10.0.102/` (only 10.0.102)
- **Manual .pkg installations**: `/usr/local/share/dotnet/sdk/` (8.0.417, 9.0.310, 10.0.102)

But `dotnet --list-sdks` only shows 10.0.102 because the `dotnet` command is pointing to the Homebrew installation.

## Solution Options

### Option 1: Update the Symlink (Recommended)

This makes the manual installation the default, which has all your SDKs:

```bash
# Remove the Homebrew symlink
sudo rm /usr/local/bin/dotnet

# Create symlink to the manual installation
sudo ln -s /usr/local/share/dotnet/dotnet /usr/local/bin/dotnet

# Verify it works
dotnet --list-sdks
```

You should now see:
```
8.0.417 [/usr/local/share/dotnet/sdk]
9.0.310 [/usr/local/share/dotnet/sdk]
10.0.102 [/usr/local/share/dotnet/sdk]
```

### Option 2: Use Environment Variable (No sudo required)

Add to your `~/.zshrc` (or `~/.bash_profile` if using bash):

```bash
# Add this line to your shell profile
export DOTNET_ROOT=/usr/local/share/dotnet
export PATH=$DOTNET_ROOT:$PATH
```

Then reload your shell:
```bash
source ~/.zshrc  # or source ~/.bash_profile
```

### Option 3: Use DOTNET_ROOT Per Project

You can set `DOTNET_ROOT` just for this project by creating a `.env` file or setting it in your shell:

```bash
export DOTNET_ROOT=/usr/local/share/dotnet
dotnet --list-sdks
```

## Verify the Fix

After applying either solution:

```bash
# Should show all installed SDKs
dotnet --list-sdks

# Should use the correct installation
dotnet --version
```

## Why This Happened

- Homebrew installs .NET in `/usr/local/Cellar/dotnet/`
- Manual .pkg installers put .NET in `/usr/local/share/dotnet/`
- The `/usr/local/bin/dotnet` symlink was pointing to Homebrew's installation
- Homebrew's installation only has the version installed via Homebrew (10.0.102)
- Your manual installations are in a different location

## Recommendation

**Use Option 1** (update the symlink) if you want the manual installation to be the default. This is recommended because:
- Manual .pkg installs typically have all SDK versions
- It's a one-time fix
- Works system-wide

**Use Option 2** (environment variable) if you want to keep Homebrew managing the symlink but override it with an environment variable.

## After Fixing

Once fixed, your `global.json` will work correctly:

```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMajor"
  }
}
```

The project will now be able to use any of your installed SDKs (8.0.417, 9.0.310, or 10.0.102) based on the `rollForward` policy.
