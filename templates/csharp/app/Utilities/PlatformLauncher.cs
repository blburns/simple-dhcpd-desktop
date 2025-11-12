using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AvaloniaAppTemplate.Utilities;

public static class PlatformLauncher
{
    public static Task OpenUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Task.CompletedTask;
        }

        try
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? StartProcessAsync(new ProcessStartInfo("cmd", $"/c start \"\" \"{url}\"") { CreateNoWindow = true })
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? StartProcessAsync(new ProcessStartInfo("open", url))
                    : StartProcessAsync(new ProcessStartInfo("xdg-open", url));
        }
        catch (Exception)
        {
            return Task.CompletedTask;
        }
    }

    private static Task StartProcessAsync(ProcessStartInfo processStartInfo)
    {
        processStartInfo.UseShellExecute = false;
        Process.Start(processStartInfo);
        return Task.CompletedTask;
    }
}

