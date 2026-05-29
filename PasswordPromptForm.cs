namespace KidsComputerTimeGuard;

public sealed class PasswordPromptForm : Form
{
    private readonly TextBox _txtPassword = new();
    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public string Password => _txtPassword.Text;

    public PasswordPromptForm()
    {
        Text = "Пароль родителя";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(360, 140);

        var lbl = new Label
        {
            AutoSize = true,
            Location = new Point(16, 16),
            Text = "Введите пароль для доступа к настройкам:"
        };

        _txtPassword.Location = new Point(16, 44);
        _txtPassword.Size = new Size(328, 23);
        _txtPassword.UseSystemPasswordChar = true;

        _btnOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(188, 88),
            Size = new Size(75, 28)
        };
        _btnOk.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_txtPassword.Text))
            {
                MessageBox.Show("Введите пароль.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        };

        _btnCancel = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Location = new Point(269, 88),
            Size = new Size(75, 28)
        };

        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        Controls.AddRange([lbl, _txtPassword, _btnOk, _btnCancel]);
    }
}
