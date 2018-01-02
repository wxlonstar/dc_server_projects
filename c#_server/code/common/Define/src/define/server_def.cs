using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 内部服务器id
    /// </summary>
    public struct InterServerID : ISerializeObject
    {
        public ushort gs_uid;
        public ushort ss_uid;
        public ushort fs_uid;
        public ushort ws_uid;
        public void Read(ByteArray by)
        {
            gs_uid = by.ReadUShort();
            ss_uid = by.ReadUShort();
            fs_uid = by.ReadUShort();
            ws_uid = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteUShort(gs_uid);
            by.WriteUShort(ss_uid);
            by.WriteUShort(fs_uid);
            by.WriteUShort(ws_uid);
        }
    }
    /// <summary>
    /// 服务器连接信息
    /// </summary>
    public class AppServer : ISerializeObject
    {
        public eServerType srv_type = 0;
        public ushort srv_realm_idx = 0;    //服务器大区id：世界服分配
        public ushort srv_uid = 0;          //服务器id：世界服分配
        public ServiceEndpoint srv_endpoint = new ServiceEndpoint();
        public eConnAppStatus srv_status = eConnAppStatus.CREATED;

        public void Read(ByteArray by)
        {
            srv_type = (eServerType)by.ReadUInt();
            srv_realm_idx = by.ReadUShort();
            srv_uid = by.ReadUShort();
            srv_endpoint.Read(by);
        }
        public void Write(ByteArray by)
        {
            by.WriteUInt((uint)srv_type);
            by.WriteUShort(srv_realm_idx);
            by.WriteUShort(srv_uid);
            srv_endpoint.Write(by);
        }
    }
    /// <summary>
    /// 服务器连接信息
    /// </summary>
    public struct AppServerItem : ISerializeObject
    {
        public ushort srv_uid;
        public eServerType srv_type;
        public uint srv_ip;
        public ushort srv_port;

        public void Read(ByteArray by)
        {
            srv_uid = by.ReadUShort();
            srv_type = (eServerType)by.ReadUInt();
            srv_ip = by.ReadUInt();
            srv_port = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteUShort(srv_uid);
            by.WriteUInt((uint)srv_type);
            by.WriteUInt(srv_ip);
            by.WriteUShort(srv_port);
        }
    }

    // ClietSession 的 UID 定义
    // 1. 此 UID 与 AccountIndex、CharacterIndex 无关，只用于标识唯一的一次客户端、服务器会话
    // 2. 此 UID 由gate server 生成
    public struct ClientUID : ISerializeObject
    {
        public ushort srv_uid;      // 所属服务器uid
        public long conn_idx;       // client 唯一在gate的id
        public ClientUID(ushort _srv_uid, long _conn_idx)
        {
            srv_uid = _srv_uid; conn_idx = _conn_idx;
        }
        public static bool operator ==(ClientUID c_uid1, ClientUID c_uid2)
        {
            return c_uid1.srv_uid == c_uid2.srv_uid && c_uid1.conn_idx == c_uid2.conn_idx;
        }
        public static bool operator !=(ClientUID c_uid1, ClientUID c_uid2)
        {
            return c_uid1.srv_uid != c_uid2.srv_uid || c_uid1.conn_idx != c_uid2.conn_idx;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public void Read(ByteArray by)
        {
            srv_uid = by.ReadUShort();
            conn_idx = by.ReadLong();
        }
        public void Read(byte[] by)
        {
            srv_uid = BitConverter.ToUInt16(by, 0);
            conn_idx = BitConverter.ToInt64(by, 2);
        }
        public void Write(ByteArray by)
        {
            by.WriteUShort(srv_uid);
            by.WriteLong(conn_idx);
        }
        public static int Size()
        {
            return (2 + 8);
        }
    }
    /// <summary>
    /// ip
    /// </summary>
    public struct ServiceEndpoint : ISerializeObject
    {
        public uint ip;
        public ushort port;
        public void Read(ByteArray by)
        {
            ip = by.ReadUInt();
            port = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteUInt(ip);
            by.WriteUShort(port);
        }
        public void Set(uint _ip, ushort _port)
        {
            ip = _ip; port = _port;
        }
    }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public enum eServerType
    {
        NONE = 0,
        WORLD,  			    // 世界、关系服
        FIGHT,                  // 战斗服
        SERVER,			        // 逻辑服
        GATE,	                // 网关
        GLOBAL,                 // 全局服
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public enum eServerStatus
    {
        NOT_CONNECTED = 0,	    // 服务器未连接
        CONNECTING,			    // 连接中
        CONNECTED,			    // 服务器已连接
        WORKING,				// 已经开始工作
        DISCONNECTED,		    // 断开
    }

    /// <summary>
    /// 服务器连接状态
    /// </summary>
    public enum eConnAppStatus
    {
        CREATED = 0,
        CONNECTING,
        CONNECTED,
        CLOSED,
    }
}
