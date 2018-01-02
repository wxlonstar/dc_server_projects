namespace dc
{
    partial class UserInfoForm
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
            this.m_txt_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_com_sex = new System.Windows.Forms.ComboBox();
            this.m_txt_lv = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_txt_name
            // 
            this.m_txt_name.Location = new System.Drawing.Point(54, 12);
            this.m_txt_name.Name = "m_txt_name";
            this.m_txt_name.ReadOnly = true;
            this.m_txt_name.Size = new System.Drawing.Size(120, 21);
            this.m_txt_name.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "姓名:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(198, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "性别:";
            // 
            // m_com_sex
            // 
            this.m_com_sex.Enabled = false;
            this.m_com_sex.FormattingEnabled = true;
            this.m_com_sex.Items.AddRange(new object[] {
            "男",
            "女"});
            this.m_com_sex.Location = new System.Drawing.Point(239, 12);
            this.m_com_sex.Name = "m_com_sex";
            this.m_com_sex.Size = new System.Drawing.Size(95, 20);
            this.m_com_sex.TabIndex = 9;
            // 
            // m_txt_lv
            // 
            this.m_txt_lv.Location = new System.Drawing.Point(394, 12);
            this.m_txt_lv.Name = "m_txt_lv";
            this.m_txt_lv.ReadOnly = true;
            this.m_txt_lv.Size = new System.Drawing.Size(120, 21);
            this.m_txt_lv.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(353, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "等级:";
            // 
            // UserInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(526, 262);
            this.Controls.Add(this.m_txt_lv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_txt_name);
            this.Controls.Add(this.m_com_sex);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "角色信息";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_txt_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox m_com_sex;
        private System.Windows.Forms.TextBox m_txt_lv;
        private System.Windows.Forms.Label label1;

    }
}

