namespace dc
{
    partial class LoggerForm
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
            this.m_list_log = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // m_list_log
            // 
            this.m_list_log.FormattingEnabled = true;
            this.m_list_log.ItemHeight = 12;
            this.m_list_log.Location = new System.Drawing.Point(1, 1);
            this.m_list_log.Name = "m_list_log";
            this.m_list_log.Size = new System.Drawing.Size(382, 760);
            this.m_list_log.TabIndex = 11;
            // 
            // LoggerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 762);
            this.Controls.Add(this.m_list_log);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoggerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "日志";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox m_list_log;

    }
}

