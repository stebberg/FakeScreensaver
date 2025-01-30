namespace FakeScreensaver;
internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Cursor.Hide();
        foreach (var screen in Screen.AllScreens)
        {
            var screenForm = new Form
            {
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized,
                TopMost = true,
                BackColor = Color.Black,
                ShowInTaskbar = false,
                Bounds = screen.Bounds,
            };

            if (screen.Primary)
                AddLabel(screenForm);

            screenForm.Show();
        }
        Application.AddMessageFilter(new KeyPressMessageFilter());
        Application.Run();
    }

    static void AddLabel(Form screenForm)
    {
        var timeLabel = new Label
        {
            Font = new Font("Consolas", 8, FontStyle.Regular),
            ForeColor = Color.Gray,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        screenForm.Controls.Add(timeLabel);
        screenForm.Shown += (sender, e) =>
        {
            var timerThread = new Thread(() => UpdateTime(timeLabel, screenForm));
            timerThread.IsBackground = true;
            timerThread.Start();
        };
    }

    static void UpdateTime(Label timeLabel, Form screenForm)
    {
        const int SLEEP_MS = 10_000;
        while (screenForm.Visible)
        {
            var text = $"{DateTime.Now.ToString("HH:mm")}{Environment.NewLine + Environment.NewLine}life is an illusion";
            if (screenForm.InvokeRequired)
                screenForm.Invoke(new Action(() => { timeLabel.Text = text; }));
            else
                timeLabel.Text = text;
            Thread.Sleep(SLEEP_MS);
        }
    }

    class KeyPressMessageFilter : IMessageFilter
    {
        const int WM_KEYDOWN = 0x0100;
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_KEYDOWN)
            {
                bool shiftIsPressed = Control.ModifierKeys == Keys.Shift;
                bool escIsPressed = (int)m.WParam == (int)Keys.Escape;
                if (shiftIsPressed && escIsPressed)
                {
                    Application.Exit();
                    return true;
                }
            }
            return false;
        }
    }
}