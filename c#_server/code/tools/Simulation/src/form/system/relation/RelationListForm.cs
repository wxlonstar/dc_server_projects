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
    /// 关系列表
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public partial class RelationListForm : Form
    {
        public RelationListForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            ServerMsgSend.SendRelationList();

            Dictionary<long, RelationInfo> list = RelationDataManager.Instance.relations;
            foreach (var obj in list)
            {
                RelationInfo info = obj.Value;
                m_list_relation.Rows.Add(info.char_idx, info.char_name, info.flags, info.level, info.char_type);
            }
        }
        /// <summary>
        /// 添加的账号id
        /// </summary>
        public void SetAddCharIdx(long char_idx)
        {
            m_add_type.SelectedIndex = 0;
            m_txt_value.Text = char_idx.ToString();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        /// <summary>
        /// 添加关系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnOK(object sender, EventArgs e)
        {
            string str_value = m_txt_value.Text.Trim();
            if (m_add_type.SelectedIndex < 0 || m_add_flag.SelectedIndex < 0 || str_value.Length == 0)
                return;

            RelationAddTarget target = new RelationAddTarget();
            if (m_add_type.SelectedIndex == 0)
            {
                target.type = eRelationAddType.Idx;
                if (!StringUtils.IsInteger(str_value)) return;
                target.char_idx = long.Parse(str_value);
            }
            else
            {
                target.type = eRelationAddType.Name;
                target.char_name = str_value;
            }

            eRelationFlag flag = eRelationFlag.Invalid;
            if (m_add_flag.SelectedIndex == 0)
                flag = eRelationFlag.Friend;
            else
                flag = eRelationFlag.Block;

            ServerMsgSend.SendRelationAdd( target, flag, m_txt_message.Text.Trim());

            this.Close();
        }
        /// <summary>
        /// 关系操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex; //获取当前列的索引

            if (col == 5)
            {
                ItemID item_id = new ItemID();
                item_id.type = eMainItemType.Currency;
                item_id.obj_type = (uint)eCurrencyType.Gold;
                item_id.obj_value = 1000;

                string idx = m_list_relation.Rows[row].Cells["Idx"].Value.ToString();
                long char_idx = long.Parse(idx);
                ServerMsgSend.SendRelationGive(char_idx, item_id);
            }
            else if (col == 6)
            {
                string idx = m_list_relation.Rows[row].Cells["Idx"].Value.ToString();
                long char_idx = long.Parse(idx);
                RelationAddTarget target_id = new RelationAddTarget();
                target_id.type = eRelationAddType.Idx;
                target_id.char_idx = char_idx;
                ServerMsgSend.SendRelationAdd(target_id, eRelationFlag.Block, "");
            }
            else if (col == 7)
            {
                string idx = m_list_relation.Rows[row].Cells["Idx"].Value.ToString();
                long char_idx = long.Parse(idx);
                m_list_relation.Rows.RemoveAt(row);
                RelationDataManager.Instance.RemoveRelation(char_idx);
                ServerMsgSend.SendRelationRemove(char_idx);
            }
        }
    }
}
