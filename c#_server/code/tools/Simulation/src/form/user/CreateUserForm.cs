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
    /// 创号界面
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public partial class CreateUserForm : Form
    {
        public CreateUserForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            this.m_txt_name.Text = "";
            this.m_com_sex.SelectedIndex = 0;

            //角色列表
            List<CharacterLogin> character_list = LoginDataMgr.Instance.character_list;
            foreach (var obj in character_list)
            {
                m_list_user.Items.Add(obj.char_name);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
       
        private void OnBtnOk(object sender, EventArgs e)
        {
            string user_name = this.m_txt_name.Text;
            eSexType sex = this.m_com_sex.SelectedIndex == 0 ? eSexType.BOY : eSexType.GIRL;

            ServerMsgSend.SendCreateCharacter(user_name, (uint)sex);

            this.Close();
        }
        private void OnBtnCancel(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 登录角色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUserDClick(object sender, MouseEventArgs e)
        {
            int index = m_list_user.SelectedIndex;
            if (index < 0) return;
            CharacterLogin char_data = LoginDataMgr.Instance.GetCharacterByIndex(index);
            ServerMsgSend.SendEnterGame(char_data.char_idx);
            ServerMsgSend.SendEnterScene(0);

            this.Close();
        }
    }
}
