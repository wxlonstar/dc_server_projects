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
        public void HandleSendChat(Unit unit, eChatType type, CharacterIdxOrName receiver, string content)
        {
            switch(type)
            {
                case eChatType.PRIVATE://私聊
                    Unit target = null;
                    if (receiver.IsIdxValid())
                    {
                        // 不能对自己发送
                        if (receiver.char_idx == unit.char_idx) return;
                        target = UnitManager.Instance.GetUnitByIdx(receiver.char_idx);
                    }
                    else
                    {
                        if (receiver.char_name == unit.char_name) return;
                        target = UnitManager.Instance.GetUnitByName(receiver.char_name);
                    }

                    if(target != null)
                    {//直接发给对方
                        ss2c.ChatRecv msg = PacketPools.Get(ss2c.msg.CHAT_RECV) as ss2c.ChatRecv;
                        msg.type = type;
                        msg.sender.Set(unit.char_idx, unit.char_name);
                        msg.chat_content = content;
                        ServerNetManager.Instance.SendProxy(target.client_uid, msg);
                    }
                    else
                    {//告诉对方不在线
                        ss2c.ChatResult msg = PacketPools.Get(ss2c.msg.CHAT_RESULT) as ss2c.ChatResult;
                        msg.error = eChatError.ERROR_DST_OFFLINE;
                        ServerNetManager.Instance.SendProxy(unit.client_uid, msg);
                    }

                    break;

                case eChatType.GROUP://组队聊天

                    break;

                case eChatType.GUILD://军团聊天

                    break;
            }
        }
        #endregion
    }
}
