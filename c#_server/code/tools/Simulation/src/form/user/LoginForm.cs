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
    /// 登录界面
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            this.m_txt_ip.Text = ServerConfig.net_info.net_server_ip;
            this.m_txt_port.Text = ServerConfig.net_info.net_server_port.ToString();
            this.m_txt_name.Text = ServerConfig.net_info.user_name;
            this.m_txt_psw.Text = ServerConfig.net_info.user_psw;

            this.RegisterEvent();
        }
        protected override void OnClosed(EventArgs e)
        {
            this.UnRegisterEvent();

            base.OnClosed(e);
        }
       
        private void OnBtnOk(object sender, EventArgs e)
        {
            string ip = this.m_txt_ip.Text;
            ushort port = ushort.Parse(this.m_txt_port.Text);
            string user_name = this.m_txt_name.Text;
            string user_psw = this.m_txt_psw.Text;

            if (!SocketUtils.IsValidIP(ip)) return;

            ServerConfig.net_info.net_server_ip = ip;
            ServerConfig.net_info.net_server_port = port;
            ServerConfig.net_info.user_name = user_name;
            ServerConfig.net_info.user_psw = user_psw;

            EnableController(false);
            m_btn_ok.Enabled = false;
            m_btn_cancel.Enabled = false;

            if(ClientNetManager.Instance.cur_conn_idx > 0)
            {
                ClientNetManager.Instance.Close();
                TimerManager.Instance.AddOnce(1000, (timer_id, param) => 
                {
                    ClientNetManager.Instance.StartConnect(ip, port, user_name, user_psw);
                });
            }
            else
            {
                ClientNetManager.Instance.StartConnect(ip, port, user_name, user_psw);
            }
        }
        private void OnBtnCancel(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OnConnectedSucceed()
        {
            this.Close();
        }
        private void OnConnectedFailed()
        {
            if (m_txt_ip.InvokeRequired)
            {
                EnableController(true);
                m_btn_ok.Enabled = true;
                m_btn_cancel.Enabled = true;
            }
        }
        private void EnableController(bool b)
        {
            this.m_txt_ip.Enabled = b;
            this.m_txt_port.Enabled = b;
            this.m_txt_name.Enabled = b;
            this.m_txt_psw.Enabled = b;
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        this.OnConnectedSucceed();
                    }
                    break;

                case ClientEventID.NET_CONNECTED_CLOSE:
                    {
                        MessageBox.Show("无法连接到主机", "错误", MessageBoxButtons.OK);
                        this.OnConnectedFailed(); 
                    }
                    break;
            }
        }
    }
}
