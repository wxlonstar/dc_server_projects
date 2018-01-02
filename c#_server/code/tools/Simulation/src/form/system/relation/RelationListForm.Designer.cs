namespace dc
{
    partial class RelationListForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.m_txt_message = new System.Windows.Forms.TextBox();
            this.m_add_type = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_add_flag = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btn_ok = new System.Windows.Forms.Button();
            this.m_txt_value = new System.Windows.Forms.TextBox();
            this.m_list_relation = new System.Windows.Forms.DataGridView();
            this.Idx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FriendName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Flag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Block = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Remove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_relation)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(178, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "姓名:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.m_txt_message);
            this.groupBox1.Controls.Add(this.m_add_type);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.m_add_flag);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.m_btn_ok);
            this.groupBox1.Controls.Add(this.m_txt_value);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(1, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(576, 77);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "添加好友";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 19;
            this.label3.Text = "留言:";
            // 
            // m_txt_message
            // 
            this.m_txt_message.Location = new System.Drawing.Point(61, 45);
            this.m_txt_message.Name = "m_txt_message";
            this.m_txt_message.Size = new System.Drawing.Size(432, 21);
            this.m_txt_message.TabIndex = 18;
            // 
            // m_add_type
            // 
            this.m_add_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_add_type.FormattingEnabled = true;
            this.m_add_type.Items.AddRange(new object[] {
            "ID",
            "姓名"});
            this.m_add_type.Location = new System.Drawing.Point(61, 18);
            this.m_add_type.Name = "m_add_type";
            this.m_add_type.Size = new System.Drawing.Size(85, 20);
            this.m_add_type.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "方式:";
            // 
            // m_add_flag
            // 
            this.m_add_flag.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_add_flag.FormattingEnabled = true;
            this.m_add_flag.Items.AddRange(new object[] {
            "好友",
            "黑名单"});
            this.m_add_flag.Location = new System.Drawing.Point(408, 18);
            this.m_add_flag.Name = "m_add_flag";
            this.m_add_flag.Size = new System.Drawing.Size(85, 20);
            this.m_add_flag.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(366, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "关系:";
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Font = new System.Drawing.Font("宋体", 14F);
            this.m_btn_ok.Location = new System.Drawing.Point(508, 18);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(62, 48);
            this.m_btn_ok.TabIndex = 13;
            this.m_btn_ok.Text = "添加";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOK);
            // 
            // m_txt_value
            // 
            this.m_txt_value.Location = new System.Drawing.Point(219, 18);
            this.m_txt_value.Name = "m_txt_value";
            this.m_txt_value.Size = new System.Drawing.Size(120, 21);
            this.m_txt_value.TabIndex = 12;
            // 
            // m_list_relation
            // 
            this.m_list_relation.AllowUserToAddRows = false;
            this.m_list_relation.AllowUserToResizeColumns = false;
            this.m_list_relation.AllowUserToResizeRows = false;
            this.m_list_relation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_list_relation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Idx,
            this.FriendName,
            this.Flag,
            this.Level,
            this.Sex,
            this.Read,
            this.Block,
            this.Remove});
            this.m_list_relation.Location = new System.Drawing.Point(1, 0);
            this.m_list_relation.Name = "m_list_relation";
            this.m_list_relation.ReadOnly = true;
            this.m_list_relation.RowHeadersVisible = false;
            this.m_list_relation.RowTemplate.Height = 23;
            this.m_list_relation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_list_relation.Size = new System.Drawing.Size(576, 150);
            this.m_list_relation.TabIndex = 13;
            this.m_list_relation.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellClick);
            // 
            // Idx
            // 
            this.Idx.HeaderText = "Idx";
            this.Idx.Name = "Idx";
            this.Idx.ReadOnly = true;
            this.Idx.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FriendName
            // 
            this.FriendName.HeaderText = "姓名";
            this.FriendName.Name = "FriendName";
            this.FriendName.ReadOnly = true;
            this.FriendName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FriendName.Width = 110;
            // 
            // Flag
            // 
            this.Flag.HeaderText = "关系";
            this.Flag.Name = "Flag";
            this.Flag.ReadOnly = true;
            this.Flag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Flag.Width = 60;
            // 
            // Level
            // 
            this.Level.HeaderText = "等级";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Level.Width = 60;
            // 
            // Sex
            // 
            this.Sex.HeaderText = "性别";
            this.Sex.Name = "Sex";
            this.Sex.ReadOnly = true;
            this.Sex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Sex.Width = 60;
            // 
            // Read
            // 
            this.Read.HeaderText = "赠送";
            this.Read.Name = "Read";
            this.Read.ReadOnly = true;
            this.Read.Text = "赠送";
            this.Read.UseColumnTextForButtonValue = true;
            this.Read.Width = 60;
            // 
            // Block
            // 
            this.Block.HeaderText = "拉黑";
            this.Block.Name = "Block";
            this.Block.ReadOnly = true;
            this.Block.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Block.Text = "拉黑";
            this.Block.UseColumnTextForButtonValue = true;
            this.Block.Width = 60;
            // 
            // Remove
            // 
            this.Remove.HeaderText = "删除";
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Text = "删除";
            this.Remove.UseColumnTextForButtonValue = true;
            this.Remove.Width = 60;
            // 
            // RelationListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(580, 245);
            this.Controls.Add(this.m_list_relation);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RelationListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "好友";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_relation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox m_txt_value;
        private System.Windows.Forms.DataGridView m_list_relation;
        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.ComboBox m_add_flag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox m_add_type;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox m_txt_message;
        private System.Windows.Forms.DataGridViewTextBoxColumn Idx;
        private System.Windows.Forms.DataGridViewTextBoxColumn FriendName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Flag;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sex;
        private System.Windows.Forms.DataGridViewButtonColumn Read;
        private System.Windows.Forms.DataGridViewButtonColumn Block;
        private System.Windows.Forms.DataGridViewButtonColumn Remove;

    }
}

