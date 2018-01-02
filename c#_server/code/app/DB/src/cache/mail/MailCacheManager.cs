using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 邮件管理器
    /// @author hannibal
    /// @time 2017-9-11
    /// </summary>
    public class MailCacheManager : Singleton<MailCacheManager>
    {
        private Dictionary<long, MailboxCache> m_mailboxs = null;

        public MailCacheManager()
        {
            m_mailboxs = new Dictionary<long, MailboxCache>();
        }

        public void Setup()
        {
        }
        public void Destroy()
        {
            foreach (var obj in m_mailboxs)
            {
                CommonObjectPools.Despawn(obj.Value);
            }
            m_mailboxs.Clear();
        }
        public void Tick()
        {
            int update_count = 0;
            MailboxCache mail_box = null;
            foreach (var obj in m_mailboxs)
            {
                mail_box = obj.Value;
                if (mail_box.NeedSave())
                {
                    mail_box.Save();
                    if (++update_count > 60) break;//当次循环最大保存数量
                }
            }
        }
        /// <summary>
        /// 读取玩家邮箱
        /// </summary>
        /// <param name="char_idx"></param>
        public void LoadMailbox(long char_idx)
        {
            if (m_mailboxs.ContainsKey(char_idx)) return;

            MailboxCache mail_box = CommonObjectPools.Spawn<MailboxCache>();
            if (mail_box.Setup(char_idx))
            {
                m_mailboxs.Add(char_idx, mail_box);
            }
            else
            {
                CommonObjectPools.Despawn(mail_box);
            }
        }
        /// <summary>
        /// 玩家登出
        /// </summary>
        public void HanldeLogoutClient(long char_idx)
        {
            MailboxCache mail_box;
            if (m_mailboxs.TryGetValue(char_idx, out mail_box))
            {
                mail_box.Destroy();
                CommonObjectPools.Despawn(char_idx);
            }
            m_mailboxs.Remove(char_idx);
        }
        /// <summary>
        /// 获取玩家邮箱
        /// </summary>
        /// <param name="char_idx"></param>
        public MailboxCache GetMailbox(long char_idx)
        {
            MailboxCache mail_box;
            if (m_mailboxs.TryGetValue(char_idx, out mail_box))
            {
                return mail_box;
            }
            return null;
        }
    }
}
