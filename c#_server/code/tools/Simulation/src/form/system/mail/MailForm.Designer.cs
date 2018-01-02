namespace dc
{
    partial class MailForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_txt_recv = new System.Windows.Forms.TextBox();
            this.m_list_mail = new System.Windows.Forms.DataGridView();
            this.Idx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Flags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Subject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Remove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_mail)).BeginInit();
            this.SuspendLayout();
            // 
            // m_txt_subject
            // 
            this.m_txt_subject.Location = new System.Drawing.Point(48, 14);
            this.m_txt_subject.Name = "m_txt_subject";
            this.m_txt_subject.Size = new System.Drawing.Size(280, 21);
            this.m_txt_subject.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "主题:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "收件方:";
            // 
            // m_txt_content
            // 
            this.m_txt_content.Location = new System.Drawing.Point(48, 41);
            this.m_txt_content.Multiline = true;
            this.m_txt_content.Name = "m_txt_content";
            this.m_txt_content.Size = new System.Drawing.Size(459, 81);
            this.m_txt_content.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "内容:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_btn_ok);
            this.groupBox1.Controls.Add(this.m_txt_recv);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.m_txt_content);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.m_txt_subject);
            this.groupBox1.Location = new System.Drawing.Point(1, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 128);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "写邮件";
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Font = new System.Drawing.Font("SimSun", 14F);
            this.m_btn_ok.Location = new System.Drawing.Point(511, 14);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(62, 108);
            this.m_btn_ok.TabIndex = 13;
            this.m_btn_ok.Text = "发送";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_txt_recv
            // 
            this.m_txt_recv.Location = new System.Drawing.Point(387, 14);
            this.m_txt_recv.Name = "m_txt_recv";
            this.m_txt_recv.Size = new System.Drawing.Size(120, 21);
            this.m_txt_recv.TabIndex = 12;
            // 
            // m_list_mail
            // 
            this.m_list_mail.AllowUserToAddRows = false;
            this.m_list_mail.AllowUserToResizeColumns = false;
            this.m_list_mail.AllowUserToResizeRows = false;
            this.m_list_mail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_list_mail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Idx,
            this.SendName,
            this.Type,
            this.Flags,
            this.Subject,
            this.Read,
            this.Remove});
            this.m_list_mail.Location = new System.Drawing.Point(1, 0);
            this.m_list_mail.Name = "m_list_mail";
            this.m_list_mail.ReadOnly = true;
            this.m_list_mail.RowHeadersVisible = false;
            this.m_list_mail.RowTemplate.Height = 23;
            this.m_list_mail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_list_mail.Size = new System.Drawing.Size(576, 150);
            this.m_list_mail.TabIndex = 13;
            this.m_list_mail.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellClick);
            // 
            // Idx
            // 
            this.Idx.HeaderText = "Idx";
            this.Idx.Name = "Idx";
            this.Idx.ReadOnly = true;
            this.Idx.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SendName
            // 
            this.SendName.HeaderText = "SendName";
            this.SendName.Name = "SendName";
            this.SendName.ReadOnly = true;
            this.SendName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Type.Width = 60;
            // 
            // Flags
            // 
            this.Flags.HeaderText = "Flags";
            this.Flags.Name = "Flags";
            this.Flags.ReadOnly = true;
            this.Flags.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Flags.Width = 60;
            // 
            // Subject
            // 
            this.Subject.HeaderText = "Subject";
            this.Subject.Name = "Subject";
            this.Subject.ReadOnly = true;
            this.Subject.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Subject.Width = 150;
            // 
            // Read
            // 
            this.Read.HeaderText = "查看";
            this.Read.Name = "Read";
            this.Read.ReadOnly = true;
            this.Read.Text = "查看";
            this.Read.UseColumnTextForButtonValue = true;
            this.Read.Width = 50;
            // 
            // Remove
            // 
            this.Remove.HeaderText = "删除";
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Text = "删除";
            this.Remove.UseColumnTextForButtonValue = true;
            this.Remove.Width = 50;
            // 
            // MailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(580, 283);
            this.Controls.Add(this.m_list_mail);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "邮件";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_mail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox m_txt_subject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_txt_content;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox m_txt_recv;
        private System.Windows.Forms.DataGridView m_list_mail;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn Idx;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Flags;
        private System.Windows.Forms.DataGridViewTextBoxColumn Subject;
        private System.Windows.Forms.DataGridViewButtonColumn Read;
        private System.Windows.Forms.DataGridViewButtonColumn Remove;

    }
}

