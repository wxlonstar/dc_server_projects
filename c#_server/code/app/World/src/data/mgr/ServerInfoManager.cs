using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 管理ws以及其他服信息
    /// @author hannibal
    /// @time 2016-7-27
    /// </summary>
    public class ServerInfoManager : Singleton<ServerInfoManager>
    {
        //连接的服务器
        private Dictionary<ushort, RemoteServerInfo> m_remote_servers = null;

        public ServerInfoManager()
        {
            m_remote_servers = new Dictionary<ushort, RemoteServerInfo>();
        }
                
        public void Setup()
        {
        }
        public void Destroy()
        {
            this.ClearRemoteServer();
        }
        #region 远程服务器
        public void AddRemoteServer(RemoteServerInfo info)
        {
            this.RemoveRemoteServer(info.srv_uid);
            m_remote_servers.Add(info.srv_uid, info);
        }
        public void RemoveRemoteServer(ushort srv_uid)
        {
            m_remote_servers.Remove(srv_uid);
        }
        public void ClearRemoteServer()
        {
            m_remote_servers.Clear();
        }
        public void UpdatePlayerCount(ushort srv_uid, ushort count)
        {
            RemoteServerInfo info;
            if(m_remote_servers.TryGetValue(srv_uid, out info))
            {
                info.player_count = count;
            }
        }
        public string GetRemoteOnlineCount()
        {
            string str = "";
            foreach(var obj in m_remote_servers)
            {
                str += ("type:" + obj.Value.type + " srv_uid:" + obj.Value.srv_uid + " count:" + obj.Value.player_count + "\n");
            }
            return str;
        }
        #endregion
    }
    /// <summary>
    /// 其他服务器信息
    /// </summary>
    public class RemoteServerInfo
    {
        public ushort srv_uid = 0;      //服务器id
        public eServerType type;        //类型
        public string ip = "";          //加入机器ip
        public long start_time = 0;     //开启时间
        public ushort player_count = 0; //在线数量
    }
}
