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
    /// 邮件
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public partial class MailForm : Form
    {
        public MailForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            List<MailTitleInfo> list = MailDataManager.Instance.mail_titles;
            foreach(var obj in list)
            {
                m_list_mail.Rows.Add(obj.mail_idx, obj.mail_type.ToString(), obj.sender_name, obj.flags, obj.subject);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnOK(object sender, EventArgs e)
        {
            MailWriteInfo info = new MailWriteInfo();
            //info.receiver_type = MailWriteInfo.eReceiverType.CHARID;
            //info.receiver_idx = long.Parse(m_txt_recv.Text);
            info.receiver.SetName(m_txt_recv.Text);
            info.subject = m_txt_subject.Text;
            info.content = m_txt_content.Text;
            ServerMsgSend.SendWriteMail(info);
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex; //获取当前列的索引
            if (col == 5)
            {
                string idx = m_list_mail.Rows[row].Cells["Idx"].Value.ToString();
                ServerMsgSend.SendReadMail(long.Parse(idx));
            }
            else if (col == 6)
            {
                if(MessageBox.Show("确定要删除邮件吗?", "信息", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string idx = m_list_mail.Rows[row].Cells["Idx"].Value.ToString();
                    ServerMsgSend.SendDeleteMail(long.Parse(idx));

                    m_list_mail.Rows.RemoveAt(row);
                }
            }
        }
    }
}
