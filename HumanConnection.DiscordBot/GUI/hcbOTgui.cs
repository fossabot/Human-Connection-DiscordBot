using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HumanConnection.DiscordBot
{
    public partial class HCBotGUI : Form
    {

        public HCBotGUI()
        {
            InitializeComponent();

            /*RegistryKey RegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Latias.eu IT\\PekeBot", true);
            if(RegKey != null)
            {
                object FirstRunDone = RegKey.GetValue("FirstRunDone");
                if (Convert.ToBoolean(FirstRunDone))
                {
                    tokenField.Text = Convert.ToString(RegKey.GetValue("Token"));
                    RegKey.SetValue("Version", Application.ProductVersion, RegistryValueKind.String);
                }
                else
                {
                    RegKey.SetValue("Version", Application.ProductVersion, RegistryValueKind.String);
                    RegKey.SetValue("User", "admin@latias.eu", RegistryValueKind.String);
                    RegKey.SetValue("FirstRunDone", true, RegistryValueKind.DWord);
                }

            }
            else
            {
                RegistryKey RegKeyNew = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Latias.eu IT\\PekeBot", true);
                RegKey.SetValue("Version", Application.ProductVersion, RegistryValueKind.String);
                RegKeyNew.SetValue("User", "admin@latias.eu", RegistryValueKind.String);
                RegKeyNew.SetValue("FirstRunDone", false, RegistryValueKind.DWord);
            }

            bot = new Process();
            bot.StartInfo.WorkingDirectory = @"..\\..\\..\\PekeBot\\bin\\Debug\\";
            bot.StartInfo.FileName = "PekeBot.exe";*/
        }//ss

        private void LaunchBot(object sender, EventArgs e)
        {
            Program.Run();
            hcLogo.Visible = false;
        }

        private void StopBot(object sender, EventArgs e)
        {
            Program.Stop();
            hcLogo.Visible = true;
        }

        private void TokenLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://discordapp.com/developers/applications/");
        }

        public string GetToken
        {
            get
            {
                RegistryKey TokenKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Latias.eu IT\\PekeBot", false);
                string token = TokenKey.GetValue("Token").ToString();
                return token;
            }
        }

        public void SetConnectionStatus(string s, System.Drawing.Color statusColor)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, System.Drawing.Color>(SetConnectionStatus), new object[] { s, statusColor });
                return;
            }
            this.statusBox.Text = s;
            this.statusColor.BackColor = statusColor;
            this.statusColor.Text = s.Substring(0, 1);
            if (HCBotConsole.GetDesktopNotifications())
            {
                HCBot_GUI_Tray.BalloonTipText = s;
                HCBot_GUI_Tray.ShowBalloonTip(1000);
            }
        }

        public void AppendConsole(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendConsole), new object[] { msg });
                return;
            }
            consoleLog.AppendText(Environment.NewLine + "[" + DateTime.Now.ToShortTimeString() + "]: " + msg);
        }

        private void HCBot_GUI_Tray_DoubleClick(object sender, EventArgs e)
        {
            Show();
            HCBot_GUI_Tray.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        public void ChangeLock(int type)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(ChangeLock), new object[] { type });
                return;
            }

            switch (type)
            {
                case 1:
                    launch.Visible = false;
                    stop.Visible = true;
                    break;
                case 2:
                    launch.Visible = true;
                    stop.Visible = false;
                    break;
            }
        }

        private void HCBot_GUI_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                HCBot_GUI_Tray.Visible = true;
            }
        }

        private void consoleSwitch_Click(object sender, EventArgs e)
        {
            if(HCBotConsole.ConsoleVisible())
            {
                HCBotConsole.HideConsoleWindow();
                consoleSwitch.Text = "Show Console";
            }
            else
            {
                HCBotConsole.ShowConsoleWindow();
                consoleSwitch.Text = "Hide Console";
            }
        }
    }
}
