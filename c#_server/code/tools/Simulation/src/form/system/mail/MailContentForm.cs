using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 邮件详情
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public partial class MailContentForm : Form
    {
        public MailContentForm(MailInfo mail_info)
        {
            InitializeComponent();
            Init(mail_info);
        }
        private void Init(MailInfo mail_info)
        {
            this.m_txt_subject.Text = mail_info.subject.ToString();
            this.m_txt_send.Text = mail_info.sender_idx.ToString();
            switch(mail_info.bin_mail_content.content_type)
            {
                case eMailContentType.NORMAL:
                    this.m_txt_content.Text = mail_info.bin_mail_content.bin_normal_content.content.ToString();
                    break;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        private void OnBtnOK(object sender, EventArgs e)
        {
            
        }
        private void OnBtnCancel(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
