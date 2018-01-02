using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 协议加解密
    /// @author hannibal
    /// @time 2016-5-25
    /// </summary>
    public class PacketEncrypt
    {
        /// <summary>
        /// 传送给客户端的key
        /// </summary>
        public static ushort Encrypt_Key = (ushort)MathUtils.RandRange(1, ushort.MaxValue);

        /// <summary>
        /// 数据包发送索引
        /// </summary>
        private static uint m_packet_index = 0;
        public static uint GetPacketIndex()
        {
            int tv = (int)(TimeUtils.TimeSince2009 & 0x03);
            m_packet_index += (uint)MathUtils.Max((long)1, (long)tv);
            return m_packet_index;
        }

        /// <summary>
        /// 计算数据包内容效验值函数：只有客户端发给gate的消息才做校验
        /// </summary>
        public static ushort CalcPacketDataVerify(byte[] buf, int idx, int size, uint packet_idx, ushort key)
        {
            ushort ret = 0x9BCE;
            byte bu = (byte)(ret & 0xFF);
            byte bv = (byte)((ret >> 8) & 0xFF);
            int index = idx;
            while (size > 0)
            {
                bu ^= buf[index];
                bv ^= bu;
                size--;
                index++;
            }
            ret = (ushort)((ushort)bu << 0);
            ret |= (ushort)((ushort)bv << 8);
            ret ^= key;
            ret ^= (ushort)~(size);
            ret ^= (ushort)(packet_idx & 0xFFFF);
            return ret;
        }
        public static uint EncrpytPacketIndex(uint idx, ushort key)
        {
            uint ret = 0x9B1E;
            ret = idx ^ key;
            return ret;
        }
        public static uint DecrpytPacketIndex(uint idx, ushort key)
        {
            uint ret = 0x9B1E;
            ret = idx ^ key;
            return ret;
        }
        public static byte[] EncryptPacket(ref byte[] buf, int index, uint dwSize, ushort uKey)
        {
            uint idx = (uint)index;
            while (dwSize > 0)
            {
                buf[idx] ^= (byte)(uKey ^ 0xBA);
                idx++;
                dwSize--;
            }
            return buf;
        }
        public static byte[] DecryptPacket(ref byte[] buf, int index, uint dwSize, ushort uKey)
        {
            uint idx = (uint)index;
            while (dwSize > 0)
            {
                buf[idx] ^= (byte)(uKey ^ 0xBA);
                idx++;
                dwSize--;
            }
            return buf;
        }
    }
}
