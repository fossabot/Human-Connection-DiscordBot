using HumanConnection.DiscordBot.Properties;

namespace HumanConnection.DiscordBot
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
            this.consoleGroup = new System.Windows.Forms.GroupBox();
            this.hcLogo = new System.Windows.Forms.PictureBox();
            this.statusColor = new System.Windows.Forms.Button();
            this.consoleLog = new System.Windows.Forms.TextBox();
            this.HCBot_GUI_Tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.consoleSwitch = new System.Windows.Forms.Button();
            this.consoleGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hcLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // launch
            // 
            this.launch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.launch.Location = new System.Drawing.Point(18, 345);
            this.launch.Name = "launch";
            this.launch.Size = new System.Drawing.Size(120, 47);
            this.launch.TabIndex = 2;
            this.launch.TabStop = false;
            this.launch.Text = "Launch";
            this.launch.UseVisualStyleBackColor = true;
            this.launch.Click += new System.EventHandler(this.LaunchBot);
            // 
            // stop
            // 
            this.stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.stop.Location = new System.Drawing.Point(282, 345);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(125, 47);
            this.stop.TabIndex = 3;
            this.stop.TabStop = false;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Visible = false;
            this.stop.Click += new System.EventHandler(this.StopBot);
            // 
            // tokenField
            // 
            this.tokenField.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenField.Location = new System.Drawing.Point(70, 313);
            this.tokenField.Name = "tokenField";
            this.tokenField.Size = new System.Drawing.Size(337, 26);
            this.tokenField.TabIndex = 4;
            // 
            // tokenLabel
            // 
            this.tokenLabel.AutoSize = true;
            this.tokenLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tokenLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenLabel.Location = new System.Drawing.Point(14, 316);
            this.tokenLabel.Name = "tokenLabel";
            this.tokenLabel.Size = new System.Drawing.Size(53, 20);
            this.tokenLabel.TabIndex = 5;
            this.tokenLabel.TabStop = true;
            this.tokenLabel.Text = "Token";
            this.tokenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.tokenLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TokenLabel_LinkClicked);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(53, 22);
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
            this.statusBox.Location = new System.Drawing.Point(119, 19);
            this.statusBox.Name = "statusBox";
            this.statusBox.ReadOnly = true;
            this.statusBox.Size = new System.Drawing.Size(269, 19);
            this.statusBox.TabIndex = 7;
            this.statusBox.TabStop = false;
            this.statusBox.Text = "unkown";
            this.statusBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // consoleGroup
            // 
            this.consoleGroup.Controls.Add(this.hcLogo);
            this.consoleGroup.Controls.Add(this.statusColor);
            this.consoleGroup.Controls.Add(this.consoleLog);
            this.consoleGroup.Controls.Add(this.statusBox);
            this.consoleGroup.Controls.Add(this.statusLabel);
            this.consoleGroup.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.consoleGroup.Location = new System.Drawing.Point(13, 12);
            this.consoleGroup.Name = "consoleGroup";
            this.consoleGroup.Size = new System.Drawing.Size(394, 285);
            this.consoleGroup.TabIndex = 8;
            this.consoleGroup.TabStop = false;
            this.consoleGroup.Text = "Console";
            // 
            // hcLogo
            // 
            this.hcLogo.Image = global::HumanConnection.DiscordBot.Properties.Resources.HC_Control_Logo_white;
            this.hcLogo.Location = new System.Drawing.Point(3, 0);
            this.hcLogo.Name = "hcLogo";
            this.hcLogo.Size = new System.Drawing.Size(388, 282);
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
            this.statusColor.Location = new System.Drawing.Point(15, 19);
            this.statusColor.Name = "statusColor";
            this.statusColor.Size = new System.Drawing.Size(32, 26);
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
            this.consoleLog.Font = new System.Drawing.Font("Consolas", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.consoleLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.consoleLog.Location = new System.Drawing.Point(5, 51);
            this.consoleLog.Multiline = true;
            this.consoleLog.Name = "consoleLog";
            this.consoleLog.ReadOnly = true;
            this.consoleLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleLog.ShortcutsEnabled = false;
            this.consoleLog.Size = new System.Drawing.Size(383, 228);
            this.consoleLog.TabIndex = 8;
            this.consoleLog.TabStop = false;
            this.consoleLog.Text = "Test";
            // 
            // HCBot_GUI_Tray
            // 
            this.HCBot_GUI_Tray.Icon = global::HumanConnection.DiscordBot.Properties.Resources.hclogo;
            this.HCBot_GUI_Tray.Text = "Human Connection\nHC Control";
            this.HCBot_GUI_Tray.DoubleClick += new System.EventHandler(this.HCBot_GUI_Tray_DoubleClick);
            // 
            // consoleSwitch
            // 
            this.consoleSwitch.Location = new System.Drawing.Point(144, 345);
            this.consoleSwitch.Name = "consoleSwitch";
            this.consoleSwitch.Size = new System.Drawing.Size(132, 47);
            this.consoleSwitch.TabIndex = 9;
            this.consoleSwitch.Text = "Hide Console";
            this.consoleSwitch.UseVisualStyleBackColor = true;
            this.consoleSwitch.Click += new System.EventHandler(this.ConsoleSwitch_Click);
            // 
            // HCBotGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 404);
            this.Controls.Add(this.consoleSwitch);
            this.Controls.Add(this.consoleGroup);
            this.Controls.Add(this.tokenLabel);
            this.Controls.Add(this.tokenField);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.launch);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::HumanConnection.DiscordBot.Properties.Resources.hclogo;
            this.MaximizeBox = false;
            this.Name = "HCBotGUI";
            this.Text = "Human Connection - HC Control";
            this.SizeChanged += new System.EventHandler(this.HCBot_GUI_SizeChanged);
            this.consoleGroup.ResumeLayout(false);
            this.consoleGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hcLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button launch;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.TextBox tokenField;
        private System.Windows.Forms.LinkLabel tokenLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox statusBox;
        private System.Windows.Forms.GroupBox consoleGroup;
        private System.Windows.Forms.TextBox consoleLog;
        private System.Windows.Forms.Button statusColor;
        private System.Windows.Forms.NotifyIcon HCBot_GUI_Tray;
        private System.Windows.Forms.PictureBox hcLogo;
        private System.Windows.Forms.Button consoleSwitch;
    }
}