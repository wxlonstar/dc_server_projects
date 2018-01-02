using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dc
{
    public partial class MainForm : Form
    {
        private Timer m_timer;
        private ePressureType m_pressure_type = ePressureType.None;
        private List<TabPage> m_tab_pages = new List<TabPage>();

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            for (int i = 0; i < m_tabControl.TabCount; ++i)
            {
                TabPage tp = m_tabControl.TabPages[i];
                m_tab_pages.Add(tp);
            }

            this.m_tabNet_IP.Text = ServerConfig.net_info.net_server_ip;
            this.m_tabNet_Port.Text = ServerConfig.net_info.net_server_port.ToString();
            this.m_tabNet_ClientCount.Text = ServerConfig.net_info.net_client_count.ToString();
            this.m_tabNet_SendCountPerSecond.Text = ServerConfig.net_info.net_send_count.ToString();
            this.m_tabNet_SendSizePerSecond.Text = ServerConfig.net_info.net_send_size.ToString();

            this.m_tabLogin_IP.Text = ServerConfig.net_info.login_server_ip;
            this.m_tabLogin_Port.Text = ServerConfig.net_info.login_server_port.ToString();
            this.m_tabLogin_ClientCount.Text = ServerConfig.net_info.login_client_count.ToString();
            this.m_tabLogin_DisconTime.Text = ServerConfig.net_info.login_dis_time.ToString();

            this.m_tabMove_IP.Text = ServerConfig.net_info.move_server_ip;
            this.m_tabMove_Port.Text = ServerConfig.net_info.move_server_port.ToString();
            this.m_tabMove_ClientCount.Text = ServerConfig.net_info.move_client_count.ToString();
            this.m_tabMove_StartAccount.Text = ServerConfig.net_info.move_start_account.ToString();
            this.m_tabMove_MoveTime.Text = ServerConfig.net_info.move_time.ToString();

            this.m_tabDB_IP.Text = ServerConfig.net_info.db_server_ip;
            this.m_tabDB_Port.Text = ServerConfig.net_info.db_server_port.ToString();
            this.m_tabDB_ClientCount.Text = ServerConfig.net_info.db_client_count.ToString();
            this.m_tabDB_StartAccount.Text = ServerConfig.net_info.db_start_account.ToString();
            this.m_tabDB_DisconTime.Text = ServerConfig.net_info.db_dis_time.ToString();

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += Update;
            m_timer.Start();
        }
        private void Update(object sender, EventArgs e)
        {
            lock (ThreadScheduler.Instance.LogicLock)
            {
                UpdateTabPage();
                ClientNetManager.Instance.ResetRecvSendPacket();
            }
        }
        private void UpdateTabPage()
        {
            switch (m_pressure_type)
            {
                case ePressureType.Net: UpdateNetTabPage(); break;
                case ePressureType.Login: UpdateLoginTabPage(); break;
                case ePressureType.Move: UpdateMoveTabPage(); break;
                case ePressureType.DB: UpdateDBTabPage(); break;
            }
        }
        /// <summary>
        /// 开启/关闭page
        /// </summary>
        private void EnableTabPage(bool b, TabPage page)
        {
            for (int i = 0; i < m_tab_pages.Count; ++i)
            {
                TabPage tp = m_tab_pages[i];
                if (!b)
                {
                    if (page == tp) continue;
                    m_tabControl.TabPages.Remove(tp);
                }
                else
                {
                    m_tabControl.TabPages.Remove(tp);
                    m_tabControl.TabPages.Insert(i, tp);
                    if(tp == page)
                    {
                        m_tabControl.SelectedTab = page;
                    }
                }
            }
        }
        #region 网络
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～网络～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 网络压力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNetPageClick(object sender, EventArgs e)
        {
            if (m_tabNet_Start.Text == "停止")
            {
                m_tabNet_Start.Enabled = false;
                ClientNetManager.Instance.CloseAll();
                m_tabNet_Start.Text = "开始";
                m_tabNet_Start.Enabled = true;
                EnableTabPage(true, m_tabNet);
                EnableNetTabPage(true);

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Net, false);
            }
            else
            {
                sPressureNetInfo info = new sPressureNetInfo();
                info.ip = this.m_tabNet_IP.Text;
                info.port = ushort.Parse(this.m_tabNet_Port.Text);
                info.client_count = ushort.Parse(this.m_tabNet_ClientCount.Text);
                info.send_count_per_second = ushort.Parse(this.m_tabNet_SendCountPerSecond.Text);
                info.send_size_per_packet = ushort.Parse(this.m_tabNet_SendSizePerSecond.Text);
                ServerConfig.net_info.net_server_ip = info.ip;
                ServerConfig.net_info.net_server_port = info.port;
                ServerConfig.net_info.net_client_count = info.client_count;

                m_tabNet_Start.Enabled = false;
                EnableTabPage(false, m_tabNet);
                EnableNetTabPage(false);
                m_pressure_type = ePressureType.Net;
                m_tabNet_Start.Text = "停止";

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Net, true, info);
                ClientNetManager.Instance.StartConnect(info.ip, info.port, info.client_count);
                m_tabNet_Start.Enabled = true;
            }
        }
        private void UpdateNetTabPage()
        {
            m_tabNet_SendMsgCnt.Text = ClientNetManager.Instance.send_msg_count.ToString();
            m_tabNet_SendMsgSize.Text = ClientNetManager.Instance.send_msg_size.ToString();
            m_tabNet_RecvMsgCnt.Text = ClientNetManager.Instance.recv_msg_count.ToString();
            m_tabNet_RecvMsgSize.Text = ClientNetManager.Instance.recv_msg_size.ToString();
            m_tabNet_CurConnect.Text = ClientNetManager.Instance.connect_count.ToString();
        }
        private void EnableNetTabPage(bool b)
        {
            this.m_tabNet_IP.Enabled = b;
            this.m_tabNet_Port.Enabled = b;
            this.m_tabNet_ClientCount.Enabled = b;
            this.m_tabNet_SendCountPerSecond.Enabled = b;
            this.m_tabNet_SendSizePerSecond.Enabled = b;
        }
        #endregion
        #region 登录
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～登录～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 登陆测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoginPageClick(object sender, EventArgs e)
        {
            if (m_tabLogin_Start.Text == "停止")
            {
                m_tabLogin_Start.Enabled = false;
                ClientNetManager.Instance.CloseAll();
                m_tabLogin_Start.Text = "开始";
                m_tabLogin_Start.Enabled = true;
                EnableTabPage(true, m_tabLogin);
                EnableLoginTabPage(true);

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Login, false);
            }
            else
            {
                sPressureLoginInfo info = new sPressureLoginInfo();
                info.ip = this.m_tabLogin_IP.Text;
                info.port = ushort.Parse(this.m_tabLogin_Port.Text);
                info.client_count = ushort.Parse(this.m_tabLogin_ClientCount.Text);
                info.dis_conn_time = float.Parse(this.m_tabLogin_DisconTime.Text);
                ServerConfig.net_info.login_server_ip = info.ip;
                ServerConfig.net_info.login_server_port = info.port;
                ServerConfig.net_info.login_client_count = info.client_count;

                m_tabLogin_Start.Enabled = false;
                EnableTabPage(false, m_tabLogin);
                EnableLoginTabPage(false);
                m_pressure_type = ePressureType.Login;
                m_tabLogin_Start.Text = "停止";

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Login, true, info);
                ClientNetManager.Instance.StartConnect(info.ip, info.port, info.client_count);
                m_tabLogin_Start.Enabled = true;
            }
        }
        private void UpdateLoginTabPage()
        {
            m_tabLogin_CurClientCount.Text = ClientNetManager.Instance.connect_count.ToString();
        }
        private void EnableLoginTabPage(bool b)
        {
            this.m_tabLogin_IP.Enabled = b;
            this.m_tabLogin_Port.Enabled = b;
            this.m_tabLogin_ClientCount.Enabled = b;
            this.m_tabLogin_DisconTime.Enabled = b;
        }
        #endregion
        #region 移动
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～移动～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnMovePageClick(object sender, EventArgs e)
        {
            if (m_tabMove_Start.Text == "停止")
            {
                m_tabMove_Start.Enabled = false;
                ClientNetManager.Instance.CloseAll();
                m_tabMove_Start.Text = "开始";
                m_tabMove_Start.Enabled = true;
                EnableTabPage(true, m_tabMove);
                EnableMoveTabPage(true);

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Move, false);
            }
            else
            {
                sPressureMoveInfo info = new sPressureMoveInfo();
                info.ip = this.m_tabMove_IP.Text;
                info.port = ushort.Parse(this.m_tabMove_Port.Text);
                info.client_count = ushort.Parse(this.m_tabMove_ClientCount.Text);
                info.move_time = float.Parse(this.m_tabMove_MoveTime.Text);
                info.start_account = int.Parse(this.m_tabMove_StartAccount.Text);
                ServerConfig.net_info.move_server_ip = info.ip;
                ServerConfig.net_info.move_server_port = info.port;
                ServerConfig.net_info.move_client_count = info.client_count;
                ServerConfig.net_info.move_start_account = info.start_account;

                m_tabMove_Start.Enabled = false;
                EnableTabPage(false, m_tabMove);
                EnableMoveTabPage(false);
                m_pressure_type = ePressureType.Move;
                m_tabMove_Start.Text = "停止";

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.Move, true, info);
                ClientNetManager.Instance.StartConnect(info.ip, info.port, info.client_count);
                m_tabMove_Start.Enabled = true;
            }
        }
        private void UpdateMoveTabPage()
        {
            m_tabMove_CurClientCount.Text = ClientNetManager.Instance.connect_count.ToString();
        }
        private void EnableMoveTabPage(bool b)
        {
            this.m_tabMove_IP.Enabled = b;
            this.m_tabMove_Port.Enabled = b;
            this.m_tabMove_ClientCount.Enabled = b;
            this.m_tabMove_StartAccount.Enabled = b;
            this.m_tabMove_MoveTime.Enabled = b;
        }
        #endregion
        #region 数据库
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～数据库～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void OnDBPageClick(object sender, EventArgs e)
        {
            if (m_tabDB_Start.Text == "停止")
            {
                m_tabDB_Start.Enabled = false;
                ClientNetManager.Instance.CloseAll();
                m_tabDB_Start.Text = "开始";
                m_tabDB_Start.Enabled = true;
                EnableTabPage(true, m_tabDB);
                EnableDBTabPage(true);

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.DB, false);
            }
            else
            {
                sPressureDBInfo info = new sPressureDBInfo();
                info.ip = this.m_tabDB_IP.Text;
                info.port = ushort.Parse(this.m_tabDB_Port.Text);
                info.client_count = ushort.Parse(this.m_tabDB_ClientCount.Text);
                info.dis_conn_time = float.Parse(this.m_tabDB_DisconTime.Text);
                info.start_account = int.Parse(this.m_tabDB_StartAccount.Text);
                ServerConfig.net_info.db_server_ip = info.ip;
                ServerConfig.net_info.db_server_port = info.port;
                ServerConfig.net_info.db_client_count = info.client_count;
                ServerConfig.net_info.db_start_account = info.start_account;

                m_tabDB_Start.Enabled = false;
                EnableTabPage(false, m_tabDB);
                EnableDBTabPage(false);
                m_pressure_type = ePressureType.DB;
                m_tabDB_Start.Text = "停止";

                EventController.TriggerEvent(ClientEventID.SWITCH_PRESSURE, ePressureType.DB, true, info);
                ClientNetManager.Instance.StartConnect(info.ip, info.port, info.client_count);
                m_tabDB_Start.Enabled = true;
            }
        }
        private void UpdateDBTabPage()
        {
            m_tabDB_CurClientCount.Text = ClientNetManager.Instance.connect_count.ToString();
        }
        private void EnableDBTabPage(bool b)
        {
            this.m_tabDB_IP.Enabled = b;
            this.m_tabDB_Port.Enabled = b;
            this.m_tabDB_ClientCount.Enabled = b;
            this.m_tabDB_StartAccount.Enabled = b;
            this.m_tabDB_DisconTime.Enabled = b;
        }
        #endregion
    }
}
