namespace dc
{
    partial class ChangeSceneForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.m_txt_scene = new dc.Compnent.RegTextBox();
            this.SuspendLayout();
            // 
            // m_btn_ok
            // 
            this.m_btn_ok.Location = new System.Drawing.Point(116, 56);
            this.m_btn_ok.Name = "m_btn_ok";
            this.m_btn_ok.Size = new System.Drawing.Size(75, 30);
            this.m_btn_ok.TabIndex = 4;
            this.m_btn_ok.Text = "确定";
            this.m_btn_ok.UseVisualStyleBackColor = true;
            this.m_btn_ok.Click += new System.EventHandler(this.OnBtnOk);
            // 
            // m_btn_cancel
            // 
            this.m_btn_cancel.Location = new System.Drawing.Point(242, 56);
            this.m_btn_cancel.Name = "m_btn_cancel";
            this.m_btn_cancel.Size = new System.Drawing.Size(75, 30);
            this.m_btn_cancel.TabIndex = 5;
            this.m_btn_cancel.Text = "取消";
            this.m_btn_cancel.UseVisualStyleBackColor = true;
            this.m_btn_cancel.Click += new System.EventHandler(this.OnBtnCancel);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "输入场景:";
            // 
            // m_txt_scene
            // 
            this.m_txt_scene.AllowEmpty = false;
            this.m_txt_scene.ErrorMessage = "必须输入数字";
            this.m_txt_scene.Location = new System.Drawing.Point(180, 12);
            this.m_txt_scene.Name = "m_txt_scene";
            this.m_txt_scene.RegexExpression = "^[+-]?\\d*[.]?\\d*$";
            this.m_txt_scene.Size = new System.Drawing.Size(137, 21);
            this.m_txt_scene.TabIndex = 8;
            // 
            // ChangeSceneForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(426, 111);
            this.Controls.Add(this.m_txt_scene);
            this.Controls.Add(this.m_btn_ok);
            this.Controls.Add(this.m_btn_cancel);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeSceneForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "切换场景";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btn_ok;
        private System.Windows.Forms.Button m_btn_cancel;
        private System.Windows.Forms.Label label3;
        private dc.Compnent.RegTextBox m_txt_scene;

    }
}

