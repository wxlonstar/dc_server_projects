using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 邮件管理器
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public class MailboxManager : Singleton<MailboxManager>
    {
        private Dictionary<long, Mailbox> m_mailboxs = null;

        public MailboxManager()
        {
            m_mailboxs = new Dictionary<long, Mailbox>();
        }

        public void Setup()
        {
            this.RegisterEvent();
        }
        public void Destroy()
        {
            this.UnRegisterEvent();
            foreach (var obj in m_mailboxs)
            {
                obj.Value.Destroy();
                CommonObjectPools.Despawn(obj.Value);
            }
            m_mailboxs.Clear();
        }
        public void Tick()
        {
            //存盘
            int update_count = 0;
            Mailbox mail_box = null;
            foreach (var obj in m_mailboxs)
            {
                mail_box = obj.Value as Mailbox;
                if (mail_box != null && mail_box.NeedSave())
                {
                    mail_box.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
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
        /// 给玩家创建邮箱
        /// </summary>
        private void OnPlayerLogin(long char_idx)
        {
            Mailbox mail_box;
            if (!m_mailboxs.TryGetValue(char_idx, out mail_box))
            {
                mail_box = CommonObjectPools.Spawn<Mailbox>();
                mail_box.Setup(char_idx);
                m_mailboxs.Add(char_idx, mail_box);
            }
        }
        /// <summary>
        /// 玩家登出
        /// </summary>
        private void OnPlayerLogout(long char_idx)
        {
            Mailbox mail_box;
            if (m_mailboxs.TryGetValue(char_idx, out mail_box))
            {
                mail_box.Destroy();
                CommonObjectPools.Despawn(mail_box);
            }
            m_mailboxs.Remove(char_idx);
        }
        #endregion
        /// <summary>
        /// 获取玩家邮箱
        /// </summary>
        public Mailbox GetMailBox(long char_idx)
        {
            Mailbox mail_box;
            if (m_mailboxs.TryGetValue(char_idx, out mail_box))
            {
                return mail_box;
            }
            return null;
        }
    }
}
