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
    /// 修改角色信息
    /// @author hannibal
    /// @time 2016-9-6
    /// </summary>
    public partial class ModifyUserInfoForm : Form
    {
        public ModifyUserInfoForm(long unit_idx)
        {
            InitializeComponent();
            Init(unit_idx);
        }
        private void Init(long unit_idx)
        {
            m_com_name.Items.Add("");
            m_com_name.Items.Add("性别");
            m_com_name.Items.Add("姓名");
            m_com_name.Items.Add("标记");
            m_com_name.Items.Add("场景");
            m_com_name.Items.Add("模型");
            m_com_name.Items.Add("职业");
            m_com_name.Items.Add("等级");
            m_com_name.Items.Add("经验");
            m_com_name.Items.Add("能量");
            m_com_name.Items.Add("点券");
            m_com_name.Items.Add("游戏币");
            m_com_name.Items.Add("生命");
            m_com_name.Items.Add("vip等级");
            m_com_name.Items.Add("vip标记");
            m_com_name.Items.Add("最后登录时间");
            m_com_name.Items.Add("最后登出时间");
            m_com_name.Items.Add("基础伤害");
            m_com_name.Items.Add("基础移动速度");
            m_com_name.Items.Add("基础能量");
            m_com_name.Items.Add("生命上限");
            m_com_name.Items.Add("能量上限");
            m_com_name.Items.Add("伤害");
            m_com_name.Items.Add("范围");
            m_com_name.Items.Add("移动速度");
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void OnBtnOk(object sender, EventArgs e)
        {
            if (m_com_name.SelectedIndex > 0 && m_txt_value.Text.Trim().Length > 0)
            {
                bool is_string = false;
                string value = m_txt_value.Text.Trim();
                eUnitModType type = (eUnitModType)m_com_name.SelectedIndex;
                switch (type)
                {
                    case eUnitModType.UMT_char_name:
                        is_string = true;
                        break;
                }
                if (is_string)
                {
                    ServerMsgSend.SendUnitModifyString(type, value);
                }
                else
                {
                    ServerMsgSend.SendUnitModifyInt(type, long.Parse(value));
                }
            }
            this.Close();
        }

        private void OnBtnCancel(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
