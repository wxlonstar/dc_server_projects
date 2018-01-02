using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 协议id
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class ProtocolID
    {
	    //
	    //	网络层保留的协议号区间
	    //

	    public const int PROTOCOL_RESERVED_LOW		= 0;		//	net 保留的协议号，最小值
	    public const int PROTOCOL_RESERVED_HIGH		= 999;		//	net 保留的协议号，最大值
	    public const int MSG_APPLAYER_BASE			= 1000;		//	应用层协议起始号码
	    public const int MSG_APPLAYER_PER_INTERVAL	= 1000;		//  消息起始结束间隔


	    //	内部id
	    public const int MSG_BASE_INTERNAL	= MSG_APPLAYER_BASE + 100;

	    public const int MSG_BASE_C2GS		= MSG_APPLAYER_BASE + 1000;
	    public const int MSG_BASE_C2SS		= MSG_APPLAYER_BASE + 2000;
	    public const int MSG_BASE_C2WS		= MSG_APPLAYER_BASE + 3000;
	    public const int MSG_BASE_C2FS		= MSG_APPLAYER_BASE + 4000;

        public const int MSG_BASE_GS2C      = MSG_APPLAYER_BASE + 5000;
        public const int MSG_BASE_GS2SS     = MSG_APPLAYER_BASE + 6000;
        public const int MSG_BASE_GS2WS     = MSG_APPLAYER_BASE + 7000;
        public const int MSG_BASE_GS2FS     = MSG_APPLAYER_BASE + 8000;

        public const int MSG_BASE_SS2C      = MSG_APPLAYER_BASE + 10000;
        public const int MSG_BASE_SS2GS     = MSG_APPLAYER_BASE + 11000;
        public const int MSG_BASE_SS2WS     = MSG_APPLAYER_BASE + 12000; 
        public const int MSG_BASE_SS2FS     = MSG_APPLAYER_BASE + 13000; 

        public const int MSG_BASE_WS2C      = MSG_APPLAYER_BASE + 15000;
        public const int MSG_BASE_WS2GS     = MSG_APPLAYER_BASE + 16000;
        public const int MSG_BASE_WS2SS     = MSG_APPLAYER_BASE + 17000;
        public const int MSG_BASE_WS2FS     = MSG_APPLAYER_BASE + 18000; 
        
        public const int MSG_BASE_FS2C      = MSG_APPLAYER_BASE + 20000;
        public const int MSG_BASE_FS2GS     = MSG_APPLAYER_BASE + 21000; 
        public const int MSG_BASE_FS2SS     = MSG_APPLAYER_BASE + 22000;
        public const int MSG_BASE_FS2WS     = MSG_APPLAYER_BASE + 23000;    

        public const int MSG_BASE_SS2GL     = MSG_APPLAYER_BASE + 25000;    //全局服
        public const int MSG_BASE_GL2SS     = MSG_APPLAYER_BASE + 26000;

        /// <summary>
        /// 注册消息池
        /// </summary>
        public static void RegisterPools()
        {
            inner.msg.RegisterPools();
            c2gs.msg.RegisterPools();
            c2ss.msg.RegisterPools();
            c2fs.msg.RegisterPools();
            c2ws.msg.RegisterPools();
            gs2c.msg.RegisterPools();
            gs2ss.msg.RegisterPools();
            gs2fs.msg.RegisterPools();
            gs2ws.msg.RegisterPools();
            ss2c.msg.RegisterPools();
            ss2gs.msg.RegisterPools();
            ss2fs.msg.RegisterPools();
            ss2ws.msg.RegisterPools();
            fs2c.msg.RegisterPools();
            fs2gs.msg.RegisterPools();
            fs2ss.msg.RegisterPools();
            fs2ws.msg.RegisterPools();
            ws2c.msg.RegisterPools();
            ws2gs.msg.RegisterPools();
            ws2ss.msg.RegisterPools();
            ws2fs.msg.RegisterPools();
            ss2gl.msg.RegisterPools();
            gl2ss.msg.RegisterPools();
        }
    }
    /// <summary>
    /// 可序列化对象
    /// </summary>
    public interface ISerializeObject
    {
        void Read(ByteArray by);
        void Write(ByteArray by);
    }
    public class PacketBase : ISerializeObject
    {
        public ushort header { get; set; }

        public PacketBase()
        {
        }
        public virtual void Init()
        {
            //初始化数据：packet属于对象池，重用时需要把上次数据重置
        }
        public virtual void Read(ByteArray by)
        {
            //undo
        }
        public virtual void Write(ByteArray by)
        {
            by.WriteUShort(header);
        }
    }
    public class PackBaseC2S : PacketBase
    {
        public uint packet_idx = 0;
        public ushort data_verify = 0;
        public ClientUID client_uid;
        public PackBaseC2S()
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            //内容在ClientMsgProc::OnNetworkClient已经读取
            //packet_idx = by.ReadUShort();
            //data_verify = by.ReadUShort();
            client_uid.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteUInt(packet_idx);
            by.WriteUShort(data_verify);
            client_uid.Write(by);
        }
    }
    public class PackBaseS2C : PacketBase
    {
        public ClientUID client_uid;
        public PackBaseS2C()
        {
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            client_uid.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            client_uid.Write(by);
        }
    }
    public class PackBaseS2S : PacketBase
    {
        public InterServerID server_uid;
        public PackBaseS2S()
        {

        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            server_uid.Read(by);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            server_uid.Write(by);
        }
    }
    /// <summary>
    /// 注：使用代理发送消息给其他server的数据结构 必须继承PackBaseC2S
    /// </summary>
    public class ProxyC2SMsg : PacketBase
    {
        public ByteArray data;
        public ProxyC2SMsg()
        {
            data = new ByteArray(NetID.SendPacketMaxSize, NetID.SendPacketMaxSize);
        }
        public override void Init()
        {
            base.Init();
            data.Clear();
        }
        public void Set(ClientUID client_uid, ushort header, ByteArray by)
        {
            data.WriteUShort(header);//必须填入id
            client_uid.Write(data);
            by.Skip(ClientUID.Size());//由于by是C2S消息，带ClientUID，所以跳过
            by.Read(data, by.Available);
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            by.Read(data, by.Available);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.Write(data.GetBuffer(), data.Available);
        }
    }
    /// <summary>
    /// 注：使用代理发送消息给客户端的数据结构 必须继承PackBaseSTC
    /// </summary>
    public class ProxyS2CMsg : PacketBase
    {
        public ByteArray data;
        public bool is_broadcast = false;// 是否是广播消息
        public ProxyS2CMsg()
        {
            data = new ByteArray(NetID.SendPacketMaxSize, NetID.SendPacketMaxSize);
        }
        public override void Init()
        {
            base.Init();
            data.Clear();
        }
        public void Set(ClientUID client_uid, PackBaseS2C packet)
        {
            if (!is_broadcast && (client_uid.srv_uid == 0 || client_uid.conn_idx == 0))
                Console.WriteLine("未设置转发目标服务器数据");
            packet.client_uid = client_uid;
            packet.Write(data);
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            is_broadcast = by.ReadBool();
            by.Read(data, by.Available);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.WriteBool(is_broadcast);
            by.Write(data.GetBuffer(), data.Available);
        }
    }

    public class ProxyS2SMsg : PacketBase
    {
        public ByteArray data;
        public ProxyS2SMsg()
        {
            data = new ByteArray(NetID.SendPacketMaxSize, NetID.SendPacketMaxSize);
        }
        public override void Init()
        {
            base.Init();
            data.Clear();
        }
        public void Set(InterServerID server_uid, PackBaseS2S packet)
        {
            packet.server_uid = server_uid;
            packet.Write(data);
        }
        public override void Read(ByteArray by)
        {
            base.Read(by);
            by.Read(data, by.Available);
        }
        public override void Write(ByteArray by)
        {
            base.Write(by);
            by.Write(data.GetBuffer(), data.Available);
        }
    }
}
