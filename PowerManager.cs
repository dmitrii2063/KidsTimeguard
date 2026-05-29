using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KidsComputerTimeGuard;

public static class PowerManager
{
    [DllImport("powrprof.dll", SetLastError = true)]
    private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

    public static void Shutdown()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "shutdown",
            Arguments = "/s /t 0",
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }

    public static void Sleep()
    {
        if (!SetSuspendState(false, true, false))
            Shutdown();
    }
}
