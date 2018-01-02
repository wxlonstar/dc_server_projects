namespace dc
{
    partial class PingForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_txt_time = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_list_ping = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_rb_gate = new System.Windows.Forms.RadioButton();
            this.m_rb_server = new System.Windows.Forms.RadioButton();
            this.m_rb_fight = new System.Windows.Forms.RadioButton();
            this.m_rb_world = new System.Windows.Forms.RadioButton();
            this.m_rb_global = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "服务器:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.m_btn_ok);
            this.groupBox1.Controls.Add(this.m_txt_time);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(519, 82);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "服务器选择";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(189, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "(单位:秒)";
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Font = new System.Drawing.Font("宋体", 14F);
            this.m_btn_ok.Location = new System.Drawing.Point(425, 17);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(90, 58);
            this.m_btn_ok.TabIndex = 13;
            this.m_btn_ok.Text = "开始";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_txt_time
            // 
            this.m_txt_time.Location = new System.Drawing.Point(63, 55);
            this.m_txt_time.Name = "m_txt_time";
            this.m_txt_time.Size = new System.Drawing.Size(120, 21);
            this.m_txt_time.TabIndex = 12;
            this.m_txt_time.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "间隔:";
            // 
            // m_list_ping
            // 
            this.m_list_ping.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.m_list_ping.FormattingEnabled = true;
            this.m_list_ping.ItemHeight = 16;
            this.m_list_ping.Location = new System.Drawing.Point(8, 3);
            this.m_list_ping.Name = "m_list_ping";
            this.m_list_ping.Size = new System.Drawing.Size(512, 148);
            this.m_list_ping.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_rb_global);
            this.panel1.Controls.Add(this.m_rb_world);
            this.panel1.Controls.Add(this.m_rb_fight);
            this.panel1.Controls.Add(this.m_rb_server);
            this.panel1.Controls.Add(this.m_rb_gate);
            this.panel1.Location = new System.Drawing.Point(63, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(356, 36);
            this.panel1.TabIndex = 16;
            // 
            // m_rb_gate
            // 
            this.m_rb_gate.AutoSize = true;
            this.m_rb_gate.Location = new System.Drawing.Point(4, 8);
            this.m_rb_gate.Name = "m_rb_gate";
            this.m_rb_gate.Size = new System.Drawing.Size(47, 16);
            this.m_rb_gate.TabIndex = 0;
            this.m_rb_gate.TabStop = true;
            this.m_rb_gate.Text = "Gate";
            this.m_rb_gate.UseVisualStyleBackColor = true;
            // 
            // m_rb_server
            // 
            this.m_rb_server.AutoSize = true;
            this.m_rb_server.Location = new System.Drawing.Point(73, 8);
            this.m_rb_server.Name = "m_rb_server";
            this.m_rb_server.Size = new System.Drawing.Size(59, 16);
            this.m_rb_server.TabIndex = 1;
            this.m_rb_server.TabStop = true;
            this.m_rb_server.Text = "Server";
            this.m_rb_server.UseVisualStyleBackColor = true;
            // 
            // m_rb_fight
            // 
            this.m_rb_fight.AutoSize = true;
            this.m_rb_fight.Location = new System.Drawing.Point(147, 8);
            this.m_rb_fight.Name = "m_rb_fight";
            this.m_rb_fight.Size = new System.Drawing.Size(53, 16);
            this.m_rb_fight.TabIndex = 2;
            this.m_rb_fight.TabStop = true;
            this.m_rb_fight.Text = "Fight";
            this.m_rb_fight.UseVisualStyleBackColor = true;
            // 
            // m_rb_world
            // 
            this.m_rb_world.AutoSize = true;
            this.m_rb_world.Location = new System.Drawing.Point(219, 8);
            this.m_rb_world.Name = "m_rb_world";
            this.m_rb_world.Size = new System.Drawing.Size(53, 16);
            this.m_rb_world.TabIndex = 3;
            this.m_rb_world.TabStop = true;
            this.m_rb_world.Text = "World";
            this.m_rb_world.UseVisualStyleBackColor = true;
            // 
            // m_rb_global
            // 
            this.m_rb_global.AutoSize = true;
            this.m_rb_global.Location = new System.Drawing.Point(292, 8);
            this.m_rb_global.Name = "m_rb_global";
            this.m_rb_global.Size = new System.Drawing.Size(59, 16);
            this.m_rb_global.TabIndex = 4;
            this.m_rb_global.TabStop = true;
            this.m_rb_global.Text = "Global";
            this.m_rb_global.UseVisualStyleBackColor = true;
            // 
            // PingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(523, 239);
            this.Controls.Add(this.m_list_ping);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ping网络";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox m_txt_time;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.ListBox m_list_ping;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton m_rb_world;
        private System.Windows.Forms.RadioButton m_rb_fight;
        private System.Windows.Forms.RadioButton m_rb_server;
        private System.Windows.Forms.RadioButton m_rb_gate;
        private System.Windows.Forms.RadioButton m_rb_global;

    }
}

