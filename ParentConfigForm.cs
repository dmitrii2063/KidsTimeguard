namespace KidsComputerTimeGuard;

public partial class ParentConfigForm : Form
{
    private AppSettings _settings;

    public ParentConfigForm()
    {
        InitializeComponent();
        _settings = SettingsService.Load();
        LoadSettingsToUi();
        Shown += OnFormShown;
    }

    private void OnFormShown(object? sender, EventArgs e)
    {
        if (_settings.AutostartPromptShown)
            return;

        var result = MessageBox.Show(
            "Создать задание в Планировщике заданий Windows?\r\n" +
            "Режим «ребёнка» будет запускаться при каждом входе пользователя.",
            "Автозапуск",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        _settings.AutostartPromptShown = true;

        if (result == DialogResult.Yes)
            EnableScheduledAutostart();

        SettingsService.Save(_settings);
    }

    private void LoadSettingsToUi()
    {
        chkEnabled.Checked = _settings.ChildModeEnabled;
        dtpShutdown.Value = DateTime.Today.Add(_settings.ShutdownTime);
        dtpUnlock.Value = DateTime.Today.Add(_settings.UnlockTime);
        chkSleepMorning.Checked = _settings.UseSleepForMorningBlock;
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        var autostart = ScheduledTaskHelper.IsConfigured() ? "да (Планировщик)" : "нет";
        statusLabel.Text =
            $"Режим: {(_settings.ChildModeEnabled ? "включён" : "выключен")}. " +
            $"Выключение: {_settings.ShutdownTime:hh\\:mm}, разблокировка: {_settings.UnlockTime:hh\\:mm}. " +
            $"Автозапуск: {autostart}.";
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        _settings.ChildModeEnabled = chkEnabled.Checked;
        _settings.ShutdownTime = dtpShutdown.Value.TimeOfDay;
        _settings.UnlockTime = dtpUnlock.Value.TimeOfDay;
        _settings.UseSleepForMorningBlock = chkSleepMorning.Checked;

        if (_settings.ShutdownTime == _settings.UnlockTime)
        {
            MessageBox.Show(
                "Время выключения и разблокировки не должны совпадать.",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        try
        {
            SettingsService.Save(_settings);
            UpdateStatusText();
            MessageBox.Show(
                "Настройки сохранены.\r\n" +
                "Файл настроек защищён: изменять могут только администраторы.",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не удалось сохранить настройки. Запустите от имени администратора.\r\n{ex.Message}",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void btnChangePassword_Click(object sender, EventArgs e)
    {
        using var dlg = new SetPasswordForm(changeMode: true);
        if (dlg.ShowDialog(this) != DialogResult.OK)
            return;

        ParentPasswordService.SetPassword(_settings, dlg.Password);
        SettingsService.Save(_settings);
        MessageBox.Show("Пароль изменён.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void btnAutostart_Click(object sender, EventArgs e) => EnableScheduledAutostart();

    private void EnableScheduledAutostart()
    {
        var (success, error) = ScheduledTaskHelper.TryCreateLogonTask(Application.ExecutablePath);
        if (success)
        {
            _settings.AutostartConfigured = true;
            SettingsService.Save(_settings);
            UpdateStatusText();
            MessageBox.Show(
                "Задание создано в Планировщике заданий.\r\n" +
                $"Имя: {ScheduledTaskHelper.FullTaskName}",
                "Автозапуск",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(
            "Не удалось создать задание в Планировщике заданий.\r\n" +
            "Запустите ярлык «Родитель» от имени администратора.\r\n\r\n" +
            (error ?? "Неизвестная ошибка"),
            "Автозапуск",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private void btnRemoveAutostart_Click(object sender, EventArgs e)
    {
        ScheduledTaskHelper.Disable();
        _settings.AutostartConfigured = false;
        SettingsService.Save(_settings);
        UpdateStatusText();
    }

    private void btnLaunchChild_Click(object sender, EventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Application.ExecutablePath,
                Arguments = "--child",
                UseShellExecute = true
            });
            MessageBox.Show(
                "Режим «ребёнка» запущен отдельным процессом (если ещё не был активен).",
                "Запуск",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
