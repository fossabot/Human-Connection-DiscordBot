using HumanConnection.DiscordBot.Properties;

namespace HumanConnection.DiscordBot.GUI
{
    partial class HCBotGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.launch = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.tokenField = new System.Windows.Forms.TextBox();
            this.tokenLabel = new System.Windows.Forms.LinkLabel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.statusBox = new System.Windows.Forms.TextBox();
            this.hcLogo = new System.Windows.Forms.PictureBox();
            this.statusColor = new System.Windows.Forms.Button();
            this.consoleLog = new System.Windows.Forms.TextBox();
            this.HCBot_GUI_Tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.consoleSwitch = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.homeTab = new System.Windows.Forms.TabPage();
            this.consoleTab = new System.Windows.Forms.TabPage();
            this.moduleConfigTab = new System.Windows.Forms.TabPage();
            this.adminModuleConfig = new System.Windows.Forms.Button();
            this.greetModuleConfig = new System.Windows.Forms.Button();
            this.guidanceModuleConfig = new System.Windows.Forms.Button();
            this.birthdayModuleConfig = new System.Windows.Forms.Button();
            this.activityModuleConfig = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.configTab = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hcLogo)).BeginInit();
            this.tabControl.SuspendLayout();
            this.homeTab.SuspendLayout();
            this.consoleTab.SuspendLayout();
            this.moduleConfigTab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.configTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // launch
            // 
            this.launch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.launch.Location = new System.Drawing.Point(7, 441);
            this.launch.Name = "launch";
            this.launch.Size = new System.Drawing.Size(143, 47);
            this.launch.TabIndex = 2;
            this.launch.TabStop = false;
            this.launch.Text = "Launch";
            this.launch.UseVisualStyleBackColor = true;
            this.launch.Click += new System.EventHandler(this.LaunchBot);
            // 
            // stop
            // 
            this.stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.stop.Location = new System.Drawing.Point(350, 441);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(143, 47);
            this.stop.TabIndex = 3;
            this.stop.TabStop = false;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Visible = false;
            this.stop.Click += new System.EventHandler(this.StopBot);
            // 
            // tokenField
            // 
            this.tokenField.Enabled = false;
            this.tokenField.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenField.Location = new System.Drawing.Point(65, 6);
            this.tokenField.Name = "tokenField";
            this.tokenField.Size = new System.Drawing.Size(337, 26);
            this.tokenField.TabIndex = 4;
            this.tokenField.TabStop = false;
            // 
            // tokenLabel
            // 
            this.tokenLabel.AutoSize = true;
            this.tokenLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tokenLabel.Enabled = false;
            this.tokenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenLabel.Location = new System.Drawing.Point(6, 9);
            this.tokenLabel.Name = "tokenLabel";
            this.tokenLabel.Size = new System.Drawing.Size(53, 20);
            this.tokenLabel.TabIndex = 5;
            this.tokenLabel.Text = "Token";
            this.tokenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tokenLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TokenLabel_LinkClicked);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(90, 16);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(60, 20);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "Status:";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusBox
            // 
            this.statusBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBox.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.statusBox.Location = new System.Drawing.Point(203, 17);
            this.statusBox.Name = "statusBox";
            this.statusBox.ReadOnly = true;
            this.statusBox.Size = new System.Drawing.Size(269, 19);
            this.statusBox.TabIndex = 7;
            this.statusBox.TabStop = false;
            this.statusBox.Text = "unkown";
            this.statusBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // hcLogo
            // 
            this.hcLogo.BackColor = System.Drawing.Color.White;
            this.hcLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hcLogo.Image = global::HumanConnection.DiscordBot.Properties.Resources.HC_Control_Logo_white;
            this.hcLogo.Location = new System.Drawing.Point(3, 3);
            this.hcLogo.Name = "hcLogo";
            this.hcLogo.Size = new System.Drawing.Size(480, 350);
            this.hcLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.hcLogo.TabIndex = 10;
            this.hcLogo.TabStop = false;
            // 
            // statusColor
            // 
            this.statusColor.AccessibleRole = System.Windows.Forms.AccessibleRole.Text;
            this.statusColor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.statusColor.CausesValidation = false;
            this.statusColor.Cursor = System.Windows.Forms.Cursors.No;
            this.statusColor.Enabled = false;
            this.statusColor.FlatAppearance.BorderSize = 0;
            this.statusColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statusColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusColor.ForeColor = System.Drawing.Color.RoyalBlue;
            this.statusColor.Location = new System.Drawing.Point(7, 6);
            this.statusColor.Name = "statusColor";
            this.statusColor.Size = new System.Drawing.Size(40, 40);
            this.statusColor.TabIndex = 9;
            this.statusColor.TabStop = false;
            this.statusColor.Text = "D";
            this.statusColor.UseCompatibleTextRendering = true;
            this.statusColor.UseVisualStyleBackColor = false;
            // 
            // consoleLog
            // 
            this.consoleLog.AcceptsReturn = true;
            this.consoleLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.consoleLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleLog.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.consoleLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleLog.Font = new System.Drawing.Font("Consolas", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.consoleLog.Location = new System.Drawing.Point(3, 3);
            this.consoleLog.Multiline = true;
            this.consoleLog.Name = "consoleLog";
            this.consoleLog.ReadOnly = true;
            this.consoleLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleLog.ShortcutsEnabled = false;
            this.consoleLog.Size = new System.Drawing.Size(476, 346);
            this.consoleLog.TabIndex = 8;
            this.consoleLog.TabStop = false;
            this.consoleLog.Text = "Init";
            this.consoleLog.TextChanged += new System.EventHandler(this.AutoScrollText);
            // 
            // HCBot_GUI_Tray
            // 
            this.HCBot_GUI_Tray.Icon = global::HumanConnection.DiscordBot.Properties.Resources.hclogo;
            this.HCBot_GUI_Tray.Text = "Human Connection\nHC Control";
            this.HCBot_GUI_Tray.Visible = true;
            this.HCBot_GUI_Tray.DoubleClick += new System.EventHandler(this.HCBot_GUI_Tray_DoubleClick);
            // 
            // consoleSwitch
            // 
            this.consoleSwitch.Location = new System.Drawing.Point(156, 441);
            this.consoleSwitch.Name = "consoleSwitch";
            this.consoleSwitch.Size = new System.Drawing.Size(188, 47);
            this.consoleSwitch.TabIndex = 9;
            this.consoleSwitch.Text = "Show native console";
            this.consoleSwitch.UseVisualStyleBackColor = true;
            this.consoleSwitch.Click += new System.EventHandler(this.ToggleNativeConsole);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.homeTab);
            this.tabControl.Controls.Add(this.configTab);
            this.tabControl.Controls.Add(this.moduleConfigTab);
            this.tabControl.Controls.Add(this.consoleTab);
            this.tabControl.Location = new System.Drawing.Point(3, 50);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(494, 389);
            this.tabControl.TabIndex = 11;
            // 
            // homeTab
            // 
            this.homeTab.BackColor = System.Drawing.Color.Black;
            this.homeTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.homeTab.Controls.Add(this.hcLogo);
            this.homeTab.Location = new System.Drawing.Point(4, 29);
            this.homeTab.Name = "homeTab";
            this.homeTab.Padding = new System.Windows.Forms.Padding(3);
            this.homeTab.Size = new System.Drawing.Size(486, 356);
            this.homeTab.TabIndex = 0;
            this.homeTab.Text = "Home";
            // 
            // consoleTab
            // 
            this.consoleTab.BackColor = System.Drawing.Color.Black;
            this.consoleTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.consoleTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.consoleTab.Controls.Add(this.consoleLog);
            this.consoleTab.Location = new System.Drawing.Point(4, 29);
            this.consoleTab.Name = "consoleTab";
            this.consoleTab.Padding = new System.Windows.Forms.Padding(3);
            this.consoleTab.Size = new System.Drawing.Size(486, 356);
            this.consoleTab.TabIndex = 1;
            this.consoleTab.Text = "Console";
            // 
            // moduleConfigTab
            // 
            this.moduleConfigTab.BackColor = System.Drawing.Color.Transparent;
            this.moduleConfigTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.moduleConfigTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.moduleConfigTab.Controls.Add(this.adminModuleConfig);
            this.moduleConfigTab.Controls.Add(this.greetModuleConfig);
            this.moduleConfigTab.Controls.Add(this.guidanceModuleConfig);
            this.moduleConfigTab.Controls.Add(this.birthdayModuleConfig);
            this.moduleConfigTab.Controls.Add(this.activityModuleConfig);
            this.moduleConfigTab.Location = new System.Drawing.Point(4, 29);
            this.moduleConfigTab.Name = "moduleConfigTab";
            this.moduleConfigTab.Size = new System.Drawing.Size(486, 356);
            this.moduleConfigTab.TabIndex = 2;
            this.moduleConfigTab.Text = "Module Config";
            // 
            // adminModuleConfig
            // 
            this.adminModuleConfig.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.adminModuleConfig.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.adminModuleConfig.Location = new System.Drawing.Point(45, 44);
            this.adminModuleConfig.Name = "adminModuleConfig";
            this.adminModuleConfig.Size = new System.Drawing.Size(100, 100);
            this.adminModuleConfig.TabIndex = 4;
            this.adminModuleConfig.TabStop = false;
            this.adminModuleConfig.Text = "Admin\r\n\r\ndisabled";
            this.adminModuleConfig.UseVisualStyleBackColor = false;
            this.adminModuleConfig.Click += new System.EventHandler(this.ToggleAdminModule);
            // 
            // greetModuleConfig
            // 
            this.greetModuleConfig.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.greetModuleConfig.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.greetModuleConfig.Location = new System.Drawing.Point(190, 44);
            this.greetModuleConfig.Name = "greetModuleConfig";
            this.greetModuleConfig.Size = new System.Drawing.Size(100, 100);
            this.greetModuleConfig.TabIndex = 5;
            this.greetModuleConfig.TabStop = false;
            this.greetModuleConfig.Text = "Greet\r\n\r\ndisabled";
            this.greetModuleConfig.UseVisualStyleBackColor = false;
            this.greetModuleConfig.Click += new System.EventHandler(this.ToggleGreetModule);
            // 
            // guidanceModuleConfig
            // 
            this.guidanceModuleConfig.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.guidanceModuleConfig.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.guidanceModuleConfig.Location = new System.Drawing.Point(337, 44);
            this.guidanceModuleConfig.Name = "guidanceModuleConfig";
            this.guidanceModuleConfig.Size = new System.Drawing.Size(100, 100);
            this.guidanceModuleConfig.TabIndex = 8;
            this.guidanceModuleConfig.TabStop = false;
            this.guidanceModuleConfig.Text = "Guidance\r\n\r\ndisabled";
            this.guidanceModuleConfig.UseVisualStyleBackColor = false;
            this.guidanceModuleConfig.Click += new System.EventHandler(this.ToggleGuidanceModule);
            // 
            // birthdayModuleConfig
            // 
            this.birthdayModuleConfig.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.birthdayModuleConfig.Enabled = false;
            this.birthdayModuleConfig.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.birthdayModuleConfig.Location = new System.Drawing.Point(117, 166);
            this.birthdayModuleConfig.Name = "birthdayModuleConfig";
            this.birthdayModuleConfig.Size = new System.Drawing.Size(100, 100);
            this.birthdayModuleConfig.TabIndex = 7;
            this.birthdayModuleConfig.TabStop = false;
            this.birthdayModuleConfig.Text = "Birthday\r\n\r\ndisabled";
            this.birthdayModuleConfig.UseVisualStyleBackColor = false;
            this.birthdayModuleConfig.Click += new System.EventHandler(this.ToggleBirthdayModule);
            // 
            // activityModuleConfig
            // 
            this.activityModuleConfig.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.activityModuleConfig.Enabled = false;
            this.activityModuleConfig.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.activityModuleConfig.Location = new System.Drawing.Point(260, 166);
            this.activityModuleConfig.Name = "activityModuleConfig";
            this.activityModuleConfig.Size = new System.Drawing.Size(100, 100);
            this.activityModuleConfig.TabIndex = 6;
            this.activityModuleConfig.TabStop = false;
            this.activityModuleConfig.Text = "Activity\r\n\r\ndisabled";
            this.activityModuleConfig.UseVisualStyleBackColor = false;
            this.activityModuleConfig.Click += new System.EventHandler(this.ToggleActivityModule);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl);
            this.panel1.Controls.Add(this.consoleSwitch);
            this.panel1.Controls.Add(this.statusColor);
            this.panel1.Controls.Add(this.statusBox);
            this.panel1.Controls.Add(this.launch);
            this.panel1.Controls.Add(this.stop);
            this.panel1.Controls.Add(this.statusLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(498, 496);
            this.panel1.TabIndex = 12;
            // 
            // configTab
            // 
            this.configTab.BackColor = System.Drawing.Color.Silver;
            this.configTab.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.configTab.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.configTab.Controls.Add(this.button1);
            this.configTab.Controls.Add(this.tokenField);
            this.configTab.Controls.Add(this.tokenLabel);
            this.configTab.Location = new System.Drawing.Point(4, 29);
            this.configTab.Name = "configTab";
            this.configTab.Padding = new System.Windows.Forms.Padding(3);
            this.configTab.Size = new System.Drawing.Size(486, 356);
            this.configTab.TabIndex = 3;
            this.configTab.Text = "Bot Config";
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.button1.Location = new System.Drawing.Point(408, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(68, 23);
            this.button1.TabIndex = 12;
            this.button1.TabStop = false;
            this.button1.Text = "Set";
            // 
            // HCBotGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 496);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::HumanConnection.DiscordBot.Properties.Resources.hclogo;
            this.MaximizeBox = false;
            this.Name = "HCBotGUI";
            this.Text = "Human Connection - HC Control";
            this.SizeChanged += new System.EventHandler(this.HCBot_GUI_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.hcLogo)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.homeTab.ResumeLayout(false);
            this.consoleTab.ResumeLayout(false);
            this.consoleTab.PerformLayout();
            this.moduleConfigTab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.configTab.ResumeLayout(false);
            this.configTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button launch;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.TextBox tokenField;
        private System.Windows.Forms.LinkLabel tokenLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox statusBox;
        private System.Windows.Forms.TextBox consoleLog;
        private System.Windows.Forms.Button statusColor;
        private System.Windows.Forms.NotifyIcon HCBot_GUI_Tray;
        private System.Windows.Forms.PictureBox hcLogo;
        private System.Windows.Forms.Button consoleSwitch;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage homeTab;
        private System.Windows.Forms.TabPage consoleTab;
        private System.Windows.Forms.TabPage moduleConfigTab;
        private System.Windows.Forms.Button adminModuleConfig;
        private System.Windows.Forms.Button greetModuleConfig;
        private System.Windows.Forms.Button activityModuleConfig;
        private System.Windows.Forms.Button birthdayModuleConfig;
        private System.Windows.Forms.Button guidanceModuleConfig;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage configTab;
        private System.Windows.Forms.Button button1;
    }
}