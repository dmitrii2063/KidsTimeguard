namespace KidsComputerTimeGuard;

public static class ParentAuth
{
    public static bool TryAuthenticate(IWin32Window? owner = null)
    {
        var settings = SettingsService.Load();

        if (!ParentPasswordService.IsConfigured(settings))
        {
            using var setup = new SetPasswordForm();
            if (setup.ShowDialog(owner) != DialogResult.OK)
                return false;

            ParentPasswordService.SetPassword(settings, setup.Password);
            SettingsService.Save(settings);
            return true;
        }

        for (var attempt = 0; attempt < 3; attempt++)
        {
            using var prompt = new PasswordPromptForm();
            if (prompt.ShowDialog(owner) != DialogResult.OK)
                return false;

            if (ParentPasswordService.Verify(prompt.Password, settings))
                return true;

            MessageBox.Show(owner,
                "Неверный пароль.",
                "Доступ запрещён",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        return false;
    }

    public static void ShowParentSettings(IWin32Window? owner = null)
    {
        if (!TryAuthenticate(owner))
            return;

        using var form = new ParentConfigForm();
        form.ShowDialog(owner);
    }
}
