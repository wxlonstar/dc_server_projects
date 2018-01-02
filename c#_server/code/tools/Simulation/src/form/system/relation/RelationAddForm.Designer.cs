namespace dc
{
    partial class RelationAddForm
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
            this.m_list_relation = new System.Windows.Forms.DataGridView();
            this.Idx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CharIdx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FriendName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Read = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Remove = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.m_list_relation)).BeginInit();
            this.SuspendLayout();
            // 
            // m_list_relation
            // 
            this.m_list_relation.AllowUserToAddRows = false;
            this.m_list_relation.AllowUserToResizeColumns = false;
            this.m_list_relation.AllowUserToResizeRows = false;
            this.m_list_relation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_list_relation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Idx,
            this.CharIdx,
            this.FriendName,
            this.message,
            this.Read,
            this.Remove});
            this.m_list_relation.Location = new System.Drawing.Point(1, 0);
            this.m_list_relation.Name = "m_list_relation";
            this.m_list_relation.ReadOnly = true;
            this.m_list_relation.RowHeadersVisible = false;
            this.m_list_relation.RowTemplate.Height = 23;
            this.m_list_relation.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_list_relation.Size = new System.Drawing.Size(626, 244);
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
            // CharIdx
            // 
            this.CharIdx.HeaderText = "CharIdx";
            this.CharIdx.Name = "CharIdx";
            this.CharIdx.ReadOnly = true;
            this.CharIdx.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FriendName
            // 
            this.FriendName.HeaderText = "姓名";
            this.FriendName.Name = "FriendName";
            this.FriendName.ReadOnly = true;
            this.FriendName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // message
            // 
            this.message.HeaderText = "留言";
            this.message.Name = "message";
            this.message.ReadOnly = true;
            this.message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.message.Width = 220;
            // 
            // Read
            // 
            this.Read.HeaderText = "同意";
            this.Read.Name = "Read";
            this.Read.ReadOnly = true;
            this.Read.Text = "同意";
            this.Read.UseColumnTextForButtonValue = true;
            this.Read.Width = 50;
            // 
            // Remove
            // 
            this.Remove.HeaderText = "拒绝";
            this.Remove.Name = "Remove";
            this.Remove.ReadOnly = true;
            this.Remove.Text = "拒绝";
            this.Remove.UseColumnTextForButtonValue = true;
            this.Remove.Width = 50;
            // 
            // RelationAddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(628, 245);
            this.Controls.Add(this.m_list_relation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RelationAddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "好友";
            ((System.ComponentModel.ISupportInitialize)(this.m_list_relation)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView m_list_relation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Idx;
        private System.Windows.Forms.DataGridViewTextBoxColumn CharIdx;
        private System.Windows.Forms.DataGridViewTextBoxColumn FriendName;
        private System.Windows.Forms.DataGridViewTextBoxColumn message;
        private System.Windows.Forms.DataGridViewButtonColumn Read;
        private System.Windows.Forms.DataGridViewButtonColumn Remove;

    }
}

