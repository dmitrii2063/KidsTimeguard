namespace KidsComputerTimeGuard;

public sealed class SetPasswordForm : Form
{
    private readonly TextBox _txtPassword = new();
    private readonly TextBox _txtConfirm = new();
    private readonly Button _btnOk;
    private readonly Button _btnCancel;

    public string Password => _txtPassword.Text;

    public SetPasswordForm(bool changeMode = false)
    {
        Text = changeMode ? "Сменить пароль" : "Задать пароль родителя";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(360, 190);
        AcceptButton = _btnOk;
        CancelButton = _btnCancel;

        var lblInfo = new Label
        {
            AutoSize = true,
            Location = new Point(16, 12),
            MaximumSize = new Size(328, 0),
            Text = changeMode
                ? "Введите новый пароль (не сообщайте ребёнку):"
                : "Задайте пароль родителя. Без него настройки недоступны:"
        };

        var lbl1 = new Label { AutoSize = true, Location = new Point(16, 52), Text = "Пароль:" };
        _txtPassword.Location = new Point(16, 72);
        _txtPassword.Size = new Size(328, 23);
        _txtPassword.UseSystemPasswordChar = true;

        var lbl2 = new Label { AutoSize = true, Location = new Point(16, 100), Text = "Повтор:" };
        _txtConfirm.Location = new Point(16, 120);
        _txtConfirm.Size = new Size(328, 23);
        _txtConfirm.UseSystemPasswordChar = true;

        _btnOk = new Button
        {
            Text = "Сохранить",
            DialogResult = DialogResult.OK,
            Location = new Point(168, 152),
            Size = new Size(85, 28)
        };
        _btnOk.Click += OnOkClick;

        _btnCancel = new Button
        {
            Text = "Отмена",
            DialogResult = DialogResult.Cancel,
            Location = new Point(259, 152),
            Size = new Size(85, 28)
        };

        Controls.AddRange([lblInfo, lbl1, _txtPassword, lbl2, _txtConfirm, _btnOk, _btnCancel]);
    }

    private void OnOkClick(object? sender, EventArgs e)
    {
        if (_txtPassword.Text.Length < 4)
        {
            MessageBox.Show("Пароль должен быть не короче 4 символов.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
            return;
        }

        if (_txtPassword.Text != _txtConfirm.Text)
        {
            MessageBox.Show("Пароли не совпадают.", Text,
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult = DialogResult.None;
        }
    }
}
