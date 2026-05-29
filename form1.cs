using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ScheduledShutdown
{
    public class Form1 : Form
    {
        private Timer checkTimer;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private DateTimePicker shutdownTimePicker;
        private DateTimePicker unlockTimePicker;
        private Label statusLabel;
        private bool isShutdownScheduled = false;

        public Form1()
        {
            InitializeComponent();
            InitializeTray();
            LoadSettings();
            StartMonitoring();
        }

        private void InitializeComponent()
        {
            this.Text = "Планировщик выключения";
            this.Size = new System.Drawing.Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosing += Form1_FormClosing;
            this.ShowInTaskbar = false;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 2,
                RowCount = 4
            };

            // Время выключения
            mainPanel.Controls.Add(new Label { Text = "Время выключения:", AutoSize = true }, 0, 0);
            shutdownTimePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Now.Date.AddHours(22)
            };
            shutdownTimePicker.ValueChanged += (s, e) => isShutdownScheduled = false;
            mainPanel.Controls.Add(shutdownTimePicker, 1, 0);

            // Время разблокировки
            mainPanel.Controls.Add(new Label { Text = "Время разблокировки:", AutoSize = true }, 0, 1);
            unlockTimePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
                Value = DateTime.Now.Date.AddHours(9)
            };
            mainPanel.Controls.Add(unlockTimePicker, 1, 1);

            // Статус
            statusLabel = new Label
            {
                Text = "Статус: Активен",
                AutoSize = true,
                ForeColor = System.Drawing.Color.Green
            };
            mainPanel.Controls.Add(statusLabel, 0, 2);
            mainPanel.SetColumnSpan(statusLabel, 2);

            // Кнопки
            var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft };
            var saveButton = new Button { Text = "Сохранить", Size = new System.Drawing.Size(100, 30) };
            saveButton.Click += (s, e) => SaveSettings();
            var testButton = new Button { Text = "Тест выключения", Size = new System.Drawing.Size(100, 30) };
            testButton.Click += (s, e) =>
            {
                if (MessageBox.Show("Тестовое выключение через 10 секунд?", "Тест",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    Process.Start("shutdown", "/s /f /t 10");
            };
            buttonPanel.Controls.Add(testButton);
            buttonPanel.Controls.Add(saveButton);
            mainPanel.Controls.Add(buttonPanel, 0, 3);
            mainPanel.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainPanel);
        }

        private void InitializeTray()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Shield,
                Visible = true,
                Text = "Планировщик выключения"
            };

            contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => this.Show());
            contextMenu.Items.Add("Выключить сейчас", null, (s, e) =>
            {
                if (MessageBox.Show("Вы действительно хотите выключить компьютер сейчас?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    Process.Start("shutdown", "/s /f /t 10");
            });
            contextMenu.Items.Add("Отменить выключение", null, (s, e) => Process.Start("shutdown", "/a"));
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Выход", null, (s, e) => { notifyIcon.Visible = false; Application.Exit(); });

            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.DoubleClick += (s, e) => this.Show();
        }

        private void LoadSettings()
        {
            try
            {
                string configPath = Path.Combine(Application.StartupPath, "config.ini");
                if (File.Exists(configPath))
                {
                    var lines = File.ReadAllLines(configPath);
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("ShutdownTime="))
                        {
                            var time = line.Substring(13);
                            if (TimeSpan.TryParse(time, out var shutdownTime))
                                shutdownTimePicker.Value = DateTime.Today.Add(shutdownTime);
                        }
                        else if (line.StartsWith("UnlockTime="))
                        {
                            var time = line.Substring(11);
                            if (TimeSpan.TryParse(time, out var unlockTimeVal))
                                unlockTimePicker.Value = DateTime.Today.Add(unlockTimeVal);
                        }
                    }
                }
            }
            catch { }
        }

        private void SaveSettings()
        {
            try
            {
                string configPath = Path.Combine(Application.StartupPath, "config.ini");
                File.WriteAllLines(configPath, new[]
                {
                    "[Settings]",
                    $"ShutdownTime={shutdownTimePicker.Value:HH:mm}",
                    $"UnlockTime={unlockTimePicker.Value:HH:mm}"
                });
                MessageBox.Show("Настройки сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void StartMonitoring()
        {
            checkTimer = new Timer();
            checkTimer.Interval = 60000;
            checkTimer.Tick += (s, e) =>
            {
                var now = DateTime.Now;
                var shutdownTime = DateTime.Today.Add(shutdownTimePicker.Value.TimeOfDay);

                if (!isShutdownScheduled && now >= shutdownTime && now < shutdownTime.AddMinutes(1))
                {
                    isShutdownScheduled = true;
                    notifyIcon.ShowBalloonTip(3000, "Планировщик выключения",
                        $"Компьютер будет выключен в {shutdownTimePicker.Value:HH:mm}", ToolTipIcon.Info);
                    System.Threading.Thread.Sleep(5000);
                    Process.Start("shutdown", "/s /f /t 30");
                }

                if (now.Date > shutdownTime.Date)
                    isShutdownScheduled = false;
            };
            checkTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}