using System;
using System.Collections.Generic;

namespace dc.inner
{
    public class msg
    {
        public const ushort Begin = ProtocolID.MSG_BASE_INTERNAL;
        public const ushort REQ_LOGIN = Begin + 1;
        public const ushort REP_LOGIN = Begin + 2;
        public const ushort APPSERVER_LIST = Begin + 3;
        public const ushort APPSERVER_ADD = Begin + 4;
        public const ushort APPSERVER_REMOVE = Begin + 5;
        public const ushort APPSERVER_SHUTDOWN = Begin + 6;

        public static void RegisterPools()
        {
            //注册消息池
            PacketPools.Register(REQ_LOGIN, "inner.ReqLogin");
            PacketPools.Register(REP_LOGIN, "inner.RepLogin");
            PacketPools.Register(APPSERVER_LIST, "inner.AppServerList");
            PacketPools.Register(APPSERVER_ADD, "inner.AppServerAdd");
            PacketPools.Register(APPSERVER_REMOVE, "inner.AppServerRemove");
            PacketPools.Register(APPSERVER_SHUTDOWN, "inner.AppServerShutdown");
        }
    }
    public class ReqLogin : PacketBase
    {
        public AppServer srv_info = new AppServer();
        public ReqLogin()//msg.REQ_LOGIN
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            srv_info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            srv_info.Write(by);
        }
    }
    public class RepLogin : PacketBase
    {
        public enum eResult { UNKNOW, SUCCESS, FAILED }
        public AppServer srv_info = new AppServer();
        public eResult result = eResult.UNKNOW;
        public long ws_time = 0;//ws时间，同步给其他服务器
        public RepLogin()//msg.REP_LOGIN
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            srv_info.Read(by);
            result = (eResult)by.ReadUInt();
            ws_time = by.ReadLong();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            srv_info.Write(by);
            by.WriteUInt((uint)result);
            by.WriteLong(ws_time);
        }
    }
    public class AppServerList : PacketBase
    {
        public List<AppServerItem> list = new List<AppServerItem>();
        public AppServerList()
        {
        }
        public override void Init()
        {
            list.Clear();
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            list.Clear();
            ushort count = by.ReadUShort();
            for (int i = 0; i < count; ++i)
            {
                AppServerItem obj = new AppServerItem();
                obj.Read(by);
                list.Add(obj);
            }
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            ushort count = (ushort)list.Count;
            by.WriteUShort(count);
            for (int i = 0; i < count; ++i)
            {
                AppServerItem obj = list[i];
                obj.Write(by);
            }
        }
    }
    public class AppServerAdd : PacketBase
    {
        public AppServerItem app_info = new AppServerItem();
        public AppServerAdd()//msg.APPSERVER_ADD
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            app_info.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            app_info.Write(by);
        }
    }
    public class AppServerRemove : PacketBase
    {
        public ushort srv_uid;
        public AppServerRemove()//msg.APPSERVER_REMOVE
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            srv_uid = by.ReadUShort();
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUShort(srv_uid);
        }
    }
    /// <summary>
    /// 关服
    /// </summary>
    public class AppServerShutdown : PacketBase
    {
        public AppServerShutdown()
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
        }
    }
}
