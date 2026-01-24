using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDhcpdDesktop.App.Services;

public class ElevatedFileService
{
    public static bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                                  RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Checks if a file path requires elevated permissions
    /// </summary>
    public static bool RequiresElevation(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return false;

        if (IsUnix)
        {
            // On Unix systems, check if path is in system directories
            return filePath.StartsWith("/etc/") || 
                   filePath.StartsWith("/usr/local/etc/") ||
                   filePath.StartsWith("/var/");
        }
        else if (IsWindows)
        {
            // On Windows, check if path is in ProgramData or system directories
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return filePath.StartsWith(programData, StringComparison.OrdinalIgnoreCase) ||
                   filePath.StartsWith(@"C:\Program Files", StringComparison.OrdinalIgnoreCase) ||
                   filePath.StartsWith(@"C:\Windows", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// Saves a file with elevated permissions if needed
    /// </summary>
    public static async Task<bool> SaveFileWithElevationAsync(string filePath, string content, string? password = null)
    {
        if (!RequiresElevation(filePath))
        {
            // No elevation needed, save normally
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        if (IsUnix)
        {
            return await SaveFileWithSudoAsync(filePath, content, password);
        }
        else if (IsWindows)
        {
            // On Windows, we'd need to use UAC elevation
            // For now, try to save and let the OS handle it
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                await File.WriteAllTextAsync(filePath, content);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // Would need UAC elevation here
                return false;
            }
        }

        return false;
    }

    private static async Task<bool> SaveFileWithSudoAsync(string filePath, string content, string? password)
    {
        try
        {
            // Create a temporary file first
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            await File.WriteAllTextAsync(tempFile, content);

            // Use sudo to copy the temp file to the destination
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/sudo",
                Arguments = $"-S cp \"{tempFile}\" \"{filePath}\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            // Send password to sudo if provided
            if (!string.IsNullOrEmpty(password))
            {
                await process.StandardInput.WriteLineAsync(password);
                await process.StandardInput.FlushAsync();
                process.StandardInput.Close();
            }

            await process.WaitForExitAsync();

            // Clean up temp file
            try
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
            catch { }

            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Loads a file, trying elevated access if needed
    /// </summary>
    public static async Task<string?> LoadFileWithElevationAsync(string filePath, string? password = null)
    {
        if (!File.Exists(filePath))
            return null;

        if (!RequiresElevation(filePath))
        {
            try
            {
                return await File.ReadAllTextAsync(filePath);
            }
            catch
            {
                return null;
            }
        }

        if (IsUnix)
        {
            return await LoadFileWithSudoAsync(filePath, password);
        }

        // For Windows or other cases, try normal read
        try
        {
            return await File.ReadAllTextAsync(filePath);
        }
        catch
        {
            return null;
        }
    }

    private static async Task<string?> LoadFileWithSudoAsync(string filePath, string? password)
    {
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/sudo",
                Arguments = $"-S cat \"{filePath}\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            // Send password to sudo if provided
            if (!string.IsNullOrEmpty(password))
            {
                await process.StandardInput.WriteLineAsync(password);
                await process.StandardInput.FlushAsync();
                process.StandardInput.Close();
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            return process.ExitCode == 0 ? output : null;
        }
        catch
        {
            return null;
        }
    }
}
