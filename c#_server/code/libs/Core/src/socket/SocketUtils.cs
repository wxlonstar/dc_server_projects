using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace dc
{
    public class SocketUtils
    {
        public const int SendRecvMaxSize = 4096;

        /// <summary>
        /// 保活
        /// TODO:网上找的，据说很牛逼，是否有效需要验证
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="keepalive_time">多长时间后开始第一次探测（单位：毫秒）</param>
        /// <param name="keepalive_interval">第一次探测失败后，后续探测时间间隔（单位：毫秒）</param>
        public static void SetKeepAlive(Socket socket, ulong keepalive_time = 5000, ulong keepalive_interval = 3000)
        {
            if (socket == null) return;

            int bytes_per_long = 32 / 8;
            byte[] keep_alive = new byte[3 * bytes_per_long];
            ulong[] input_params = new ulong[3];
            int i1;
            int bits_per_byte = 8;

            if (keepalive_time == 0 || keepalive_interval == 0)
                input_params[0] = 0;
            else
                input_params[0] = 1;
            input_params[1] = keepalive_time;
            input_params[2] = keepalive_interval;
            for (i1 = 0; i1 < input_params.Length; i1++)
            {
                keep_alive[i1 * bytes_per_long + 3] = (byte)(input_params[i1] >> ((bytes_per_long - 1) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 2] = (byte)(input_params[i1] >> ((bytes_per_long - 2) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 1] = (byte)(input_params[i1] >> ((bytes_per_long - 3) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 0] = (byte)(input_params[i1] >> ((bytes_per_long - 4) * bits_per_byte) & 0xff);
            }
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keep_alive);
        }
        /// <summary>
        /// ip和数字转换
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static uint IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return uint.Parse(items[0]) << 24
                    | uint.Parse(items[1]) << 16
                    | uint.Parse(items[2]) << 8
                    | uint.Parse(items[3]);
        }
        private static StringBuilder ip_sb = new StringBuilder();
        public static string IntToIp(uint ipInt)
        {
            ip_sb.Clear();
            ip_sb.Append((ipInt >> 24) & 0xFF).Append(".");
            ip_sb.Append((ipInt >> 16) & 0xFF).Append(".");
            ip_sb.Append((ipInt >> 8) & 0xFF).Append(".");
            ip_sb.Append(ipInt & 0xFF);
            return ip_sb.ToString();
        }
        /// <summary>
        /// ip格式是否正确
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsValidIP(string ip)
        {
            if(ip == null || ip.Length == 0)return false;
            return Regex.IsMatch(ip, @"^(?:(?:1[0-9][0-9]\.)|(?:2[0-4][0-9]\.)|(?:25[0-5]\.)|(?:[1-9][0-9]\.)|(?:[0-9]\.)){3}(?:(?:1[0-9][0-9])|(?:2[0-4][0-9])|(?:25[0-5])|(?:[1-9][0-9])|(?:[0-9]))$");
        }
    }
}
