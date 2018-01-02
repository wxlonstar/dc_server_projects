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
    /// 绘制角色界面：作为子界面，嵌入到主界面
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public partial class DrawUserForm : Form
    {
        private Timer m_timer;

        private static int MAX_COUNT = 1000;
        private int m_cur_rect_count = 0;
        private UnitRectInfo[] m_list_unit_rect = new UnitRectInfo[MAX_COUNT];

        private long m_right_menu_unit_idx = 0;

        public DrawUserForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += Update;
            m_timer.Start();

            for(int i = 0; i < m_list_unit_rect.Length; ++i)
            {
                m_list_unit_rect[i] = new UnitRectInfo();
            }
        }
        private void Update(object sender, EventArgs e)
        {
            m_pic_root.Refresh();
            lock(ThreadScheduler.Instance.LogicLock)
            {
                this.DrawUnit();
            }
        }

        private void DrawUnit()
        {
            m_cur_rect_count = 0;
            using (Graphics gra = this.m_pic_root.CreateGraphics())
            {
                gra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Unit main_unit = null;
                using (Brush bush = new SolidBrush(Color.Green))
                {
                    Unit unit = null;
                    Dictionary<long, Unit> units = UnitManager.Instance.units;
                    foreach (var obj in units)
                    {
                        if (m_cur_rect_count >= m_list_unit_rect.Length) break;

                        unit = obj.Value;
                        if(unit.obj_idx == PlayerDataMgr.Instance.main_player_id)
                        {
                            main_unit = unit;
                            continue;
                        }

                        int x = 10 * unit.pos.x + 500;
                        int y = 10 * unit.pos.y + 500;
                        gra.FillEllipse(bush, x, y, 10, 10);

                        UnitRectInfo unit_info = m_list_unit_rect[m_cur_rect_count];
                        unit_info.unit_idx = unit.obj_idx;
                        unit_info.rect.X = x;
                        unit_info.rect.Y = y;
                        unit_info.rect.Width = 10;
                        unit_info.rect.Height = 10;
                        ++m_cur_rect_count;
                    }
                }
                if (main_unit != null)
                {
                    using (Brush bush = new SolidBrush(Color.Red))
                    {
                        if (m_cur_rect_count >= m_list_unit_rect.Length) return;

                        int x = 10 * main_unit.pos.x + 500;
                        int y = 10 * main_unit.pos.y + 500;
                        gra.FillEllipse(bush, x, y, 10, 10);

                        UnitRectInfo unit_info = m_list_unit_rect[m_cur_rect_count];
                        unit_info.unit_idx = main_unit.obj_idx;
                        unit_info.rect.X = x;
                        unit_info.rect.Y = y;
                        unit_info.rect.Width = 10;
                        unit_info.rect.Height = 10;
                        ++m_cur_rect_count;
                    }
                }
            }
        }
        private void OnPicMouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Right)
            {
            }
        }
        private void OnPicMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Right)
            {
            }
        }
        private void OnPicMouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < m_cur_rect_count; ++i)
                {
                    Rectangle rect = m_list_unit_rect[i].rect;
                    if (rect.Contains(e.Location))
                    {
                        m_right_menu_unit_idx = m_list_unit_rect[i].unit_idx;
                        mainContextMenuStrip.Show(this, e.Location);
                        return;
                    }
                }

                //移动
                Unit main_unit = UnitManager.Instance.GetUnitByIdx(PlayerDataMgr.Instance.main_player_id);
                if(main_unit != null)
                {
                    int new_x, new_y;
                    int x = e.Location.X - 500;
                    int y = e.Location.Y - 500;
                    if (x > main_unit.pos.x * 10)
                        new_x = main_unit.pos.x + 1;
                    else
                        new_x = main_unit.pos.x - 1;
                    if (y > main_unit.pos.y * 10)
                        new_y = main_unit.pos.y + 1;
                    else
                        new_y = main_unit.pos.y - 1;
                    main_unit.ModifyPos(new_x, new_y);
                    ServerMsgSend.SendUnitMove(new_x, new_y);
                }
            }
        }
        #region 菜单
        /// <summary>
        /// 查看
        /// </summary>
        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(m_right_menu_unit_idx > 0)
            {
                UserInfoForm info_form = new UserInfoForm(m_right_menu_unit_idx);
                info_form.ShowDialog(this);
            }
        }
        /// <summary>
        /// 踢下线
        /// </summary>
        private void KillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_right_menu_unit_idx > 0)
            {
            }
        }
        /// <summary>
        /// 加好友
        /// </summary>
        private void AddFriendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_right_menu_unit_idx > 0)
            {
                RelationListForm relation_form = new RelationListForm();
                relation_form.SetAddCharIdx(m_right_menu_unit_idx);
                relation_form.ShowDialog(this);
            }
        }
        /// <summary>
        /// 私聊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_right_menu_unit_idx > 0)
            {
                ChatForm chat_form = new ChatForm();
                chat_form.SetAddCharIdx(m_right_menu_unit_idx);
                chat_form.ShowDialog(this);
            }
        }
        #endregion

    }

    class UnitRectInfo
    {
        public long unit_idx = 0;
        public Rectangle rect = new Rectangle();
    }
}
