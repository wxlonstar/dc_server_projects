namespace dc
{
    partial class MailContentForm
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
            this.m_txt_subject = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_txt_content = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_txt_send = new System.Windows.Forms.TextBox();
            this.m_btn_cancel = new System.Windows.Forms.Button();
            this.panel = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // m_txt_subject
            // 
            this.m_txt_subject.Location = new System.Drawing.Point(48, 12);
            this.m_txt_subject.Name = "m_txt_subject";
            this.m_txt_subject.ReadOnly = true;
            this.m_txt_subject.Size = new System.Drawing.Size(280, 21);
            this.m_txt_subject.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "主题:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "发件人:";
            // 
            // m_txt_content
            // 
            this.m_txt_content.Location = new System.Drawing.Point(48, 39);
            this.m_txt_content.Multiline = true;
            this.m_txt_content.Name = "m_txt_content";
            this.m_txt_content.ReadOnly = true;
            this.m_txt_content.Size = new System.Drawing.Size(459, 81);
            this.m_txt_content.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "内容:";
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Font = new System.Drawing.Font("SimSun", 14F);
            this.m_btn_ok.Location = new System.Drawing.Point(169, 240);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(62, 31);
            this.m_btn_ok.TabIndex = 13;
            this.m_btn_ok.Text = "收取";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_txt_send
            // 
            this.m_txt_send.Location = new System.Drawing.Point(387, 12);
            this.m_txt_send.Name = "m_txt_send";
            this.m_txt_send.ReadOnly = true;
            this.m_txt_send.Size = new System.Drawing.Size(120, 21);
            this.m_txt_send.TabIndex = 12;
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Font = new System.Drawing.Font("SimSun", 14F);
            this.m_btn_cancel.Location = new System.Drawing.Point(281, 240);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(62, 31);
            this.m_btn_cancel.TabIndex = 14;
            this.m_btn_cancel.Text = "关闭";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            this.m_btn_cancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(9, 126);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(498, 101);
            this.panel.TabIndex = 15;
            this.panel.TabStop = false;
            this.panel.Text = "附件";
            // 
            // MailContentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(511, 283);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.m_btn_cancel);
            this.Controls.Add(this.m_btn_ok);
            this.Controls.Add(this.m_txt_send);
            this.Controls.Add(this.m_txt_content);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_txt_subject);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailContentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "邮件详情";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_txt_subject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_txt_content;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox m_txt_send;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.GroupBox panel;

    }
}

