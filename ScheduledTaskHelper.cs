using System.Diagnostics;
using System.Security;
using System.Text;
using Microsoft.Win32;

namespace KidsComputerTimeGuard;

public static class ScheduledTaskHelper
{
    public const string TaskFolder = "KidsComputerTimeGuard";
    public const string TaskName = "ChildMode";
    public static string FullTaskName => $"{TaskFolder}\\{TaskName}";

    public static bool IsConfigured()
    {
        try
        {
            using var process = RunSchtasks($"/Query /TN \"{FullTaskName}\"");
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public static (bool Success, string? Error) TryCreateLogonTask(string exePath)
    {
        RemoveLegacyRegistryAutostart();

        var xmlPath = Path.Combine(Path.GetTempPath(), "KidsComputerTimeGuard_task.xml");
        try
        {
            File.WriteAllText(xmlPath, BuildLogonTaskXml(exePath), Encoding.Unicode);
            using var process = RunSchtasks($"/Create /TN \"{FullTaskName}\" /XML \"{xmlPath}\" /F");
            if (process.ExitCode == 0)
                return (true, null);

            return (false, GetLastErrorMessage(process));
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
        finally
        {
            try { File.Delete(xmlPath); } catch { /* ignore */ }
        }
    }

    public static void Disable()
    {
        RemoveLegacyRegistryAutostart();
        try { RunSchtasks($"/Delete /TN \"{FullTaskName}\" /F"); } catch { /* ignore */ }
        try { RunSchtasks("/Delete /TN \"KidsComputerTimeGuard\" /F"); } catch { /* ignore */ }
    }

    public static string? GetLastErrorMessage(Process process)
    {
        var err = process.StandardError.ReadToEnd().Trim();
        var outp = process.StandardOutput.ReadToEnd().Trim();
        return string.IsNullOrEmpty(err) ? outp : err;
    }

    private static Process RunSchtasks(string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        process.Start();
        process.WaitForExit();
        return process;
    }

    private static string BuildLogonTaskXml(string exePath)
    {
        var command = SecurityElement.Escape(exePath) ?? exePath;
        return $"""
            <?xml version="1.0" encoding="UTF-16"?>
            <Task version="1.4" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
              <RegistrationInfo>
                <Description>Kids Computer Time Guard — режим ребёнка при входе</Description>
              </RegistrationInfo>
              <Triggers>
                <LogonTrigger>
                  <Enabled>true</Enabled>
                </LogonTrigger>
              </Triggers>
              <Principals>
                <Principal id="Author">
                  <GroupId>S-1-5-32-545</GroupId>
                  <RunLevel>LeastPrivilege</RunLevel>
                </Principal>
              </Principals>
              <Settings>
                <MultipleInstancesPolicy>IgnoreNew</MultipleInstancesPolicy>
                <DisallowStartIfOnBatteries>false</DisallowStartIfOnBatteries>
                <StopIfGoingOnBatteries>false</StopIfGoingOnBatteries>
                <AllowHardTerminate>true</AllowHardTerminate>
                <StartWhenAvailable>true</StartWhenAvailable>
                <Enabled>true</Enabled>
                <Hidden>false</Hidden>
                <ExecutionTimeLimit>PT0S</ExecutionTimeLimit>
              </Settings>
              <Actions Context="Author">
                <Exec>
                  <Command>{command}</Command>
                  <Arguments>--child</Arguments>
                </Exec>
              </Actions>
            </Task>
            """;
    }

    private static void RemoveLegacyRegistryAutostart()
    {
        const string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string valueName = "KidsComputerTimeGuard";

        try
        {
            using var hkcu = Registry.CurrentUser.OpenSubKey(runKey, true);
            hkcu?.DeleteValue(valueName, false);
        }
        catch { /* ignore */ }

        try
        {
            using var hklm = Registry.LocalMachine.OpenSubKey(runKey, true);
            hklm?.DeleteValue(valueName, false);
        }
        catch { /* ignore */ }
    }
}
