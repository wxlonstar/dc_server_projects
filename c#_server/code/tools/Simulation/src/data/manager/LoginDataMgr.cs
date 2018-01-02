using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 登录数据
    /// @author hannibal
    /// @time 2016-8-28
    /// </summary>
    public class LoginDataMgr : Singleton<LoginDataMgr>
    {
        private List<CharacterLogin> m_character_list = new List<CharacterLogin>();

        public void Setup()
        {

        }

        public void Destroy()
        {
            m_character_list.Clear();
        }

        public void AddCharacterList(List<CharacterLogin> list)
        {
            m_character_list.Clear();
            foreach(var obj in list)
            {
                CharacterLogin data = obj;
                m_character_list.Add(data);
            }
        }
        public CharacterLogin GetCharacterByIndex(int index)
        {
            if (index < 0 || index >= m_character_list.Count)
                throw new ArgumentException();
            else
                return m_character_list[index];
        }
        public List<CharacterLogin> character_list
        {
            get { return m_character_list; }
        }
    }
}
