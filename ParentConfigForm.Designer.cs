namespace KidsComputerTimeGuard;

partial class ParentConfigForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        lblTitle = new Label();
        lblShutdown = new Label();
        dtpShutdown = new DateTimePicker();
        lblUnlock = new Label();
        dtpUnlock = new DateTimePicker();
        chkEnabled = new CheckBox();
        chkSleepMorning = new CheckBox();
        btnApply = new Button();
        statusLabel = new Label();
        btnAutostart = new Button();
        btnRemoveAutostart = new Button();
        btnLaunchChild = new Button();
        btnChangePassword = new Button();
        SuspendLayout();

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTitle.Location = new Point(16, 12);
        lblTitle.Text = "Режим родителя — настройки";

        lblShutdown.AutoSize = true;
        lblShutdown.Location = new Point(16, 52);
        lblShutdown.Text = "Время выключения (вечер):";

        dtpShutdown.Format = DateTimePickerFormat.Time;
        dtpShutdown.ShowUpDown = true;
        dtpShutdown.Location = new Point(220, 48);
        dtpShutdown.Size = new Size(100, 23);
        dtpShutdown.Value = new DateTime(2024, 1, 1, 22, 0, 0);

        lblUnlock.AutoSize = true;
        lblUnlock.Location = new Point(16, 88);
        lblUnlock.Text = "Время разблокировки (утро):";

        dtpUnlock.Format = DateTimePickerFormat.Time;
        dtpUnlock.ShowUpDown = true;
        dtpUnlock.Location = new Point(220, 84);
        dtpUnlock.Size = new Size(100, 23);
        dtpUnlock.Value = new DateTime(2024, 1, 1, 6, 0, 0);

        chkEnabled.AutoSize = true;
        chkEnabled.Location = new Point(16, 124);
        chkEnabled.Text = "Включить режим для ребёнка";

        chkSleepMorning.AutoSize = true;
        chkSleepMorning.Location = new Point(16, 152);
        chkSleepMorning.Text = "Утром переводить в сон (иначе — выключение)";

        btnApply.Location = new Point(16, 188);
        btnApply.Size = new Size(120, 32);
        btnApply.Text = "Применить";
        btnApply.Click += btnApply_Click;

        btnAutostart.Location = new Point(148, 188);
        btnAutostart.Size = new Size(140, 32);
        btnAutostart.Text = "Включить автозапуск";
        btnAutostart.Click += btnAutostart_Click;

        btnRemoveAutostart.Location = new Point(300, 188);
        btnRemoveAutostart.Size = new Size(150, 32);
        btnRemoveAutostart.Text = "Убрать автозапуск";
        btnRemoveAutostart.Click += btnRemoveAutostart_Click;

        btnLaunchChild.Location = new Point(16, 228);
        btnLaunchChild.Size = new Size(180, 32);
        btnLaunchChild.Text = "Запустить режим «ребёнка»";
        btnLaunchChild.Click += btnLaunchChild_Click;

        btnChangePassword.Location = new Point(210, 228);
        btnChangePassword.Size = new Size(150, 32);
        btnChangePassword.Text = "Сменить пароль";
        btnChangePassword.Click += btnChangePassword_Click;

        statusLabel.Location = new Point(16, 272);
        statusLabel.Size = new Size(440, 56);
        statusLabel.Text = "";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(470, 340);
        Controls.AddRange([
            lblTitle, lblShutdown, dtpShutdown, lblUnlock, dtpUnlock,
            chkEnabled, chkSleepMorning, btnApply, btnAutostart, btnRemoveAutostart,
            btnLaunchChild, btnChangePassword, statusLabel
        ]);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Kids Computer Time Guard — Родитель";
        ResumeLayout(false);
        PerformLayout();
    }

    private Label lblTitle;
    private Label lblShutdown;
    private DateTimePicker dtpShutdown;
    private Label lblUnlock;
    private DateTimePicker dtpUnlock;
    private CheckBox chkEnabled;
    private CheckBox chkSleepMorning;
    private Button btnApply;
    private Label statusLabel;
    private Button btnAutostart;
    private Button btnRemoveAutostart;
    private Button btnLaunchChild;
    private Button btnChangePassword;
}
