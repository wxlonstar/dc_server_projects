namespace dc
{
    partial class ChatForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.m_txt_content = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_comb_chat_type = new System.Windows.Forms.ComboBox();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_txt_recevier = new System.Windows.Forms.TextBox();
            this.m_list_mail = new System.Windows.Forms.DataGridView();
            this.Idx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChatType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Subject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_mail)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "频道:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "私聊:";
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
            this.groupBox1.Controls.Add(this.m_comb_chat_type);
            this.groupBox1.Controls.Add(this.m_btn_ok);
            this.groupBox1.Controls.Add(this.m_txt_recevier);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.m_txt_content);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 128);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "聊天";
            // 
            // m_comb_chat_type
            // 
            this.m_comb_chat_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_comb_chat_type.FormattingEnabled = true;
            this.m_comb_chat_type.Items.AddRange(new object[] {
            "私聊",
            "世界",
            "当前",
            "组队",
            "军团",
            "战场"});
            this.m_comb_chat_type.Location = new System.Drawing.Point(49, 14);
            this.m_comb_chat_type.Name = "m_comb_chat_type";
            this.m_comb_chat_type.Size = new System.Drawing.Size(121, 20);
            this.m_comb_chat_type.TabIndex = 14;
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Font = new System.Drawing.Font("宋体", 14F);
            this.m_btn_ok.Location = new System.Drawing.Point(511, 14);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(62, 108);
            this.m_btn_ok.TabIndex = 13;
            this.m_btn_ok.Text = "发送";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_txt_recevier
            // 
            this.m_txt_recevier.Location = new System.Drawing.Point(387, 14);
            this.m_txt_recevier.Name = "m_txt_recevier";
            this.m_txt_recevier.Size = new System.Drawing.Size(120, 21);
            this.m_txt_recevier.TabIndex = 12;
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
            this.ChatType,
            this.Subject});
            this.m_list_mail.Location = new System.Drawing.Point(1, 0);
            this.m_list_mail.Name = "m_list_mail";
            this.m_list_mail.ReadOnly = true;
            this.m_list_mail.RowHeadersVisible = false;
            this.m_list_mail.RowTemplate.Height = 23;
            this.m_list_mail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_list_mail.Size = new System.Drawing.Size(576, 150);
            this.m_list_mail.TabIndex = 13;
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
            // ChatType
            // 
            this.ChatType.HeaderText = "Type";
            this.ChatType.Name = "ChatType";
            this.ChatType.ReadOnly = true;
            this.ChatType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ChatType.Width = 80;
            // 
            // Subject
            // 
            this.Subject.HeaderText = "Content";
            this.Subject.Name = "Subject";
            this.Subject.ReadOnly = true;
            this.Subject.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Subject.Width = 290;
            // 
            // ChatForm
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
            this.Name = "ChatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "聊天";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_mail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_txt_content;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox m_txt_recevier;
        private System.Windows.Forms.DataGridView m_list_mail;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.ComboBox m_comb_chat_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn Idx;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChatType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Subject;

    }
}

