using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 聊天管理器
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public class ChatDataManager : Singleton<ChatDataManager>
    {
        private List<ChatInfo> m_chat_infoes = new List<ChatInfo>();
        
        public void Setup()
        {

        }

        public void Destroy()
        {
            m_chat_infoes.Clear();
        }

        public void AddChat(eChatType type, CharacterIdxName sender, string content)
        {
            ChatInfo info = new ChatInfo();
            info.type = type;
            info.sender = sender;
            info.content = content;
            m_chat_infoes.Add(info);
        }

        public List<ChatInfo> chat_infoes
        {
            get { return m_chat_infoes; }
        }
    }

    public class ChatInfo
    {
        public eChatType type;
        public CharacterIdxName sender;
        public string content;
    }
}
