namespace KidsComputerTimeGuard;

public sealed class ChildModeContext : ApplicationContext
{
    private readonly TimeGuardService _service;
    private readonly NotifyIcon _trayIcon;

    public ChildModeContext()
    {
        _service = new TimeGuardService();
        _service.Start();

        _trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Shield,
            Visible = true,
            Text = "Kids Computer Time Guard"
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Настройки родителя…", null, (_, _) => OpenParentSettings());
        menu.Items.Add("Обновить настройки", null, (_, _) => _service.ReloadSettings());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Выход", null, (_, _) =>
        {
            if (ParentAuth.TryAuthenticate())
                ExitThread();
        });
        _trayIcon.ContextMenuStrip = menu;
        _trayIcon.DoubleClick += (_, _) => OpenParentSettings();

        var timer = new System.Windows.Forms.Timer { Interval = 120_000 };
        timer.Tick += (_, _) => _service.ReloadSettings();
        timer.Start();
    }

    private void OpenParentSettings()
    {
        ParentAuth.ShowParentSettings();
        _service.ReloadSettings();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _service.Dispose();
        }
        base.Dispose(disposing);
    }
}
