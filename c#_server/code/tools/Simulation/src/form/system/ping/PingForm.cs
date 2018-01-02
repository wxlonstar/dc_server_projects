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
    /// Ping网络
    /// @author hannibal
    /// @time 2017-9-11
    /// </summary>
    public partial class PingForm : Form
    {
        public PingForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            EventController.AddEventListener(ClientEventID.SERVER_PING, OnRecvPing);
        }
        protected override void OnClosed(EventArgs e)
        {
            EventController.RemoveEventListener(ClientEventID.SERVER_PING, OnRecvPing);
            PingDataManager.Instance.Stop();
            base.OnClosed(e);
        }
        private void OnRecvPing(GameEvent evt)
        {
            eServerType server_type = evt.Get<eServerType>(0);
            uint packet_id = evt.Get<uint>(1);
            long tick = evt.Get<long>(2);
            long offset_time = evt.Get<long>(3);
            uint flags = evt.Get<uint>(4);

            string str = string.Format("{0} ID:{1}, 时间:{2}, 延迟:{3}, 标记:{4}",server_type, packet_id, tick, offset_time, flags);
            m_list_ping.Items.Add(str);
            if (m_list_ping.Items.Count > 8)
                m_list_ping.Items.RemoveAt(0);
        }
        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBtnOK(object sender, EventArgs e)
        {
            if(m_btn_ok.Text == "开始")
            {
                uint ping_type = 0;
                if (m_rb_gate.Checked)
                    ping_type |= (uint)ePingType.Gate;
                else if (m_rb_server.Checked)
                    ping_type |= (uint)ePingType.Server;
                else if (m_rb_fight.Checked)
                    ping_type |= (uint)ePingType.Fight;
                else if (m_rb_world.Checked)
                    ping_type |= (uint)ePingType.World;
                else if (m_rb_global.Checked)
                    ping_type |= (uint)ePingType.Global;

                if (ping_type == 0) return;

                if (m_txt_time.Text.Trim() == "") return;
                int time_offset = 0;
                int.TryParse(m_txt_time.Text.Trim(), out time_offset);
                if (time_offset <= 0) return;

                PingDataManager.Instance.Start(ping_type, time_offset);

                this.EnableControl(false);
            }
            else
            {
                PingDataManager.Instance.Stop();

                this.EnableControl(true);
            }
        }
        private void EnableControl(bool b)
        {
            m_rb_gate.Enabled = b;
            m_rb_server.Enabled = b;
            m_rb_fight.Enabled = b;
            m_rb_world.Enabled = b;
            m_rb_global.Enabled = b;
            m_txt_time.Enabled = b;
            if (b) 
                m_btn_ok.Text = "开始";
            else
                m_btn_ok.Text = "停止";
        }
    }
}
