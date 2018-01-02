using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 聊天
    /// @author hannibal
    /// @time 2017-8-14
    /// </summary>
    public class ChatManager : Singleton<ChatManager>
    {
        public ChatManager()
        {
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
        }
        public void Tick()
        {
        }
        #region 事件
        private void RegisterEvent()
        {
            EventController.AddEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.AddEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void UnRegisterEvent()
        {
            EventController.RemoveEventListener(EventID.PLAYER_ENTER_GAME, OnGameEvent);
            EventController.RemoveEventListener(EventID.PLAYER_LEAVE_GAME, OnGameEvent);
        }
        private void OnGameEvent(GameEvent evt)
        {
            switch (evt.type)
            {
                case EventID.PLAYER_ENTER_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogin(char_idx);
                    }
                    break;
                case EventID.PLAYER_LEAVE_GAME:
                    {
                        long char_idx = evt.Get<long>(0);
                        this.OnPlayerLogout(char_idx);
                    }
                    break;
            }
        }
        /// <summary>
        /// 玩家登录
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
        }
        /// <summary>
        /// 玩家登出
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
        }
        #endregion
        #region 聊天
        /// <summary>
        /// 处理客户端发送的聊天信息
        /// </summary>
        public void HandleSendChat(Player player, eChatType type, CharacterIdxOrName receiver, string content)
        {
            //TODO:是否禁言

            switch(type)
            {
                case eChatType.PRIVATE://私聊
                    Player target = null;
                    if (receiver.IsIdxValid())
                    {
                        // 不能对自己发送
                        if (receiver.char_idx == player.char_idx) return;
                        target = UnitManager.Instance.GetPlayerByIdx(receiver.char_idx);
                    }
                    else
                    {
                        if (receiver.char_name == player.char_name) return;
                        target = UnitManager.Instance.GetPlayerByName(receiver.char_name);
                    }

                    if(target != null)
                    {//如果本服玩家在线，直接发送
                        //检测是否在对方黑名单
                        if (RelationManager.Instance.HaveRelationFlag(player.char_idx, target.char_idx, eRelationFlag.Block))
                        {
                            ss2c.ChatResult msg = PacketPools.Get(ss2c.msg.CHAT_RESULT) as ss2c.ChatResult;
                            msg.error = eChatError.ERROR_DST_REFUSE;
                            ServerNetManager.Instance.SendProxy(target.client_uid, msg);
                        }
                        else
                        {
                            //直接发给对方
                            ss2c.ChatRecv msg = PacketPools.Get(ss2c.msg.CHAT_RECV) as ss2c.ChatRecv;
                            msg.type = type;
                            msg.sender.Set(player.char_idx, player.char_name);
                            msg.chat_content = content;
                            ServerNetManager.Instance.SendProxy(player.client_uid, msg);
                        }
                    }
                    else
                    {//通过ws转发
                        ss2ws.ChatSend msg = PacketPools.Get(ss2ws.msg.CHAT_SEND) as ss2ws.ChatSend;
                        msg.type = type;
                        msg.sender.Set(player.char_idx, player.char_name);
                        msg.receiver = receiver;
                        msg.chat_content = content;
                        ServerNetManager.Instance.Send2WS(msg);
                    }

                    break;

                case eChatType.WORLD://世界聊天
                    {
                        ss2c.ChatRecv msg = PacketPools.Get(ss2c.msg.CHAT_RECV) as ss2c.ChatRecv;
                        msg.type = type;
                        msg.sender.Set(player.char_idx, player.char_name);
                        msg.chat_content = content;
                        ServerNetManager.Instance.BroadcastProxyMsg(msg);
                    }
                    break;

                case eChatType.CURRENT://当前聊天

                    break;

                case eChatType.GROUP://组队聊天

                    break;

                case eChatType.GUILD://军团聊天

                    break;

                case eChatType.INSTANCE://战场聊天

                    break;
            }
        }
        #endregion
    }
}
