using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;

namespace KidsComputerTimeGuard;

public static class SettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "KidsComputerTimeGuard");

    public static string SettingsFilePath => Path.Combine(SettingsDirectory, "appsettings.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsFilePath))
                return CreateDefaultAndSave();

            var json = File.ReadAllText(SettingsFilePath);
            var dto = JsonSerializer.Deserialize<SettingsDto>(json, JsonOptions);
            return dto?.ToAppSettings() ?? CreateDefaultAndSave();
        }
        catch
        {
            return new AppSettings();
        }
    }

    public static void Save(AppSettings settings)
    {
        Directory.CreateDirectory(SettingsDirectory);
        var dto = SettingsDto.FromAppSettings(settings);
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        var tempPath = SettingsFilePath + ".tmp";
        File.WriteAllText(tempPath, json);
        if (File.Exists(SettingsFilePath))
            File.Replace(tempPath, SettingsFilePath, null);
        else
            File.Move(tempPath, SettingsFilePath);

        ApplyProtectiveAcl(SettingsFilePath);
        ApplyProtectiveAcl(SettingsDirectory);
    }

    private static AppSettings CreateDefaultAndSave()
    {
        var defaults = new AppSettings();
        Save(defaults);
        return defaults;
    }

    private static void ApplyProtectiveAcl(string path)
    {
        try
        {
            var isDirectory = Directory.Exists(path) && !File.Exists(path);

            FileSystemSecurity security = isDirectory
                ? new DirectorySecurity(path, AccessControlSections.All)
                : new FileSecurity(path, AccessControlSections.All);

            security.SetAccessRuleProtection(true, false);

            foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier))
                         .Cast<FileSystemAccessRule>()
                         .ToArray())
            {
                security.RemoveAccessRule(rule);
            }

            var adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
            var systemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
            var usersSid = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            security.AddAccessRule(new FileSystemAccessRule(
                systemSid, FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(
                adminSid, FileSystemRights.FullControl, AccessControlType.Allow));
            security.AddAccessRule(new FileSystemAccessRule(
                usersSid,
                FileSystemRights.Read | FileSystemRights.ReadAndExecute | FileSystemRights.Synchronize,
                isDirectory ? InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit : InheritanceFlags.None,
                PropagationFlags.None,
                AccessControlType.Allow));

            if (isDirectory)
                new DirectoryInfo(path).SetAccessControl((DirectorySecurity)security);
            else
                new FileInfo(path).SetAccessControl((FileSecurity)security);
        }
        catch
        {
            // Требуются права администратора
        }
    }

    private sealed class SettingsDto
    {
        public bool ChildModeEnabled { get; set; }
        public string ShutdownTime { get; set; } = "22:00";
        public string UnlockTime { get; set; } = "06:00";
        public bool UseSleepForMorningBlock { get; set; }
        public bool AutostartConfigured { get; set; }
        public bool AutostartPromptShown { get; set; }
        public string? ParentPasswordHash { get; set; }
        public string? ParentPasswordSalt { get; set; }

        public static SettingsDto FromAppSettings(AppSettings s) => new()
        {
            ChildModeEnabled = s.ChildModeEnabled,
            ShutdownTime = s.ShutdownTime.ToString(@"hh\:mm"),
            UnlockTime = s.UnlockTime.ToString(@"hh\:mm"),
            UseSleepForMorningBlock = s.UseSleepForMorningBlock,
            AutostartConfigured = s.AutostartConfigured,
            AutostartPromptShown = s.AutostartPromptShown,
            ParentPasswordHash = s.ParentPasswordHash,
            ParentPasswordSalt = s.ParentPasswordSalt
        };

        public AppSettings ToAppSettings() => new()
        {
            ChildModeEnabled = ChildModeEnabled,
            ShutdownTime = TimeSpan.TryParse(ShutdownTime, out var st) ? st : new TimeSpan(22, 0, 0),
            UnlockTime = TimeSpan.TryParse(UnlockTime, out var ut) ? ut : new TimeSpan(6, 0, 0),
            UseSleepForMorningBlock = UseSleepForMorningBlock,
            AutostartConfigured = AutostartConfigured,
            AutostartPromptShown = AutostartPromptShown,
            ParentPasswordHash = ParentPasswordHash,
            ParentPasswordSalt = ParentPasswordSalt
        };
    }
}
