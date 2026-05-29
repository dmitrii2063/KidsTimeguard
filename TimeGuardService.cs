namespace KidsComputerTimeGuard;

public sealed class TimeGuardService : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private AppSettings _settings;
    private bool _actionTakenThisCycle;
    private readonly object _lock = new();

    public TimeGuardService()
    {
        _settings = SettingsService.Load();
        _timer = new System.Windows.Forms.Timer { Interval = 30_000 };
        _timer.Tick += OnTick;
    }

    public void Start()
    {
        OnTick(null, EventArgs.Empty);
        _timer.Start();
    }

    public void Stop() => _timer.Stop();

    public void ReloadSettings()
    {
        lock (_lock)
        {
            _settings = SettingsService.Load();
            _actionTakenThisCycle = false;
        }
    }

    private void OnTick(object? sender, EventArgs e)
    {
        AppSettings settings;
        lock (_lock)
            settings = _settings;

        if (!settings.ChildModeEnabled)
            return;

        var now = DateTime.Now.TimeOfDay;

        if (!IsInBlockedPeriod(now, settings.ShutdownTime, settings.UnlockTime))
        {
            _actionTakenThisCycle = false;
            return;
        }

        if (ShouldEnforceNow(now, settings))
            EnforceBlock(settings);
    }

    public static bool IsInBlockedPeriod(TimeSpan now, TimeSpan shutdown, TimeSpan unlock)
    {
        if (shutdown > unlock)
            return now >= shutdown || now < unlock;

        return now >= shutdown && now < unlock;
    }

    private static bool ShouldEnforceNow(TimeSpan now, AppSettings settings)
    {
        if (now < settings.UnlockTime)
            return true;

        if (settings.ShutdownTime > settings.UnlockTime && now >= settings.ShutdownTime)
            return true;

        return settings.ShutdownTime <= settings.UnlockTime
               && now >= settings.ShutdownTime
               && now < settings.UnlockTime;
    }

    private void EnforceBlock(AppSettings settings)
    {
        lock (_lock)
        {
            if (_actionTakenThisCycle)
                return;
            _actionTakenThisCycle = true;
        }

        var now = DateTime.Now.TimeOfDay;
        var morningBlock = now < settings.UnlockTime;

        if (morningBlock && settings.UseSleepForMorningBlock)
            PowerManager.Sleep();
        else
            PowerManager.Shutdown();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
