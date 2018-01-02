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
    /// 主界面
    /// @author hannibal
    /// @time 2016-8-22
    /// </summary>
    public partial class MainForm : Form
    {
        private Timer m_timer;
        private MagneticManager m_magnetic;
        private DrawUserForm m_draw_user_form;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += Update;
            m_timer.Start();

            LoginForm login_form = new LoginForm();
            login_form.Show(this);

            this.RegisterEvent();
        }
        private void OnLoad(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;//设置父窗体是容器
            m_draw_user_form = new DrawUserForm();//实例化子窗体
            m_draw_user_form.MdiParent = this;//设置窗体的父子关系
            m_draw_user_form.Parent = m_main_panel;//设置子窗体的容器为父窗体中的Panel
            m_draw_user_form.Show();//显示子窗体，此句很重要，否则子窗体不会显示

            m_magnetic = new MagneticManager(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            this.UnRegisterEvent();
            base.OnClosed(e);
        }
        private void Update(object sender, EventArgs e)
        {
        }
        #region 事件
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.SHOW_MESSAGE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.SHOW_STATUS, OnGameEvent);
            EventController.AddEventListener(ClientEventID.OPEN_FORM, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.SHOW_MESSAGE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.SHOW_STATUS, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.OPEN_FORM, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        this.ShowState(eFormStatusType.Account, "已连接");
                    }
                    break;
                case ClientEventID.NET_CONNECTED_CLOSE:
                    {
                        this.ShowState(eFormStatusType.Account, "未连接");
                        this.ShowState(eFormStatusType.User, "未登陆");
                        this.ShowState(eFormStatusType.Scene, "当前场景");
                    }
                    break;
                case ClientEventID.SHOW_MESSAGE:
                    {
                        string msg = evt.Get<string>(0);
                        string title = evt.Get<string>(1);
                        MessageBox.Show(msg, title, MessageBoxButtons.OK);
                    }
                    break;
                case ClientEventID.SHOW_STATUS:
                    {
                        eFormStatusType type = evt.Get<eFormStatusType>(0);
                        switch(type)
                        {
                            case eFormStatusType.Account:
                            case eFormStatusType.User:
                            case eFormStatusType.Scene:
                                string msg = evt.Get<string>(1);
                                this.ShowState(type, msg);
                                break;

                            case eFormStatusType.Log:
                                msg = evt.Get<string>(1);
                                this.ShowState(type, msg);
                                break;
                        }
                    }
                    break;
                case ClientEventID.OPEN_FORM:
                    {
                        eFormType form_type = evt.Get<eFormType>(0);
                        switch(form_type)
                        {
                            case eFormType.CreateUser:
                                CreateUserForm create_form = new CreateUserForm();
                                create_form.ShowDialog();
                                break;
                        }
                    }
                    break;

            }
        }
        #endregion
        #region 菜单事件
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～菜单事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        /// <summary>
        /// 登录
        /// </summary>
        private void NetLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm login_form = new LoginForm();
            login_form.Show(this);
        }
        /// <summary>
        /// 登出
        /// </summary>
        private void NetLoginOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult re = MessageBox.Show("确定要登出吗?", "登出", MessageBoxButtons.OKCancel);
            if(re == DialogResult.OK)
            {
                ClientNetManager.Instance.Close();
            }
        }
        /// <summary>
        /// 创号
        /// </summary>
        private void NetCreateUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateUserForm create_form = new CreateUserForm();
            create_form.ShowDialog(this);
        }
        /// <summary>
        /// 查看信息
        /// </summary>
        private void NetUserInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserInfoForm info_form = new UserInfoForm(PlayerDataMgr.Instance.main_player_id);
            info_form.ShowDialog(this);
        }
        /// <summary>
        /// 修改属性
        /// </summary>
        private void NetModifyInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyUserInfoForm info_form = new ModifyUserInfoForm(PlayerDataMgr.Instance.main_player_id);
            info_form.ShowDialog(this);
        }


        /// <summary>
        /// 切换场景
        /// </summary>
        private void SceneChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeSceneForm create_form = new ChangeSceneForm();
            create_form.ShowDialog(this);
        }
        
        /// <summary>
        /// 邮件
        /// </summary>
        private void SysMailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChatForm mail_form = new ChatForm();
            mail_form.ShowDialog(this);
        }

        /// <summary>
        /// 好友申请列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysRelationAppleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RelationAddForm relation_form = new RelationAddForm();
            relation_form.ShowDialog(this);
        }
        /// <summary>
        /// 好友列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysRelationListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RelationListForm relation_form = new RelationListForm();
            relation_form.ShowDialog(this);
        }
        /// <summary>
        /// 聊天
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChatForm chat_form = new ChatForm();
            chat_form.ShowDialog(this);
        }

        /// <summary>
        /// 日志
        /// </summary>
        private void ToolsLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoggerForm log_form = new LoggerForm();
            log_form.Show();

            m_magnetic.addChild(log_form, MagneticLocation.Right);
        }
        /// <summary>
        /// 控制台
        /// </summary>
        private void ToolsConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientUtils.SwitchConsole();
        }
        /// <summary>
        /// ping网速
        /// </summary>
        private void ToolsPingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PingForm ping_form = new PingForm();
            ping_form.Show();
        }
        
        #endregion
        #region 状态栏
        private void ShowState(eFormStatusType type, string txt)
        {
            switch (type)
            {
                case eFormStatusType.Account:
                    this.m_status_account.Text = txt;
                    break;
                case eFormStatusType.User:
                    this.m_status_user.Text = txt;
                    break;
                case eFormStatusType.Scene:
                    this.m_status_scene.Text = txt;
                    break;
                case eFormStatusType.Log:
                    this.m_status_log.Text = txt;
                    break;
            }
        }
        #endregion

    }

}
