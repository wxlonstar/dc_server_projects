namespace dc
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.m_db_list = new System.Windows.Forms.ComboBox();
            this.m_table_name = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_field_name = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_bin_type = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.m_field_list = new System.Windows.Forms.CheckedListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.m_field_config_list = new System.Windows.Forms.CheckedListBox();
            this.m_key_type = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.m_txt_key = new System.Windows.Forms.TextBox();
            this.m_btn_find = new System.Windows.Forms.Button();
            this.m_find_result = new System.Windows.Forms.DataGridView();
            this.m_sql_type = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.m_btn_save = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_find_result)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库:";
            // 
            // m_db_list
            // 
            this.m_db_list.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_db_list.FormattingEnabled = true;
            this.m_db_list.Location = new System.Drawing.Point(65, 23);
            this.m_db_list.Name = "m_db_list";
            this.m_db_list.Size = new System.Drawing.Size(90, 20);
            this.m_db_list.TabIndex = 1;
            this.m_db_list.SelectedIndexChanged += new System.EventHandler(this.DBSelectedIndexChanged);
            // 
            // m_table_name
            // 
            this.m_table_name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_table_name.FormattingEnabled = true;
            this.m_table_name.Location = new System.Drawing.Point(209, 23);
            this.m_table_name.Name = "m_table_name";
            this.m_table_name.Size = new System.Drawing.Size(90, 20);
            this.m_table_name.TabIndex = 3;
            this.m_table_name.SelectedIndexChanged += new System.EventHandler(this.TableSelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "表名:";
            // 
            // m_field_name
            // 
            this.m_field_name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_field_name.FormattingEnabled = true;
            this.m_field_name.Location = new System.Drawing.Point(352, 23);
            this.m_field_name.Name = "m_field_name";
            this.m_field_name.Size = new System.Drawing.Size(90, 20);
            this.m_field_name.TabIndex = 5;
            this.m_field_name.SelectedValueChanged += new System.EventHandler(this.OnFieldNameSelectedValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(311, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "字段:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.m_bin_type);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.m_table_name);
            this.groupBox1.Controls.Add(this.m_field_name);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.m_db_list);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(1, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(596, 56);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "需要修改的字段";
            // 
            // m_bin_type
            // 
            this.m_bin_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_bin_type.FormattingEnabled = true;
            this.m_bin_type.Items.AddRange(new object[] {
            "binary",
            "varbinary",
            "bit",
            "tinyblob",
            "blob",
            "mediumblob",
            "longblob"});
            this.m_bin_type.Location = new System.Drawing.Point(495, 23);
            this.m_bin_type.Name = "m_bin_type";
            this.m_bin_type.Size = new System.Drawing.Size(90, 20);
            this.m_bin_type.TabIndex = 7;
            this.m_bin_type.SelectedIndexChanged += new System.EventHandler(this.OnBinTypeSelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(454, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 6;
            this.label8.Text = "类型:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.m_field_list);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.m_field_config_list);
            this.groupBox2.Location = new System.Drawing.Point(1, 75);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(596, 140);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "字段结构";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(537, 43);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(48, 58);
            this.button1.TabIndex = 16;
            this.button1.Text = "打开目录";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnEditConfig);
            // 
            // m_field_list
            // 
            this.m_field_list.CheckOnClick = true;
            this.m_field_list.FormattingEnabled = true;
            this.m_field_list.Location = new System.Drawing.Point(293, 18);
            this.m_field_list.Name = "m_field_list";
            this.m_field_list.Size = new System.Drawing.Size(233, 116);
            this.m_field_list.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(258, 68);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(23, 12);
            this.label9.TabIndex = 6;
            this.label9.Text = ">>>";
            // 
            // m_field_config_list
            // 
            this.m_field_config_list.CheckOnClick = true;
            this.m_field_config_list.FormattingEnabled = true;
            this.m_field_config_list.Location = new System.Drawing.Point(12, 19);
            this.m_field_config_list.Name = "m_field_config_list";
            this.m_field_config_list.Size = new System.Drawing.Size(234, 116);
            this.m_field_config_list.TabIndex = 0;
            this.m_field_config_list.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.OnFieldConfigItemCheck);
            this.m_field_config_list.SelectedIndexChanged += new System.EventHandler(this.OnFieldConfigSelectedIndexChanged);
            // 
            // m_key_type
            // 
            this.m_key_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_key_type.FormattingEnabled = true;
            this.m_key_type.Location = new System.Drawing.Point(61, 227);
            this.m_key_type.Name = "m_key_type";
            this.m_key_type.Size = new System.Drawing.Size(90, 20);
            this.m_key_type.TabIndex = 9;
            this.m_key_type.SelectedIndexChanged += new System.EventHandler(this.OnSqlKeySelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "查询键:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(176, 231);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "值:";
            // 
            // m_txt_key
            // 
            this.m_txt_key.Location = new System.Drawing.Point(205, 227);
            this.m_txt_key.Name = "m_txt_key";
            this.m_txt_key.Size = new System.Drawing.Size(86, 21);
            this.m_txt_key.TabIndex = 10;
            this.m_txt_key.TextChanged += new System.EventHandler(this.OnSqlKeyValueTextChanged);
            // 
            // m_btn_find
            // 
            this.m_btn_find.Location = new System.Drawing.Point(462, 222);
            this.m_btn_find.Name = "m_btn_find";
            this.m_btn_find.Size = new System.Drawing.Size(59, 29);
            this.m_btn_find.TabIndex = 11;
            this.m_btn_find.Text = "查询";
            this.m_btn_find.UseVisualStyleBackColor = true;
            this.m_btn_find.Click += new System.EventHandler(this.OnFindResult);
            // 
            // m_find_result
            // 
            this.m_find_result.AllowUserToAddRows = false;
            this.m_find_result.AllowUserToResizeColumns = false;
            this.m_find_result.AllowUserToResizeRows = false;
            this.m_find_result.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_find_result.Location = new System.Drawing.Point(10, 257);
            this.m_find_result.MultiSelect = false;
            this.m_find_result.Name = "m_find_result";
            this.m_find_result.RowHeadersVisible = false;
            this.m_find_result.RowTemplate.Height = 23;
            this.m_find_result.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.m_find_result.Size = new System.Drawing.Size(576, 150);
            this.m_find_result.TabIndex = 12;
            // 
            // m_sql_type
            // 
            this.m_sql_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_sql_type.FormattingEnabled = true;
            this.m_sql_type.Items.AddRange(new object[] {
            "数字",
            "字符串"});
            this.m_sql_type.Location = new System.Drawing.Point(358, 227);
            this.m_sql_type.Name = "m_sql_type";
            this.m_sql_type.Size = new System.Drawing.Size(80, 20);
            this.m_sql_type.TabIndex = 14;
            this.m_sql_type.SelectedIndexChanged += new System.EventHandler(this.OnSqlValueTypeSelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(311, 230);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "类型:";
            // 
            // m_btn_save
            // 
            this.m_btn_save.Location = new System.Drawing.Point(527, 222);
            this.m_btn_save.Name = "m_btn_save";
            this.m_btn_save.Size = new System.Drawing.Size(59, 29);
            this.m_btn_save.TabIndex = 15;
            this.m_btn_save.Text = "保存";
            this.m_btn_save.UseVisualStyleBackColor = true;
            this.m_btn_save.Click += new System.EventHandler(this.OnSaveResult);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 415);
            this.Controls.Add(this.m_btn_save);
            this.Controls.Add(this.m_sql_type);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.m_find_result);
            this.Controls.Add(this.m_btn_find);
            this.Controls.Add(this.m_txt_key);
            this.Controls.Add(this.m_key_type);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bin数据修改器";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_find_result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox m_db_list;
        private System.Windows.Forms.ComboBox m_table_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox m_field_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox m_field_config_list;
        private System.Windows.Forms.ComboBox m_key_type;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox m_txt_key;
        private System.Windows.Forms.Button m_btn_find;
        private System.Windows.Forms.DataGridView m_find_result;
        private System.Windows.Forms.ComboBox m_sql_type;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox m_bin_type;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button m_btn_save;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckedListBox m_field_list;
        private System.Windows.Forms.Button button1;
    }
}

