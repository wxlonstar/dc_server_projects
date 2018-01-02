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
    public partial class RelationAddForm : Form
    {
        public RelationAddForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            List<RelationAddInfo> list = RelationDataManager.Instance.apply_relations;
            foreach (var obj in list)
            {
                m_list_relation.Rows.Add(obj.event_idx, obj.player_id.char_idx, obj.player_id.char_name, obj.message);
            }
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

            long event_idx = 0;
            long char_idx = 0;
            eRelationApplyCmd cmd = eRelationApplyCmd.Agree;
            if (col == 4)
            {
                string idx = m_list_relation.Rows[row].Cells["Idx"].Value.ToString();
                event_idx = long.Parse(idx);
                string str_char_idx = m_list_relation.Rows[row].Cells["CharIdx"].Value.ToString();
                char_idx = long.Parse(str_char_idx);
                cmd = eRelationApplyCmd.Agree;
            }
            else if (col == 5)
            {
                string idx = m_list_relation.Rows[row].Cells["Idx"].Value.ToString();
                event_idx = long.Parse(idx);
                string str_char_idx = m_list_relation.Rows[row].Cells["CharIdx"].Value.ToString();
                char_idx = long.Parse(str_char_idx);
                cmd = eRelationApplyCmd.Reject;
            }
            m_list_relation.Rows.RemoveAt(row);
            RelationDataManager.Instance.RemoveNewApplys(event_idx);

            ServerMsgSend.SendRelationApplyCommand(event_idx, char_idx, cmd);
        }
    }
}
