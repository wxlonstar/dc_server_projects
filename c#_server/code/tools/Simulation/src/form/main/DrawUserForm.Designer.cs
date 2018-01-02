namespace dc
{
    partial class DrawUserForm
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
            this.m_pic_root = new System.Windows.Forms.PictureBox();
            this.mainContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.KillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddFriendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.m_pic_root)).BeginInit();
            this.mainContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_pic_root
            // 
            this.m_pic_root.Location = new System.Drawing.Point(0, 0);
            this.m_pic_root.Name = "m_pic_root";
            this.m_pic_root.Size = new System.Drawing.Size(1000, 1000);
            this.m_pic_root.TabIndex = 4;
            this.m_pic_root.TabStop = false;
            this.m_pic_root.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseDown);
            this.m_pic_root.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseMove);
            this.m_pic_root.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnPicMouseUp);
            // 
            // mainContextMenuStrip
            // 
            this.mainContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.KillToolStripMenuItem,
            this.AddFriendToolStripMenuItem,
            this.ViewToolStripMenuItem,
            this.ChatToolStripMenuItem});
            this.mainContextMenuStrip.Name = "mainContextMenuStrip";
            this.mainContextMenuStrip.Size = new System.Drawing.Size(153, 114);
            // 
            // KillToolStripMenuItem
            // 
            this.KillToolStripMenuItem.Name = "KillToolStripMenuItem";
            this.KillToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.KillToolStripMenuItem.Text = "剔除";
            this.KillToolStripMenuItem.Click += new System.EventHandler(this.KillToolStripMenuItem_Click);
            // 
            // AddFriendToolStripMenuItem
            // 
            this.AddFriendToolStripMenuItem.Name = "AddFriendToolStripMenuItem";
            this.AddFriendToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.AddFriendToolStripMenuItem.Text = "加好友";
            this.AddFriendToolStripMenuItem.Click += new System.EventHandler(this.AddFriendToolStripMenuItem_Click);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ViewToolStripMenuItem.Text = "查看";
            this.ViewToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItem_Click);
            // 
            // ChatToolStripMenuItem
            // 
            this.ChatToolStripMenuItem.Name = "ChatToolStripMenuItem";
            this.ChatToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ChatToolStripMenuItem.Text = "私聊";
            this.ChatToolStripMenuItem.Click += new System.EventHandler(this.ChatToolStripMenuItem_Click);
            // 
            // DrawUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1000, 1000);
            this.Controls.Add(this.m_pic_root);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "角色";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.m_pic_root)).EndInit();
            this.mainContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_pic_root;
        private System.Windows.Forms.ContextMenuStrip mainContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem KillToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddFriendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ChatToolStripMenuItem;


    }
}

