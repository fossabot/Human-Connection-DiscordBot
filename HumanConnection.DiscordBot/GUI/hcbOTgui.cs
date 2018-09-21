using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HumanConnection.DiscordBot.GUI
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
            }*/
            ContextMenu contextMenu = new ContextMenu();
            MenuItem closeItem = new MenuItem();

            contextMenu.MenuItems.AddRange(
                new MenuItem[] {
                    closeItem
                }
            );

            closeItem.Index = 0;
            closeItem.Text = "Shutdown";
            closeItem.Click += new EventHandler(CloseItem_Click);
            HCBot_GUI_Tray.ContextMenu = contextMenu;
        }

        private async void CloseItem_Click(object sender, EventArgs e)
        {
            Program.Stop();
            await Task.Delay(5000);
            Close();
        }

        private void LaunchBot(object sender, EventArgs e)
        {
            Program.Run();
        }

        private void StopBot(object sender, EventArgs e)
        {
            Program.Stop();
        }

        private void TokenLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://discordapp.com/developers/applications/");
        }

        public string GetToken
        {
            get
            {
                /*RegistryKey TokenKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Latias.eu IT\\PekeBot", false);
                string token = TokenKey.GetValue("Token").ToString();*/
                string token = "NDg3OTEwODYzNjk3NzM5Nzk3.Dn3dUQ.nmyHskPUyQ0kBqM_gpUBEbd_wGQ";
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
            if (NativeConsole.HCBotConsole.GetDesktopNotifications())
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
            Activate();
            Show();
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
            }
        }

        #region Native console switch
        private void ToggleNativeConsole(object sender, EventArgs e)
        {
            if(NativeConsole.HCBotConsole.ConsoleVisible())
            {
                NativeConsole.HCBotConsole.HideConsoleWindow();
                consoleSwitch.Text = "Show native console";
            }
            else
            {
                NativeConsole.HCBotConsole.ShowConsoleWindow();
                consoleSwitch.Text = "Hide native console";
            }
        }
        #endregion

        #region Toggle modules
        private void ToggleAdminModule(object sender, EventArgs e)
        {
            NativeConsole.HCBotConsole.AdminModuleEnabled = !NativeConsole.HCBotConsole.AdminModuleEnabled;
            adminModuleConfig.Text = NativeConsole.HCBotConsole.GetAdminModuleText();
        }

        private void ToggleGreetModule(object sender, EventArgs e)
        {
            NativeConsole.HCBotConsole.GreetModuleEnabled = !NativeConsole.HCBotConsole.GreetModuleEnabled;
            greetModuleConfig.Text = NativeConsole.HCBotConsole.GetGreetModuleText();
        }

        private void ToggleActivityModule(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ToggleBirthdayModule(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ToggleGuidanceModule(object sender, EventArgs e)
        {
            NativeConsole.HCBotConsole.GuidanceModuleEnabled = !NativeConsole.HCBotConsole.GuidanceModuleEnabled;
            guidanceModuleConfig.Text = NativeConsole.HCBotConsole.GetGuidanceModuleText();
        }
        #endregion

        #region Autoscroll log
        private void AutoScrollText(object sender, EventArgs e)
        {
            consoleLog.SelectionStart = consoleLog.TextLength;
            consoleLog.ScrollToCaret();
        }
        #endregion
    }
}
