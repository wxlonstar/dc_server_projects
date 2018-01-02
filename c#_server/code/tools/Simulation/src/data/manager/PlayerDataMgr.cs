using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 玩家数据
    /// @author hannibal
    /// @time 2016-8-28
    /// </summary>
    public class PlayerDataMgr : Singleton<PlayerDataMgr>
    {
        private long m_main_player_id = 0;
        private PlayerInfoForClient m_main_player_info;

        public PlayerDataMgr()
        {
        }

        public void Setup()
        {
            m_main_player_id = 0;
        }
        public void Destroy()
        {
        }
        public long main_player_id 
        { 
            get { return m_main_player_id; }
            set { m_main_player_id = value; }
        }
        public PlayerInfoForClient main_player_info
        {
            get { return m_main_player_info; }
            set { m_main_player_info = value; }
        }
    }
}
