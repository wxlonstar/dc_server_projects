using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 消息处理
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ServerMsgProc
    {
        public delegate void MsgProcFunction(PacketBase packet);
        protected Dictionary<ushort, MsgProcFunction> m_msg_proc = null;

        public ServerMsgProc()
        {
            m_msg_proc = new Dictionary<ushort, MsgProcFunction>();
        }
        public void Setup()
        {
            this.RegisterHandle();
        }
        public void Destroy()
        {
            m_msg_proc.Clear();
        }
        private void RegisterHandle()
        {
            RegisterMsgProc(gs2c.msg.PING_NET, OnPingNet);
            RegisterMsgProc(gs2c.msg.ENCRYPT, OnEncryptInfo);
            RegisterMsgProc(gs2c.msg.CLIENT_LOGIN, OnClientLogin);
            RegisterMsgProc(gs2c.msg.CREATE_CHARACTER, OnCreateCharacter);
            RegisterMsgProc(gs2c.msg.ENTER_GAME, OnEnterGame);
            RegisterMsgProc(gs2c.msg.ROBOT_TEST, OnRobotTest);
            RegisterMsgProc(gs2c.msg.SPEED_CHECK, OnSpeedCheck);

            RegisterMsgProc(ss2c.msg.PING_NET, OnPingNet);
            RegisterMsgProc(ss2c.msg.ENUM_CHAR, OnCharacterList);
            RegisterMsgProc(ss2c.msg.CHARACTER_INFO, OnCharacterInfo);
            RegisterMsgProc(ss2c.msg.ENTER_SCENE, OnEnterScene);
            RegisterMsgProc(ss2c.msg.ENTER_AOI, OnUnitEnter);
            RegisterMsgProc(ss2c.msg.LEAVE_AOI, OnUnitLeave);
            RegisterMsgProc(ss2c.msg.UNIT_MOVE, OnUnitMove);
            RegisterMsgProc(ss2c.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterMsgProc(ss2c.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);
            RegisterMsgProc(ss2c.msg.MAIL_COUNT, OnMailCount);
            RegisterMsgProc(ss2c.msg.MAIL_LIST, OnMailList);
            RegisterMsgProc(ss2c.msg.MAIL_READ, OnMailRead);
            RegisterMsgProc(ss2c.msg.MAIL_COMMAND, OnMailCommand);
            RegisterMsgProc(ss2c.msg.RELATION_ADD, OnRelationAdd);
            RegisterMsgProc(ss2c.msg.RELATION_REMOVE, OnRelationRemove);
            RegisterMsgProc(ss2c.msg.RELATION_LIST, OnRelationList);
            RegisterMsgProc(ss2c.msg.RELATION_GIVE, OnRelationGive);
            RegisterMsgProc(ss2c.msg.CHAT_RECV, OnChatRecv);
            RegisterMsgProc(ss2c.msg.CHAT_RESULT, OnChatError);

            RegisterMsgProc(fs2c.msg.PING_NET, OnPingNet);

            RegisterMsgProc(ws2c.msg.PING_NET, OnPingNet);
            RegisterMsgProc(ws2c.msg.SHUTDOWN_SERVER, OnServerShutdown);
            RegisterMsgProc(ws2c.msg.SERVER_TIME, OnServerTime);
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
        public void OnNetworkServer(long conn_idx, ushort header, ByteArray data)
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
            else
            {
                Log.Warning("未注册消息处理函数id：" + header);
            }
            PacketPools.Recover(packet);
        }

        #region ping
        private void OnPingNet(PacketBase packet)
        {
            eServerType server_type = eServerType.NONE;
            uint packet_id = 0;  //发送包id
            long tick = 0;       //发送时间，记录延迟
            long offset_time = 0;
            uint flags = 0;
            if(packet is gs2c.PingNet)
            {
                gs2c.PingNet msg = packet as gs2c.PingNet;
                server_type = eServerType.GATE;
                packet_id = msg.packet_id;
                tick = msg.tick;
                offset_time = Time.time - tick;
                flags = msg.flags;
                Log.Debug("收到gs包:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + (Time.time - msg.tick));
            }
            else if (packet is ss2c.PingNet)
            {
                ss2c.PingNet msg = packet as ss2c.PingNet;
                packet_id = msg.packet_id;
                tick = msg.tick;
                offset_time = Time.time - tick;
                flags = msg.flags;
                if(Utils.HasFlag(msg.flags, (uint)eServerType.GLOBAL))
                    server_type = eServerType.GLOBAL;
                else
                    server_type = eServerType.SERVER;
                Log.Debug("收到ss包:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + (Time.time - msg.tick));
            }
            else if (packet is fs2c.PingNet)
            {
                fs2c.PingNet msg = packet as fs2c.PingNet;
                server_type = eServerType.FIGHT;
                packet_id = msg.packet_id;
                tick = msg.tick;
                offset_time = Time.time - tick;
                flags = msg.flags;
                Log.Debug("收到fs包:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + (Time.time - msg.tick));
            }
            else if (packet is ws2c.PingNet)
            {
                ws2c.PingNet msg = packet as ws2c.PingNet;
                server_type = eServerType.WORLD;
                packet_id = msg.packet_id;
                tick = msg.tick;
                offset_time = Time.time - tick;
                flags = msg.flags;
                Log.Debug("收到ws包:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + (Time.time - msg.tick));
            }
            if (server_type != eServerType.NONE)
            {
                EventController.TriggerEvent(ClientEventID.SERVER_PING, server_type, packet_id, tick, offset_time, flags);
            }
        }
        #endregion

        #region 登陆
        /// <summary>
        /// 加密
        /// </summary>
        private void OnEncryptInfo(PacketBase packet)
        {
            gs2c.EncryptInfo msg = packet as gs2c.EncryptInfo;
            GlobalID.ENCRYPT_KEY = msg.key;

            ServerMsgSend.SendLogin(ServerConfig.net_info.user_name, ServerConfig.net_info.user_psw);
        }
        /// <summary>
        /// 登陆
        /// </summary>
        private void OnClientLogin(PacketBase packet)
        {
            gs2c.ClientLogin msg = packet as gs2c.ClientLogin;
            if(msg.login_result == eLoginResult.E_SUCCESS)
            {
                EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Account, "登录账号:" + ServerConfig.net_info.user_name);
                EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Log, "登录成功");

                ServerMsgSend.SendCharacterList();
            }
            else
            {
                EventController.TriggerEvent(ClientEventID.SHOW_MESSAGE, "登录错误:" + msg.login_result, "错误");
                EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Log, "登录失败");
            }
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        private void OnCharacterList(PacketBase packet)
        {
            ss2c.EnumCharacter msg = packet as ss2c.EnumCharacter;
            LoginDataMgr.Instance.AddCharacterList(msg.list);
            if (msg.list.Count == 0 || msg.list.Count > 1)
            {
                EventController.TriggerEvent(ClientEventID.OPEN_FORM, eFormType.CreateUser);
            }
            else
            {
                CharacterLogin char_info = msg.list[0];
                EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.User, "当前角色:" + char_info.char_name);

                ServerMsgSend.SendEnterGame(char_info.char_idx);
            }
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        private void OnCreateCharacter(PacketBase packet)
        {
            gs2c.CreateCharacter msg = packet as gs2c.CreateCharacter;
            if(msg.result != eCreateCharResult.E_SUCCESS)
            {
                EventController.TriggerEvent(ClientEventID.SHOW_MESSAGE, "创建角色错误:" + msg.result, "错误");
            }
            else
            {
                EventController.TriggerEvent(ClientEventID.SHOW_MESSAGE, "创建成功:" + msg.char_idx, "信息");
                ServerMsgSend.SendCharacterList();
            }
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        private void OnRobotTest(PacketBase packet)
        {
        }
        /// <summary>
        /// 防加速
        /// </summary>
        private void OnSpeedCheck(PacketBase packet)
        {
            gs2c.SpeedCheck msg = packet as gs2c.SpeedCheck;
            SpeedCheckManager.Instance.OnRecvCheckGrap(msg.check_sn, msg.delay_time);
        }
        
        #endregion

        #region 角色
        /// <summary>
        /// 角色基础信息
        /// </summary>
        /// <param name="packet"></param>
        private void OnCharacterInfo(PacketBase packet)
        {
            ss2c.CharacterInfo msg = packet as ss2c.CharacterInfo;
            PlayerDataMgr.Instance.main_player_info = msg.data;
            PlayerDataMgr.Instance.main_player_id = msg.data.char_idx;
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnEnterGame(PacketBase packet)
        {
            gs2c.EnterGame msg = packet as gs2c.EnterGame;
            ServerMsgSend.SendEnterScene(0);
        }
        /// <summary>
        /// 场景切换
        /// </summary>
        private void OnEnterScene(PacketBase packet)
        {
            ss2c.EnterScene msg = packet as ss2c.EnterScene;

            UnitManager.Instance.RemoveAll();
            //玩家信息
            PlayerInfoForClient char_info = PlayerDataMgr.Instance.main_player_info;
            PlayerAOIInfo player_info = CommonObjectPools.Spawn<PlayerAOIInfo>();
            player_info.char_name = char_info.char_name;
            player_info.char_type = char_info.char_type;
            player_info.pos_x = msg.pos.x;
            player_info.pos_y = msg.pos.y;
            player_info.flags = char_info.flags;
            player_info.model_idx = char_info.model_idx;
            player_info.job = char_info.job;
            player_info.level = char_info.level;
            player_info.exp = char_info.exp;
            player_info.energy = char_info.energy;
            player_info.gold = char_info.gold;
            player_info.coin = char_info.coin;
            player_info.hp = char_info.hp;
            player_info.hp_max = char_info.hp_max;
            player_info.hurt = char_info.hurt;
            player_info.range = char_info.range;
            player_info.run_speed = char_info.run_speed;
            player_info.vip_grade = char_info.vip_grade;
            player_info.vip_flags = char_info.vip_flags;

            //创建玩家对象
            Player player = CommonObjectPools.Spawn<Player>();
            player.obj_idx = char_info.char_idx;
            player.Setup();
            player.LoadData(player_info);
            UnitManager.Instance.AddUnit(player);

            EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Scene, "当前场景:" + msg.scene_type);
            EventController.TriggerEvent(ClientEventID.SHOW_STATUS, eFormStatusType.Log, "进入场景:" + msg.scene_type);
        }
        /// <summary>
        /// 单位进入
        /// </summary>
        private void OnUnitEnter(PacketBase packet)
        {
            ss2c.UnitEnterAOI msg = packet as ss2c.UnitEnterAOI;
            Log.Debug("OnUnitEnter:" + msg.unit_idx.obj_idx);

            Unit unit = null;
            switch(msg.unit_idx.type)
            {
                case eUnitType.PLAYER: unit = CommonObjectPools.Spawn<Player>(); break;
            }
            if(unit == null)
            {
                Log.Warning("未定义类型:" + msg.unit_idx.type);
                return;
            }
            unit.obj_idx = msg.unit_idx.obj_idx;
            unit.Setup();
            unit.LoadData(msg.unit_info);
            UnitManager.Instance.AddUnit(unit);
        }
        /// <summary>
        /// 单位离开
        /// </summary>
        private void OnUnitLeave(PacketBase packet)
        {
            ss2c.UnitLeaveAOI msg = packet as ss2c.UnitLeaveAOI;
            Log.Debug("OnUnitLeave:" + msg.unit_idx.obj_idx);

            UnitManager.Instance.RemoveUnit(msg.unit_idx.obj_idx);
        }
        /// <summary>
        /// 单位移动
        /// </summary>
        private void OnUnitMove(PacketBase packet)
        {
            ss2c.UnitMove msg = packet as ss2c.UnitMove;

            Unit unit = UnitManager.Instance.GetUnitByIdx(msg.unit_idx.obj_idx);
            if (unit != null)
            {
                unit.ModifyPos(msg.pos.x, msg.pos.y);
            }
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            ss2c.NotifyUpdatePlayerAttribInteger msg = packet as ss2c.NotifyUpdatePlayerAttribInteger;

            Player player = UnitManager.Instance.GetUnitByIdx(msg.unit_idx.obj_idx) as Player;
            if (player != null)
            {
                switch(msg.type)
                {
                    case eUnitModType.UMT_char_type: player.char_type = (byte)msg.value; break;
                    case eUnitModType.UMT_flags: player.flags = (uint)msg.value; break;
                    case eUnitModType.UMT_model_idx: player.model_idx = (uint)msg.value; break;
                    case eUnitModType.UMT_job: player.job = (byte)msg.value; break;
                    case eUnitModType.UMT_level: player.level = (ushort)msg.value; break;
                    case eUnitModType.UMT_exp: player.exp = (uint)msg.value; break;
                    case eUnitModType.UMT_energy: player.energy = (uint)msg.value; break;
                    case eUnitModType.UMT_gold: player.gold = (uint)msg.value; break;
                    case eUnitModType.UMT_coin: player.coin = (uint)msg.value; break;
                    case eUnitModType.UMT_hp: player.hp = (uint)msg.value; break;
                    case eUnitModType.UMT_vip_grade: player.vip_grade = (uint)msg.value; break;
                    case eUnitModType.UMT_vip_flags: player.vip_flags = (uint)msg.value; break;

                    case eUnitModType.UMT_base_energy: player.energy = (uint)msg.value; break;
                    case eUnitModType.UMT_base_hurt: player.hurt = (uint)msg.value; break;
                    case eUnitModType.UMT_base_run_speed: player.run_speed = (uint)msg.value; break;

                    case eUnitModType.UMT_hp_max: player.hp_max = (uint)msg.value; break;
                    case eUnitModType.UMT_hurt: player.hurt = (uint)msg.value; break;
                    case eUnitModType.UMT_range: player.range = (uint)msg.value; break;
                    case eUnitModType.UMT_run_speed: player.run_speed = (uint)msg.value; break;
                }
            }
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            ss2c.NotifyUpdatePlayerAttribString msg = packet as ss2c.NotifyUpdatePlayerAttribString;

            Player player = UnitManager.Instance.GetUnitByIdx(msg.unit_idx.obj_idx) as Player;
            if (player != null)
            {
                switch (msg.type)
                {
                    case eUnitModType.UMT_char_name: player.char_name = msg.value; break;
                }
            }
        }
        #endregion

        #region 邮件
        /// <summary>
        /// 通知有新邮件
        /// </summary>
        private void OnMailCount(PacketBase packet)
        {
            ss2c.MailCount msg = packet as ss2c.MailCount;
            if(msg.total_mail_count > 0)
            {
                ServerMsgSend.SendMailList();
            }
        }
        /// <summary>
        /// 邮件列表
        /// </summary>
        private void OnMailList(PacketBase packet)
        {
            ss2c.MailList msg = packet as ss2c.MailList;
            MailDataManager.Instance.AddNewMails(msg.mail_list);
        }
        /// <summary>
        /// 读邮件
        /// </summary>
        private void OnMailRead(PacketBase packet)
        {
            ss2c.MailRead msg = packet as ss2c.MailRead;
            MailContentForm form = new MailContentForm(msg.mail_info);
            form.ShowDialog();
        }
        /// <summary>
        /// 邮件操作结果
        /// </summary>
        private void OnMailCommand(PacketBase packet)
        {
            ss2c.MailCommand msg = packet as ss2c.MailCommand;

        }
        #endregion

        #region 关系
        /// <summary>
        /// 添加关系
        /// </summary>
        private void OnRelationAdd(PacketBase packet)
        {
            ss2c.RelationAdd msg = packet as ss2c.RelationAdd;
            RelationAddInfo info = new RelationAddInfo();
            info.event_idx = msg.event_idx;
            info.player_id = msg.player_id;
            info.message = msg.message;
            info.flag = msg.flag;
            RelationDataManager.Instance.AddNewApplys(info);
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        private void OnRelationRemove(PacketBase packet)
        {
            ss2c.RelationRemove msg = packet as ss2c.RelationRemove;
            RelationDataManager.Instance.RemoveRelation(msg.target_char_idx);
        }
        /// <summary>
        /// 关系列表
        /// </summary>
        private void OnRelationList(PacketBase packet)
        {
            ss2c.RelationList msg = packet as ss2c.RelationList;
            RelationDataManager.Instance.AddRelation(msg.list);
        }
        /// <summary>
        /// 赠送
        /// </summary>
        private void OnRelationGive(PacketBase packet)
        {
            ss2c.RelationGive msg = packet as ss2c.RelationGive;
            System.Windows.Forms.MessageBox.Show("收到("+msg.player_id.char_name+")赠送的礼物", "提示", System.Windows.Forms.MessageBoxButtons.OK);
        }
        #endregion
                
        #region 关系
        /// <summary>
        /// 添加关系
        /// </summary>
        private void OnChatRecv(PacketBase packet)
        {
            ss2c.ChatRecv msg = packet as ss2c.ChatRecv;
            ChatDataManager.Instance.AddChat(msg.type, msg.sender, msg.chat_content);
        }
        /// <summary>
        /// 聊天错误
        /// </summary>
        private void OnChatError(PacketBase packet)
        {
            ss2c.ChatResult msg = packet as ss2c.ChatResult;
        }
        #endregion

        #region ws
        /// <summary>
        /// 服务器即将关闭
        /// </summary>
        private void OnServerShutdown(PacketBase packet)
        {
            ws2c.ShutdownServer msg = packet as ws2c.ShutdownServer;
            Log.Debug("服务器即将关闭:" + msg.leave_time);
        }
        int timer_id = 0;
        /// <summary>
        /// 服务器时间
        /// </summary>
        private void OnServerTime(PacketBase packet)
        {
            ws2c.ServerTime msg = packet as ws2c.ServerTime;
            Log.Debug("服务器时间:" + msg.server_time);
            if (timer_id != 0)
                TimerManager.Instance.RemoveTimer(timer_id);
            timer_id = TimerManager.Instance.AddLoop(1000 * 60 * 10, int.MaxValue, (id, param) => { ServerMsgSend.SendServerTime(); });
        }
        #endregion
    }
}
