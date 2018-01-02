namespace dc
{
    partial class ModifyUserInfoForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.m_com_name = new System.Windows.Forms.ComboBox();
            this.m_txt_value = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btn_cancel = new System.Windows.Forms.Button();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "属性:";
            // 
            // m_com_name
            // 
            this.m_com_name.FormattingEnabled = true;
            this.m_com_name.Location = new System.Drawing.Point(101, 12);
            this.m_com_name.Name = "m_com_name";
            this.m_com_name.Size = new System.Drawing.Size(95, 20);
            this.m_com_name.TabIndex = 9;
            // 
            // m_txt_value
            // 
            this.m_txt_value.Location = new System.Drawing.Point(256, 12);
            this.m_txt_value.Name = "m_txt_value";
            this.m_txt_value.Size = new System.Drawing.Size(120, 21);
            this.m_txt_value.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(215, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "值:";
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Location = new System.Drawing.Point(256, 60);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(75, 30);
            this.m_btn_cancel.TabIndex = 13;
            this.m_btn_cancel.Text = "取消";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            this.m_btn_cancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Location = new System.Drawing.Point(130, 60);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(75, 30);
            this.m_btn_ok.TabIndex = 12;
            this.m_btn_ok.Text = "确定";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOk);
            // 
            // ModifyUserInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(449, 114);
            this.Controls.Add(this.m_btn_cancel);
            this.Controls.Add(this.m_btn_ok);
            this.Controls.Add(this.m_txt_value);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_com_name);
            this.Controls.Add(this.label4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModifyUserInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改属性";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox m_com_name;
        private System.Windows.Forms.TextBox m_txt_value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.Button m_btn_ok;

    }
}

