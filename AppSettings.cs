namespace KidsComputerTimeGuard;

public sealed class AppSettings
{
    public bool ChildModeEnabled { get; set; }

    public TimeSpan ShutdownTime { get; set; } = new(22, 0, 0);

    public TimeSpan UnlockTime { get; set; } = new(6, 0, 0);

    public bool UseSleepForMorningBlock { get; set; }

    public bool AutostartConfigured { get; set; }

    public bool AutostartPromptShown { get; set; }

    public string? ParentPasswordHash { get; set; }

    public string? ParentPasswordSalt { get; set; }
}
