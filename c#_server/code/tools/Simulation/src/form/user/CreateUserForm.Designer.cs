namespace dc
{
    partial class CreateUserForm
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
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_btn_cancel = new System.Windows.Forms.Button();
            this.m_txt_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.m_com_sex = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_list_user = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Location = new System.Drawing.Point(154, 54);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(75, 30);
            this.m_btn_ok.TabIndex = 4;
            this.m_btn_ok.Text = "确定";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOk);
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Location = new System.Drawing.Point(280, 54);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(75, 30);
            this.m_btn_cancel.TabIndex = 5;
            this.m_btn_cancel.Text = "取消";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            this.m_btn_cancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // m_txt_name
            // 
            this.m_txt_name.Location = new System.Drawing.Point(123, 20);
            this.m_txt_name.Name = "m_txt_name";
            this.m_txt_name.Size = new System.Drawing.Size(138, 21);
            this.m_txt_name.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(82, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "姓名:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(289, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "性别:";
            // 
            // m_com_sex
            // 
            this.m_com_sex.FormattingEnabled = true;
            this.m_com_sex.Items.AddRange(new object[] {
            "男",
            "女"});
            this.m_com_sex.Location = new System.Drawing.Point(330, 20);
            this.m_com_sex.Name = "m_com_sex";
            this.m_com_sex.Size = new System.Drawing.Size(95, 20);
            this.m_com_sex.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_txt_name);
            this.groupBox1.Controls.Add(this.m_com_sex);
            this.groupBox1.Controls.Add(this.m_btn_ok);
            this.groupBox1.Controls.Add(this.m_btn_cancel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(1, 173);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 88);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "创号";
            // 
            // m_list_user
            // 
            this.m_list_user.FormattingEnabled = true;
            this.m_list_user.ItemHeight = 12;
            this.m_list_user.Location = new System.Drawing.Point(1, 1);
            this.m_list_user.Name = "m_list_user";
            this.m_list_user.Size = new System.Drawing.Size(524, 160);
            this.m_list_user.TabIndex = 11;
            this.m_list_user.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnUserDClick);
            // 
            // CreateUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(526, 262);
            this.Controls.Add(this.m_list_user);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateUserForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "角色";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.TextBox m_txt_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox m_com_sex;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox m_list_user;

    }
}

