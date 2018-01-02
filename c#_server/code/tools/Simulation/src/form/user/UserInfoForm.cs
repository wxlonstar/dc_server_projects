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
    /// 角色信息
    /// @author hannibal
    /// @time 2016-8-30
    /// </summary>
    public partial class UserInfoForm : Form
    {
        public UserInfoForm(long unit_idx)
        {
            InitializeComponent();
            Init(unit_idx);
        }
        private void Init(long unit_idx)
        {
            PlayerInfoForClient info = PlayerDataMgr.Instance.main_player_info;
            Unit unit = UnitManager.Instance.GetUnitByIdx(unit_idx);
            if(unit != null)
            {
                if(unit is Player)
                {
                    Player player = unit as Player;
                    this.m_txt_name.Text = player.char_name;
                    this.m_com_sex.SelectedIndex = player.char_type == (byte)eSexType.BOY ? 0 : 1;
                    this.m_txt_lv.Text = player.level.ToString();
                }
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
