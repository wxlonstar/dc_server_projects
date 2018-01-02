using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 网关+client消息处理
    /// @author hannibal
    /// @time 2016-8-16
    /// </summary>
    public class GateMsgProc : ConnAppProc
    {
        protected MsgProcFunction[] m_client_msg_proc = null;

        public GateMsgProc()
            : base()
        {
            m_msg_begin_idx = ProtocolID.MSG_BASE_GS2SS;
            m_client_msg_proc = new MsgProcFunction[ProtocolID.MSG_APPLAYER_PER_INTERVAL];
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_client_msg_proc[i] = null;
            }
        }
        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
            for (int i = 0; i < ProtocolID.MSG_APPLAYER_PER_INTERVAL; ++i)
            {
                m_client_msg_proc[i] = null;
            }
        }
        protected override void RegisterHandle()
        {
            ///处理gate消息
            RegisterMsgProc(gs2ss.msg.PROXY_CLIENT_MSG, HandleProxyClientMsg);
            RegisterMsgProc(gs2ss.msg.ENUM_CHAR, OnEnumCharacterList);
            RegisterMsgProc(gs2ss.msg.ENTER_GAME, OnEnterGame);
            RegisterMsgProc(gs2ss.msg.ROBOT_TEST, OnRobotTest);
            RegisterMsgProc(gs2ss.msg.LOGOUT_ACCOUNT, OnLogoutAccount);
            RegisterMsgProc(gs2ss.msg.KICK_ACCOUNT, OnKickAccount);

            ///处理客户端消息
            RegisterClientMsgProc(c2ss.msg.PING_NET, OnPingNet);
            RegisterClientMsgProc(c2ss.msg.ENTER_SCENE, OnEnterScene);
            //AOI
            RegisterClientMsgProc(c2ss.msg.UNIT_MOVE, OnUnitMove);
            RegisterClientMsgProc(c2ss.msg.UNIT_MODIFY_INT, OnUnitAttrModifyInt);
            RegisterClientMsgProc(c2ss.msg.UNIT_MODIFY_STRING, OnUnitAttrModifyString);
            //邮件
            RegisterClientMsgProc(c2ss.msg.MAIL_LIST, OnMailList);
            RegisterClientMsgProc(c2ss.msg.MAIL_READ, OnMailRead);
            RegisterClientMsgProc(c2ss.msg.MAIL_TAKE, OnMailTake);
            RegisterClientMsgProc(c2ss.msg.MAIL_WRITE, OnMailWrite);
            RegisterClientMsgProc(c2ss.msg.MAIL_DELETE, OnMailDelete);
            //关系
            RegisterClientMsgProc(c2ss.msg.RELATION_ADD, OnRelationAdd);
            RegisterClientMsgProc(c2ss.msg.RELATION_REMOVE, OnRelationRemove);
            RegisterClientMsgProc(c2ss.msg.RELATION_LIST, OnRelationList);
            RegisterClientMsgProc(c2ss.msg.RELATION_GIVE, OnRelationGive);
            RegisterClientMsgProc(c2ss.msg.RELATION_APPLY_CMD, OnRelationApplyCommand);
            //聊天
            RegisterClientMsgProc(c2ss.msg.CHAT_SEND, OnChatSend);
        }
        private void RegisterMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - m_msg_begin_idx);
            m_msg_proc[msg_id] = fun;
        }
        private void RegisterClientMsgProc(ushort id, MsgProcFunction fun)
        {
            ushort msg_id = (ushort)(id - ProtocolID.MSG_BASE_C2SS);
            m_client_msg_proc[msg_id] = fun;
        }
        /// <summary>
        /// 处理由gate转发的client消息
        /// </summary>
        /// <param name="packet"></param>
        private void HandleProxyClientMsg(PacketBase packet)
        {
            ProxyC2SMsg proxy_msg = packet as ProxyC2SMsg;

            //转发的消息id
            ushort header = proxy_msg.data.ReadUShort();
            if (header < ProtocolID.MSG_BASE_C2SS || header >= ProtocolID.MSG_BASE_C2SS + ProtocolID.MSG_APPLAYER_PER_INTERVAL)
            {
                Log.Debug("收到错误的消息ID:" + header);
                return;
            }

            PacketBase msg = PacketPools.Get(header);
            msg.Read(proxy_msg.data);

            //处理
            ushort msg_id = (ushort)(header - ProtocolID.MSG_BASE_C2SS);
            MsgProcFunction fun = m_client_msg_proc[msg_id];
            if (fun == null)
            {
                Log.Debug("未注册消息处理函数ID:" + header);
            }
            else
            {
                fun(msg);
            }
            PacketPools.Recover(msg);
        }

        #region gate消息处理
        /// <summary>
        /// 角色列表
        /// </summary>
        private void OnEnumCharacterList(PacketBase packet)
        {
            gs2ss.EnumCharacter msg = packet as gs2ss.EnumCharacter;
            ClientUID client_uid = msg.client_uid;

            SQLCharHandle.QueryCharacterList(msg.account_idx, new DBID(msg.game_db_id), (data) =>
            {
                ss2c.EnumCharacter rep_msg = PacketPools.Get(ss2c.msg.ENUM_CHAR) as ss2c.EnumCharacter;
                foreach (var char_data in data)
                {
                    rep_msg.list.Add(char_data);
                }
                ServerNetManager.Instance.SendProxy(client_uid, rep_msg);
            }
            );
        }
        /// <summary>
        /// 进入游戏
        /// </summary>
        private void OnEnterGame(PacketBase packet)
        {
            gs2ss.EnterGame msg = packet as gs2ss.EnterGame;
            ClientUID client_uid = msg.client_uid;
            InterServerID server_uid = msg.server_uid;

            if (!UnitManager.Instance.HasUnit(msg.char_idx))
            {
                DBID db_id = new DBID();
                db_id.game_id = ServerConfig.GetDBByAccountIdx(msg.account_idx, eDBType.Game);
                PlayerInfoForSS ss_data = CommonObjectPools.Spawn<PlayerInfoForSS>();
                SQLCharHandle.QueryCharacterInfo(msg.char_idx, db_id, ss_data, is_load =>
                {
                    if(is_load)
                    {
                        //创建玩家
                        Player player = CommonObjectPools.Spawn<Player>();
                        player.client_uid = client_uid;
                        player.LoadData(ss_data);
                        UnitManager.Instance.AddUnit(player);

                        //告诉gs成功进入游戏
                        ss2gs.EnterGame rep_gs_msg = PacketPools.Get(ss2gs.msg.ENTER_GAME) as ss2gs.EnterGame;
                        rep_gs_msg.server_uid = server_uid;
                        rep_gs_msg.client_uid = client_uid;
                        rep_gs_msg.char_idx = ss_data.char_idx;
                        ServerNetManager.Instance.Send(server_uid.gs_uid, rep_gs_msg);

                        //告诉ws
                        ss2ws.LoginClient rep_ws_msg = PacketPools.Get(ss2ws.msg.LOGIN_CLIENT) as ss2ws.LoginClient;
                        rep_ws_msg.server_uid = server_uid;
                        rep_ws_msg.client_uid = client_uid;
                        rep_ws_msg.data.Copy(ss_data);
                        ServerNetManager.Instance.Send2WS(rep_ws_msg);

                        //告诉gl
                        ss2gl.LoginClient rep_gl_msg = PacketPools.Get(ss2gl.msg.LOGIN_CLIENT) as ss2gl.LoginClient;
                        rep_gl_msg.server_uid = server_uid;
                        rep_gl_msg.data.Copy(ss_data);
                        ServerNetManager.Instance.Send2GL(rep_gl_msg);

                        //告诉客户端角色基础信息
                        ss2c.CharacterInfo rep_msg = PacketPools.Get(ss2c.msg.CHARACTER_INFO) as ss2c.CharacterInfo;
                        rep_msg.data.Copy(ss_data);
                        ServerNetManager.Instance.SendProxy(client_uid, rep_msg, false);
                    }
                    CommonObjectPools.Despawn(ss_data);
                });
            }
        }
        /// <summary>
        /// 压力测试
        /// </summary>
        private void OnRobotTest(PacketBase packet)
        {
            gs2ss.RobotTest msg = packet as gs2ss.RobotTest;

            ss2gs.RobotTest rep_msg = PacketPools.Get(ss2gs.msg.ROBOT_TEST) as ss2gs.RobotTest;
            rep_msg.client_uid = msg.client_uid;
            rep_msg.length = msg.length;
            this.Send(rep_msg);
        }
        /// <summary>
        /// 账号登出
        /// </summary>
        private void OnLogoutAccount(PacketBase packet)
        {
            gs2ss.LogoutAccount msg = packet as gs2ss.LogoutAccount;
            HandleLogoutAccount(msg.account_idx);
        }
        /// <summary>
        /// 踢号
        /// </summary>
        private void OnKickAccount(PacketBase packet)
        {
            gs2ss.KickoutAccount msg = packet as gs2ss.KickoutAccount;
            HandleLogoutAccount(msg.account_idx);
        }
        /// <summary>
        /// 处理登出逻辑
        /// </summary>
        private void HandleLogoutAccount(long account_idx)
        {
            Player player = UnitManager.Instance.GetPlayerByAccount(account_idx) as Player;
            if (player == null)
            {//如果在加载完角色信息前退出，则不会有unit
                return;
            }

            //告诉fs
            ss2fs.LogoutClient fs_msg = PacketPools.Get(ss2fs.msg.LOGOUT_CLIENT) as ss2fs.LogoutClient;
            fs_msg.char_idx = player.char_idx;
            ServerNetManager.Instance.Send2FS(player.fs_uid, fs_msg);

            //告诉ws
            ss2ws.LogoutClient ws_msg = PacketPools.Get(ss2ws.msg.LOGOUT_CLIENT) as ss2ws.LogoutClient;
            ws_msg.char_idx = player.char_idx;
            ServerNetManager.Instance.Send2WS(ws_msg);

            //告诉gl
            ss2gl.LogoutClient gl_msg = PacketPools.Get(ss2gl.msg.LOGOUT_CLIENT) as ss2gl.LogoutClient;
            gl_msg.char_idx = player.char_idx;
            ServerNetManager.Instance.Send2GL(gl_msg);

            //从场景移除
            BaseScene scene = SceneManager.Instance.GetScene(player.scene_obj_idx);
            if (scene != null)
            {
                scene.RemoveUnit(player);
            }
            //从管理器移除
            UnitManager.Instance.RemoveUnit(player);
        }
        #endregion

        #region 客户端消息
        /// <summary>
        /// ping网络
        /// </summary>
        private void OnPingNet(PacketBase packet)
        {
            c2ss.PingNet msg = packet as c2ss.PingNet;

            if(Utils.HasFlag(msg.flags, (uint)eServerType.GLOBAL))
            {//ping gl
                ss2gl.PingNet rep_msg = PacketPools.Get(ss2gl.msg.PING_NET) as ss2gl.PingNet;
                rep_msg.client_uid = msg.client_uid;
                rep_msg.packet_id = msg.packet_id;
                rep_msg.tick = msg.tick;
                rep_msg.flags = msg.flags;
                ServerNetManager.Instance.Send2GL(rep_msg);
            }
            else
            {
                long offset_time = Time.time - msg.tick;
                Log.Debug("收到第:" + msg.packet_id + " 时间:" + Time.time + " 延迟:" + offset_time);

                ss2c.PingNet rep_msg = PacketPools.Get(ss2c.msg.PING_NET) as ss2c.PingNet;
                rep_msg.packet_id = msg.packet_id;
                rep_msg.tick = msg.tick;
                rep_msg.flags = msg.flags;
                ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg);
            }
        }
        /// <summary>
        /// 进入场景
        /// </summary>
        private void OnEnterScene(PacketBase packet)
        {
            c2ss.EnterScene msg = packet as c2ss.EnterScene;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if(player == null)
            {
                Log.Debug("OnEnterScene 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            uint scene_idx = 0;
            if(msg.scene_type == 0)
            {//进入游戏后的第一个消息：告诉client需要进入的场景id
                scene_idx = player.scene_type_idx;//上次所在场景
            }
            else
            {
                //相同场景跳转，直接返回
                if(msg.scene_type == player.scene_type_idx)
                    return;
                scene_idx = msg.scene_type;
            }
            //判断场景是否有效
            if (!SceneID.IsValidScene(scene_idx))
                return;

            //加入场景
            BaseScene scene = SceneManager.Instance.CreateScene(scene_idx);
            if (scene != null)
            {
                //先从旧的场景移除
                BaseScene old_scene = SceneManager.Instance.GetScene(player.scene_obj_idx);
                if(old_scene != null)
                {
                    old_scene.RemoveUnit(player);
                }
                //再加入新的场景
                scene.AddUnit(player);
                player.unit_attr.SetAttribInteger(eUnitModType.UMT_scene_type, scene_idx);
            }
            else
            {
                Log.Warning("加入场景失败:" + scene_idx);
                return;
            }

            //告诉client
            ss2c.EnterScene rep_msg = PacketPools.Get(ss2c.msg.ENTER_SCENE) as ss2c.EnterScene;
            rep_msg.scene_type = scene_idx;
            rep_msg.scene_instance_id = 1;
            rep_msg.pos.Set(player.pos_x, player.pos_y);//TODO：获取场景出生点
            rep_msg.dir = eDirection.NONE;
            ServerNetManager.Instance.SendProxy(msg.client_uid, rep_msg, false);
        }

        #region 角色
        /// <summary>
        /// 移动
        /// </summary>
        private void OnUnitMove(PacketBase packet)
        {
            c2ss.UnitMove msg = packet as c2ss.UnitMove;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnUnitMove 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            player.ModifyPos(msg.pos.x, msg.pos.y);

            //广播
            AOIManager.Instance.UpdatePosition(player.char_idx, msg.pos.y, msg.pos.x, false);
        }
        /// <summary>
        /// 属性改变
        /// </summary>
        private void OnUnitAttrModifyInt(PacketBase packet)
        {
            c2ss.NotifyUpdatePlayerAttribInteger msg = packet as c2ss.NotifyUpdatePlayerAttribInteger;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnUnitAttrModifyInt 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            player.unit_attr.SetAttribInteger(msg.type, msg.value);
        }
        private void OnUnitAttrModifyString(PacketBase packet)
        {
            c2ss.NotifyUpdatePlayerAttribString msg = packet as c2ss.NotifyUpdatePlayerAttribString;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnUnitAttrModifyString 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            player.unit_attr.SetAttribString(msg.type, msg.value);
        }
        #endregion

        #region 邮件
        /// <summary>
        /// 邮件列表
        /// </summary>
        private void OnMailList(PacketBase packet)
        {
            c2ss.MailList msg = packet as c2ss.MailList;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if(player == null)
            {
                Log.Debug("OnMailList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            Mailbox mail_box = MailboxManager.Instance.GetMailBox(player.char_idx);
            if(mail_box != null)
            {
                mail_box.HandleReqList();
            }
        }
        private void OnMailRead(PacketBase packet)
        {
            c2ss.MailRead msg = packet as c2ss.MailRead;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnMailList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            Mailbox mail_box = MailboxManager.Instance.GetMailBox(player.char_idx);
            if (mail_box != null)
            {
                mail_box.HandleReadMail(msg.mail_idx);
            }
        }
        private void OnMailTake(PacketBase packet)
        {
            c2ss.MailTake msg = packet as c2ss.MailTake;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnMailList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            Mailbox mail_box = MailboxManager.Instance.GetMailBox(player.char_idx);
            if (mail_box != null)
            {
                mail_box.HandleTakeMail(msg.mail_idx, msg.delete_mail);
            }
        }
        private void OnMailWrite(PacketBase packet)
        {
            c2ss.MailWrite msg = packet as c2ss.MailWrite;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnMailList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            Mailbox mail_box = MailboxManager.Instance.GetMailBox(player.char_idx);
            if (mail_box != null)
            {
                mail_box.HandleWriteMail(msg.info);
            }
        }
        private void OnMailDelete(PacketBase packet)
        {
            c2ss.MailDelete msg = packet as c2ss.MailDelete;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnMailList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }
            Mailbox mail_box = MailboxManager.Instance.GetMailBox(player.char_idx);
            if (mail_box != null)
            {
                mail_box.HandleDeleteMail(msg.mail_list);
            }
        }
        #endregion

        #region 关系
        /// <summary>
        /// 添加关系
        /// </summary>
        private void OnRelationAdd(PacketBase packet)
        {
            c2ss.RelationAdd msg = packet as c2ss.RelationAdd;

            //判断数据有效性
            if ((msg.target_id.type == eRelationAddType.Idx && msg.target_id.char_idx <= 0) ||
                (msg.target_id.type == eRelationAddType.Name && string.IsNullOrEmpty(msg.target_id.char_name)))
                return;

            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnRelationAdd 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            //不能加自己
            if ((msg.target_id.type == eRelationAddType.Idx && msg.target_id.char_idx == player.char_idx) ||
                (msg.target_id.type == eRelationAddType.Name && msg.target_id.char_name == player.char_name))
                return;

            MemberRelation relation = RelationManager.Instance.GetMember(player.char_idx);
            if(relation != null)
            {
                relation.AddRelationClient(msg.target_id, msg.flag, msg.message);
            }
        }
        /// <summary>
        /// 移除关系
        /// </summary>
        private void OnRelationRemove(PacketBase packet)
        {
            c2ss.RelationRemove msg = packet as c2ss.RelationRemove;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnRelationRemove 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            MemberRelation relation = RelationManager.Instance.GetMember(player.char_idx);
            if (relation != null)
            {
                relation.RemoveRelationClient(msg.target_char_idx);
            }
        }
        /// <summary>
        /// 关系列表
        /// </summary>
        private void OnRelationList(PacketBase packet)
        {
            c2ss.RelationList msg = packet as c2ss.RelationList;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnRelationList 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            MemberRelation relation = RelationManager.Instance.GetMember(player.char_idx);
            if (relation != null)
            {
                relation.RelationListClient();
            }
        }
        /// <summary>
        /// 赠送
        /// </summary>
        private void OnRelationGive(PacketBase packet)
        {
            c2ss.RelationGive msg = packet as c2ss.RelationGive;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnRelationGive 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            MemberRelation relation = RelationManager.Instance.GetMember(player.char_idx);
            if (relation != null)
            {
                relation.FriendGiveClient(msg.target_char_idx, msg.item_id);
            }
        }
        /// <summary>
        /// 操作
        /// </summary>
        private void OnRelationApplyCommand(PacketBase packet)
        {
            c2ss.RelationApplyCmd msg = packet as c2ss.RelationApplyCmd;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnRelationApply 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            MemberRelation relation = RelationManager.Instance.GetMember(player.char_idx);
            if (relation != null)
            {
                relation.ApplyCommandClient(msg.event_idx, msg.target_char_idx, msg.cmd);
            }
        }
        #endregion

        #region 聊天
        /// <summary>
        /// 发送聊天
        /// </summary>
        private void OnChatSend(PacketBase packet)
        {
            c2ss.ChatSend msg = packet as c2ss.ChatSend;
            Player player = UnitManager.Instance.GetPlayerByClientUID(msg.client_uid);
            if (player == null)
            {
                Log.Debug("OnChatSend 未找到unit:" + msg.client_uid.srv_uid + ", " + msg.client_uid.conn_idx);
                return;
            }

            ChatManager.Instance.HandleSendChat(player, msg.type, msg.receiver, msg.chat_content);
        }
        #endregion
        #endregion
    }
}
