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
    public partial class ChatForm : Form
    {
        public ChatForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            List<ChatInfo> list = ChatDataManager.Instance.chat_infoes;
            foreach (var obj in list)
            {
                m_list_mail.Rows.Add(obj.sender.char_idx.ToString(), obj.sender.char_name, obj.type, obj.content);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        /// <summary>
        /// 添加的账号id
        /// </summary>
        public void SetAddCharIdx(long char_idx)
        {
            m_comb_chat_type.SelectedIndex = 0;
            m_txt_recevier.Text = char_idx.ToString();
        }
        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnOK(object sender, EventArgs e)
        {
            int select_idx = m_comb_chat_type.SelectedIndex;
            if(select_idx >= 0 && m_txt_content.Text.Length > 0)
            {
                eChatType type = (eChatType)(select_idx + 2);
                string content = m_txt_content.Text;
                long receiver = 0;
                if(type == eChatType.PRIVATE)
                {
                    receiver = long.Parse(m_txt_recevier.Text);
                }
                ServerMsgSend.SendChat(type, receiver, content);
            }
        }
    }
}
