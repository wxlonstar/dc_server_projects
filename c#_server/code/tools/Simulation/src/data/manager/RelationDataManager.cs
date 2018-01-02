using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 关系管理器
    /// @author hannibal
    /// @time 2016-9-11
    /// </summary>
    public class RelationDataManager : Singleton<RelationDataManager>
    {
        private Dictionary<long, RelationInfo> m_relations = new Dictionary<long, RelationInfo>();
        private List<RelationAddInfo> m_apply_relations = new List<RelationAddInfo>();
        
        public void Setup()
        {

        }

        public void Destroy()
        {
            m_relations.Clear();
            m_apply_relations.Clear();
        }

        public void AddRelation(List<RelationInfo> list)
        {
            foreach (var info in list)
            {
                m_relations.Remove(info.char_idx);
                m_relations.Add(info.char_idx, info);
            }
        }
        public void RemoveRelation(long char_idx)
        {
            m_relations.Remove(char_idx);
        }

        public void AddNewApplys(RelationAddInfo info)
        {
            m_apply_relations.ForEach((item) => { 
                if (item.player_id.char_idx == info.player_id.char_idx)
                    return; 
            });

            m_apply_relations.Add(info);
        }
        public void RemoveNewApplys(long event_idx)
        {
            for(int i = 0; i < m_apply_relations.Count; ++i)
            {
                if (m_apply_relations[i].event_idx == event_idx)
                {
                    m_apply_relations.RemoveAt(i);
                    break;
                }
            }
        }

        public Dictionary<long, RelationInfo> relations
        {
            get { return m_relations; }
        }
        public List<RelationAddInfo> apply_relations
        {
            get { return m_apply_relations; }
        }
    }
    /// <summary>
    /// 请求加好友
    /// </summary>
    public struct RelationAddInfo
    {
        public long event_idx;
        public PlayerIDName player_id;
        public string message;
        public eRelationFlag flag;
    }
}
