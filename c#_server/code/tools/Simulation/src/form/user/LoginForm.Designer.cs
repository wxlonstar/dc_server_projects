namespace dc
{
    partial class LoginForm
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
            this.m_txt_ip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_btn_cancel = new System.Windows.Forms.Button();
            this.m_txt_psw = new System.Windows.Forms.TextBox();
            this.m_txt_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_txt_port = new dc.Compnent.RegTextBox();
            this.SuspendLayout();
            // 
            // m_txt_ip
            // 
            this.m_txt_ip.Location = new System.Drawing.Point(101, 38);
            this.m_txt_ip.Name = "m_txt_ip";
            this.m_txt_ip.Size = new System.Drawing.Size(138, 21);
            this.m_txt_ip.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器IP:";
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Location = new System.Drawing.Point(130, 124);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(75, 30);
            this.m_btn_ok.TabIndex = 4;
            this.m_btn_ok.Text = "确定";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOk);
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Location = new System.Drawing.Point(256, 124);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(75, 30);
            this.m_btn_cancel.TabIndex = 5;
            this.m_btn_cancel.Text = "取消";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            this.m_btn_cancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // m_txt_psw
            // 
            this.m_txt_psw.Location = new System.Drawing.Point(308, 75);
            this.m_txt_psw.Name = "m_txt_psw";
            this.m_txt_psw.Size = new System.Drawing.Size(110, 21);
            this.m_txt_psw.TabIndex = 9;
            this.m_txt_psw.UseSystemPasswordChar = true;
            // 
            // m_txt_name
            // 
            this.m_txt_name.Location = new System.Drawing.Point(101, 75);
            this.m_txt_name.Name = "m_txt_name";
            this.m_txt_name.Size = new System.Drawing.Size(138, 21);
            this.m_txt_name.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(60, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "账号:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(267, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "密码:";
            // 
            // m_txt_port
            // 
            this.m_txt_port.EmptyMessage = "不能为空";
            this.m_txt_port.ErrorMessage = "必须输入数字";
            this.m_txt_port.Location = new System.Drawing.Point(308, 38);
            this.m_txt_port.Name = "m_txt_port";
            this.m_txt_port.RegexExpression = "^[+-]?\\d*[.]?\\d*$";
            this.m_txt_port.Size = new System.Drawing.Size(110, 21);
            this.m_txt_port.TabIndex = 10;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(461, 175);
            this.Controls.Add(this.m_txt_port);
            this.Controls.Add(this.m_txt_psw);
            this.Controls.Add(this.m_txt_name);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_btn_cancel);
            this.Controls.Add(this.m_btn_ok);
            this.Controls.Add(this.m_txt_ip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_txt_ip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.TextBox m_txt_psw;
        private System.Windows.Forms.TextBox m_txt_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Compnent.RegTextBox m_txt_port;

    }
}

