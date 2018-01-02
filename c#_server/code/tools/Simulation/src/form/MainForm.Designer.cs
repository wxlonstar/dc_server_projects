namespace dc
{
    partial class MainForm
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.NetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetLoginOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetCreateUserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetUserInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NetModifyInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SceneChangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LogicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SysMailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SysRelationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SysRelationAppleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SysRelationListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SysChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.m_status_account = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_status_user = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_status_scene = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_status_log = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_main_panel = new System.Windows.Forms.Panel();
            this.mainMenuStrip.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NetToolStripMenuItem,
            this.SceneToolStripMenuItem,
            this.LogicToolStripMenuItem,
            this.ToolsToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(984, 25);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "main_menuStrip";
            // 
            // NetToolStripMenuItem
            // 
            this.NetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NetLoginToolStripMenuItem,
            this.NetLoginOutToolStripMenuItem,
            this.NetCreateUserToolStripMenuItem,
            this.NetUserInfoToolStripMenuItem,
            this.NetModifyInfoToolStripMenuItem});
            this.NetToolStripMenuItem.Name = "NetToolStripMenuItem";
            this.NetToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.NetToolStripMenuItem.Text = "账号";
            // 
            // NetLoginToolStripMenuItem
            // 
            this.NetLoginToolStripMenuItem.Name = "NetLoginToolStripMenuItem";
            this.NetLoginToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NetLoginToolStripMenuItem.Text = "登录";
            this.NetLoginToolStripMenuItem.Click += new System.EventHandler(this.NetLoginToolStripMenuItem_Click);
            // 
            // NetLoginOutToolStripMenuItem
            // 
            this.NetLoginOutToolStripMenuItem.Name = "NetLoginOutToolStripMenuItem";
            this.NetLoginOutToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NetLoginOutToolStripMenuItem.Text = "登出";
            this.NetLoginOutToolStripMenuItem.Click += new System.EventHandler(this.NetLoginOutToolStripMenuItem_Click);
            // 
            // NetCreateUserToolStripMenuItem
            // 
            this.NetCreateUserToolStripMenuItem.Name = "NetCreateUserToolStripMenuItem";
            this.NetCreateUserToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NetCreateUserToolStripMenuItem.Text = "角色";
            this.NetCreateUserToolStripMenuItem.Click += new System.EventHandler(this.NetCreateUserToolStripMenuItem_Click);
            // 
            // NetUserInfoToolStripMenuItem
            // 
            this.NetUserInfoToolStripMenuItem.Name = "NetUserInfoToolStripMenuItem";
            this.NetUserInfoToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NetUserInfoToolStripMenuItem.Text = "信息";
            this.NetUserInfoToolStripMenuItem.Click += new System.EventHandler(this.NetUserInfoToolStripMenuItem_Click);
            // 
            // NetModifyInfoToolStripMenuItem
            // 
            this.NetModifyInfoToolStripMenuItem.Name = "NetModifyInfoToolStripMenuItem";
            this.NetModifyInfoToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.NetModifyInfoToolStripMenuItem.Text = "修改数据";
            this.NetModifyInfoToolStripMenuItem.Click += new System.EventHandler(this.NetModifyInfoToolStripMenuItem_Click);
            // 
            // SceneToolStripMenuItem
            // 
            this.SceneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SceneChangeToolStripMenuItem});
            this.SceneToolStripMenuItem.Name = "SceneToolStripMenuItem";
            this.SceneToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.SceneToolStripMenuItem.Text = "场景";
            // 
            // SceneChangeToolStripMenuItem
            // 
            this.SceneChangeToolStripMenuItem.Name = "SceneChangeToolStripMenuItem";
            this.SceneChangeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.SceneChangeToolStripMenuItem.Text = "切换场景";
            this.SceneChangeToolStripMenuItem.Click += new System.EventHandler(this.SceneChangeToolStripMenuItem_Click);
            // 
            // LogicToolStripMenuItem
            // 
            this.LogicToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SysMailToolStripMenuItem,
            this.SysRelationToolStripMenuItem,
            this.SysChatToolStripMenuItem});
            this.LogicToolStripMenuItem.Name = "LogicToolStripMenuItem";
            this.LogicToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.LogicToolStripMenuItem.Text = "系统";
            // 
            // SysMailToolStripMenuItem
            // 
            this.SysMailToolStripMenuItem.Name = "SysMailToolStripMenuItem";
            this.SysMailToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.SysMailToolStripMenuItem.Text = "邮件";
            this.SysMailToolStripMenuItem.Click += new System.EventHandler(this.SysMailToolStripMenuItem_Click);
            // 
            // SysRelationToolStripMenuItem
            // 
            this.SysRelationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SysRelationAppleToolStripMenuItem,
            this.SysRelationListToolStripMenuItem});
            this.SysRelationToolStripMenuItem.Name = "SysRelationToolStripMenuItem";
            this.SysRelationToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.SysRelationToolStripMenuItem.Text = "关系";
            // 
            // SysRelationAppleToolStripMenuItem
            // 
            this.SysRelationAppleToolStripMenuItem.Name = "SysRelationAppleToolStripMenuItem";
            this.SysRelationAppleToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.SysRelationAppleToolStripMenuItem.Text = "申请列表";
            this.SysRelationAppleToolStripMenuItem.Click += new System.EventHandler(this.SysRelationAppleToolStripMenuItem_Click);
            // 
            // SysRelationListToolStripMenuItem
            // 
            this.SysRelationListToolStripMenuItem.Name = "SysRelationListToolStripMenuItem";
            this.SysRelationListToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.SysRelationListToolStripMenuItem.Text = "好友列表";
            this.SysRelationListToolStripMenuItem.Click += new System.EventHandler(this.SysRelationListToolStripMenuItem_Click);
            // 
            // SysChatToolStripMenuItem
            // 
            this.SysChatToolStripMenuItem.Name = "SysChatToolStripMenuItem";
            this.SysChatToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.SysChatToolStripMenuItem.Text = "聊天";
            this.SysChatToolStripMenuItem.Click += new System.EventHandler(this.SysChatToolStripMenuItem_Click);
            // 
            // ToolsToolStripMenuItem
            // 
            this.ToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolsSetToolStripMenuItem,
            this.ToolsLogToolStripMenuItem,
            this.ToolsConsoleToolStripMenuItem,
            this.pingToolStripMenuItem});
            this.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem";
            this.ToolsToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.ToolsToolStripMenuItem.Text = "工具";
            // 
            // ToolsSetToolStripMenuItem
            // 
            this.ToolsSetToolStripMenuItem.Name = "ToolsSetToolStripMenuItem";
            this.ToolsSetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolsSetToolStripMenuItem.Text = "设置";
            // 
            // ToolsLogToolStripMenuItem
            // 
            this.ToolsLogToolStripMenuItem.Name = "ToolsLogToolStripMenuItem";
            this.ToolsLogToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolsLogToolStripMenuItem.Text = "日志";
            this.ToolsLogToolStripMenuItem.Click += new System.EventHandler(this.ToolsLogToolStripMenuItem_Click);
            // 
            // ToolsConsoleToolStripMenuItem
            // 
            this.ToolsConsoleToolStripMenuItem.Name = "ToolsConsoleToolStripMenuItem";
            this.ToolsConsoleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolsConsoleToolStripMenuItem.Text = "控制台";
            this.ToolsConsoleToolStripMenuItem.Click += new System.EventHandler(this.ToolsConsoleToolStripMenuItem_Click);
            // 
            // pingToolStripMenuItem
            // 
            this.pingToolStripMenuItem.Name = "pingToolStripMenuItem";
            this.pingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pingToolStripMenuItem.Text = "Ping";
            this.pingToolStripMenuItem.Click += new System.EventHandler(this.ToolsPingToolStripMenuItem_Click);
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_status_account,
            this.m_status_user,
            this.m_status_scene,
            this.m_status_log});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 740);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(984, 22);
            this.mainStatusStrip.TabIndex = 2;
            this.mainStatusStrip.Text = "statusStrip1";
            // 
            // m_status_account
            // 
            this.m_status_account.AutoSize = false;
            this.m_status_account.Name = "m_status_account";
            this.m_status_account.Size = new System.Drawing.Size(120, 17);
            this.m_status_account.Text = "未连接";
            this.m_status_account.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_status_user
            // 
            this.m_status_user.AutoSize = false;
            this.m_status_user.Name = "m_status_user";
            this.m_status_user.Size = new System.Drawing.Size(120, 17);
            this.m_status_user.Text = "未登陆";
            this.m_status_user.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_status_scene
            // 
            this.m_status_scene.AutoSize = false;
            this.m_status_scene.Name = "m_status_scene";
            this.m_status_scene.Size = new System.Drawing.Size(120, 17);
            this.m_status_scene.Text = "当前场景:";
            this.m_status_scene.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_status_log
            // 
            this.m_status_log.AutoSize = false;
            this.m_status_log.Margin = new System.Windows.Forms.Padding(400, 3, 0, 2);
            this.m_status_log.Name = "m_status_log";
            this.m_status_log.Size = new System.Drawing.Size(200, 17);
            this.m_status_log.Text = "当前操作";
            this.m_status_log.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_main_panel
            // 
            this.m_main_panel.Location = new System.Drawing.Point(0, 28);
            this.m_main_panel.Name = "m_main_panel";
            this.m_main_panel.Size = new System.Drawing.Size(984, 709);
            this.m_main_panel.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 762);
            this.Controls.Add(this.m_main_panel);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.mainMenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模拟客户端";
            this.Load += new System.EventHandler(this.OnLoad);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem NetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LogicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetLoginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetLoginOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SceneToolStripMenuItem;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel m_status_account;
        private System.Windows.Forms.ToolStripStatusLabel m_status_log;
        private System.Windows.Forms.ToolStripMenuItem NetCreateUserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsLogToolStripMenuItem;
        private System.Windows.Forms.Panel m_main_panel;
        private System.Windows.Forms.ToolStripStatusLabel m_status_user;
        private System.Windows.Forms.ToolStripStatusLabel m_status_scene;
        private System.Windows.Forms.ToolStripMenuItem SceneChangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetUserInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem NetModifyInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysMailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysRelationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysRelationAppleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysRelationListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysChatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pingToolStripMenuItem;


    }
}

