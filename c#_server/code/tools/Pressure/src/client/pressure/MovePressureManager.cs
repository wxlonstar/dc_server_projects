using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace dc
{
    /// <summary>
    /// 移动压力测试
    /// @author hannibal
    /// @time 2016-9-3
    /// </summary>
    public class MovePressureManager : Singleton<MovePressureManager>
    {
        private bool m_active = false;
        private Timer m_timer = null;
        private sPressureMoveInfo m_pressure_info = null;
        private Dictionary<long, MoveClient> m_connectes = null;
        private List<MoveClient> m_dis_connectes = null;

        public MovePressureManager()
        {
            m_connectes = new Dictionary<long, MoveClient>();
            m_dis_connectes = new List<MoveClient>();
        }

        public void Setup()
        {
            m_active = false;
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            m_connectes.Clear();
            m_dis_connectes.Clear();
        }
        public void Tick()
        {
        }
        private int total = 0;
        private void Update(object sender, EventArgs e)
        {
            if (m_active)
            {
                List<long> list_keys = new List<long>(m_connectes.Keys);
                if (list_keys.Count <= 0) return;

                int count = (int)(m_pressure_info.move_time * 0.1f);
                count = count < 1 ? 1 : count;
                lock (ThreadScheduler.Instance.LogicLock)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        long conn_idx = MathUtils.RandRange_List<long>(list_keys);
                        MoveClient client;
                        if (m_connectes.TryGetValue(conn_idx, out client))
                        {
                            if(client.CanMove())
                            {
                                ++total;
                                if (total % 100 == 0) Log.Debug("发送移动包数量:" + total);
                                this.MoveOne(conn_idx, client.pos.x, client.pos.y);
                                list_keys.Remove(conn_idx);
                            }
                        }
                    }
                }
            }
        }
        private void Start()
        {
            Stop();
            m_active = true;
            m_timer = new Timer();
            m_timer.Interval = 100;
            m_timer.Tick += Update;
            m_timer.Start();
        }
        private void Stop()
        {
            m_active = false;
            if (m_timer != null)
            {
                m_timer.Stop();
                m_timer = null;
            }
        }
        /// <summary>
        /// 移动
        /// </summary>
        private void MoveOne(long conn_idx, int x, int y)
        {
            x = MathUtils.RandRange(-2, 2) + x;
            y = MathUtils.RandRange(-2, 2) + y;
            x = (int)MathUtils.Clamp(x, -50, 50);
            x = (int)MathUtils.Clamp(x, -50, 50);
            ServerMsgSend.SendUnitMove(conn_idx, x, y);
        }
        /*～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～事件～～～～～～～～～～～～～～～～～～～～～～～～～～～～～～*/
        private void RegisterEvent()
        {
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.AddEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
            EventController.AddEventListener(ClientEventID.RECV_DATA, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_OPEN, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.NET_CONNECTED_CLOSE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.SWITCH_PRESSURE, OnGameEvent);
            EventController.RemoveEventListener(ClientEventID.RECV_DATA, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case ClientEventID.NET_CONNECTED_OPEN:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        if (!m_connectes.ContainsKey(conn_idx))
                        {
                            if (m_dis_connectes.Count > 0)
                            {
                                MoveClient client = MathUtils.RandRange_List<MoveClient>(m_dis_connectes);
                                client.ReSetup(conn_idx);
                                m_connectes.Add(conn_idx, client);
                                m_dis_connectes.Remove(client);
                            }
                            else
                            {
                                MoveClient client = new MoveClient();
                                client.Setup(conn_idx, m_pressure_info.start_account, conn_idx);
                                m_connectes.Add(conn_idx, client);
                            }
                        }
                    }
                    break;

                case ClientEventID.NET_CONNECTED_CLOSE:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        MoveClient client;
                        if (m_connectes.TryGetValue(conn_idx, out client))
                        {
                            m_dis_connectes.Add(client);
                        }
                        m_connectes.Remove(conn_idx);
                    }
                    break;

                case ClientEventID.SWITCH_PRESSURE:
                    {
                        ePressureType type = evt.Get<ePressureType>(0);
                        bool is_start = evt.Get<bool>(1);
                        if (type == ePressureType.Move && is_start)
                        {
                            m_pressure_info = evt.Get<sPressureMoveInfo>(2);
                            this.Start();
                        }
                        else
                        {
                            this.Stop();
                        }
                    }
                    break;
                case ClientEventID.RECV_DATA:
                    {
                        if (!m_active) break;
                        long conn_idx = evt.Get<long>(0);
                        ushort header = evt.Get<ushort>(1);
                        ByteArray data = evt.Get<ByteArray>(2);
                        MoveClient client;
                        if(m_connectes.TryGetValue(conn_idx, out client))
                        {
                            client.OnNetworkServer(header, data);
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 单个客户端
    /// @author hannibal
    /// @time 2016-9-3
    /// </summary>
    class MoveClient
    {
        protected delegate void MsgProcFunction(PacketBase packet);
        protected Dictionary<ushort, MsgProcFunction> m_msg_proc = null;

        protected long m_conn_idx = 0;
        protected long m_start_account_id = 1;
        protected long m_account_idx = 0;
        protected uint m_scene_type = 0;
        protected Position2D m_pos = new Position2D();
        protected PlayerInfoForClient m_unit_info;

        public MoveClient()
        {
            m_msg_proc = new Dictionary<ushort, MsgProcFunction>();
        }
        public void Setup(long conn_idx, long start_account_idx, long account_idx)
        {
            m_conn_idx = conn_idx;
            m_start_account_id = start_account_idx;
            m_account_idx = account_idx;
            m_scene_type = 0;
            this.RegisterHandle();
        }
        public void ReSetup(long conn_idx)
        {
            m_conn_idx = conn_idx;
            m_scene_type = 0;
        }
        public void Destroy()
        {
            m_msg_proc.Clear();
        }
        private void RegisterHandle()
        {
            RegisterMsgProc(gs2c.msg.ENCRYPT, OnEncryptInfo);
            RegisterMsgProc(gs2c.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(gs2c.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(gs2c.msg.ENTER_GAME, OnEnterGame);

            RegisterMsgProc(ss2c.msg.ENUM_CHAR, OnCharacterList);
            RegisterMsgProc(ss2c.msg.CHARACTER_INFO, OnCharacterInfo);
            RegisterMsgProc(ss2c.msg.ENTER_SCENE, OnEnterScene);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            if (m_msg_proc.ContainsKey(id))
            {
                Log.Error("相同键已经存在:" + id);
                return;
            }
            m_msg_proc.Add(id, fun);
        }

        /// <summary>
        /// 网络事件处理
        /// </summary>
        public void OnNetworkServer(ushort header, ByteArray data)
        {
            PacketBase packet = PacketPools.Get(header);
            packet.Read(data);

            MsgProcFunction fun;
            if (m_msg_proc.TryGetValue(packet.header, out fun))
            {
                try
                {
                    fun(packet);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
            PacketPools.Recover(packet);
        }
        /// <summary>
        /// 加密
        /// </summary>
        private void OnEncryptInfo(PacketBase packet)
        {
            gs2c.EncryptInfo msg = packet as gs2c.EncryptInfo;
            GlobalID.ENCRYPT_KEY = msg.key;

            ServerMsgSend.SendLogin(m_conn_idx, "test" + (int)(m_account_idx + m_start_account_id - 1), "1");
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private void OnClientLogin(PacketBase packet)
        {
            gs2c.ClientLogin msg = packet as gs2c.ClientLogin;
            if (msg.login_result == eLoginResult.E_SUCCESS)
            {
                ServerMsgSend.SendCharacterList(m_conn_idx);
            }
            else
            {
                Log.Error("登录错误:" + m_conn_idx);
            }
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        private void OnCharacterList(PacketBase packet)
        {
            ss2c.EnumCharacter msg = packet as ss2c.EnumCharacter;
            if (msg.list.Count == 0 || msg.list.Count > 1)
            {
                ServerMsgSend.SendCreateCharacter(m_conn_idx, "test" + (int)(m_account_idx + m_start_account_id - 1), (uint)eSexType.BOY);
            }
            else
            {
                CharacterLogin char_info = msg.list[0];
                ServerMsgSend.SendEnterGame(m_conn_idx, char_info.char_idx);
            }
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            gs2c.CreateCharacter msg = packet as gs2c.CreateCharacter;
            if (msg.result != eCreateCharResult.E_SUCCESS)
            {
                Log.Error("创建角色错误:" + msg.result);
            }
            else
            {
                Log.Debug("角色创建成功:" + msg.char_idx);
                ServerMsgSend.SendCharacterList(m_conn_idx);
            }
        }
        /// <summary>
        /// 角色基础信息
        /// </summary>
        /// <param name="packet"></param>
        private void OnCharacterInfo(PacketBase packet)
        {
            ss2c.CharacterInfo msg = packet as ss2c.CharacterInfo;
            m_unit_info = msg.data;
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnEnterGame(PacketBase packet)
        {
            gs2c.EnterGame msg = packet as gs2c.EnterGame;
            ServerMsgSend.SendEnterScene(m_conn_idx, 0);
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        private void OnEnterScene(PacketBase packet)
        {
            ss2c.EnterScene msg = packet as ss2c.EnterScene;
            m_scene_type = msg.scene_type;
            m_pos = msg.pos;
        }

        public bool CanMove()
        {
            if (m_scene_type == 0) return false;
            return true;
        }
        public long conn_idx
        {
            get { return m_conn_idx; }
        }
        public Position2D pos
        {
            get { return m_pos; }
        }
    }
}
