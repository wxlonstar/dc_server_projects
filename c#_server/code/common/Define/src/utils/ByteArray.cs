using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 字节数组
    /// @author hannibal
    /// @time 2016-5-23
    /// </summary>
    public class ByteArray
    {
        protected byte[] m_Buffer;

        protected int m_BufferLen;
        protected int m_MaxBufferLen;

        protected int m_Head;
        protected int m_Tail;

        public ByteArray(int BufferSize, int MaxBufferSize)
        {
            m_Head = 0;
            m_Tail = 0;

            m_BufferLen = BufferSize;
            m_MaxBufferLen = MaxBufferSize;

            m_Buffer = new byte[m_BufferLen];
        }

        public bool Resize(int size)
        {
            size = Math.Max(size, (int)(m_BufferLen >> 1));
            int newBufferLen = m_BufferLen + size;
            if (newBufferLen > m_MaxBufferLen)
            {
                Console.WriteLine("ByteArray::Resize 缓冲区溢出:" + newBufferLen);
                newBufferLen = m_MaxBufferLen;
            }

            int len = Available;
            if (size < 0)
            {
                if (newBufferLen < len) return false;
            }

            byte[] newBuffer = new byte[newBufferLen];
            if (m_Head < m_Tail)
            {
                Array.Copy(m_Buffer, m_Head, newBuffer, 0, m_Tail - m_Head);
            }
            else if (m_Head > m_Tail)
            {
                Array.Copy(m_Buffer, m_Head, newBuffer, 0, m_BufferLen - m_Head);
                Array.Copy(m_Buffer, 0, newBuffer, m_BufferLen - m_Head, m_Tail);
            }

            m_Buffer = newBuffer;
            m_BufferLen = newBufferLen;
            m_Head = 0;
            m_Tail = len;

            return true;
        }
        public void Clear()
        {
            m_Head = 0;
            m_Tail = 0;
        }

        private byte[] val = new byte[8];
        public byte ReadByte()
        {
            Read(ref val, 1);
            return val[0];
        }
        public bool ReadBool()
        {
            Read(ref val, 1);
            return val[0] == 0 ? false : true;
        }
        public short ReadShort()
        {
            Read(ref val, 2);
            return BitConverter.ToInt16(val, 0);
        }
        public ushort ReadUShort()
        {
            Read(ref val, 2);
            return BitConverter.ToUInt16(val, 0);
        }
        public int ReadInt()
        {
            Read(ref val, 4);
            return BitConverter.ToInt32(val, 0);
        }
        public uint ReadUInt()
        {
            Read(ref val, 4);
            return BitConverter.ToUInt32(val, 0);
        }
        public long ReadLong()
        {
            Read(ref val, 8);
            return BitConverter.ToInt64(val, 0);
        }
        public ulong ReadUlong()
        {
            Read(ref val, 8);
            return BitConverter.ToUInt64(val, 0);
        }
        public float ReadFloat()
        {
            Read(ref val, 4);
            return BitConverter.ToSingle(val, 0);
        }
        public double ReadDouble()
        {
            Read(ref val, 8);
            return BitConverter.ToDouble(val, 0);
        }
        public string ReadString()
        {
            ushort length = 0;
            length = ReadUShort();
            byte[] by = new byte[length];
            Read(ref by, length);
            string val = Encoding.UTF8.GetString(by, 0, length);
            return val;
        }
        public char[] ReadChars()
        {
            ushort length = 0;
            length = ReadUShort();
            byte[] by = new byte[length];
            Read(ref by, length);
            char[] ch = Encoding.UTF8.GetChars(by, 0, length);
            return ch;
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="buf">接收缓冲区</param>
        /// <param name="len">读取长度</param>
        /// <param name="index">存放缓冲区起始位置</param>
        /// <returns></returns>
        public int Read(ref byte[] buf, int len, int index = 0)
        {
            if (len == 0)
                return 0;

            if (len > Available)//不够数据
                return 0;

            if (m_Head < m_Tail)
            {
                Array.Copy(m_Buffer, m_Head, buf, index, len);
            }
            else
            {
                int rightLen = m_BufferLen - m_Head;
                if (len <= rightLen)
                {
                    Array.Copy(m_Buffer, m_Head, buf, index, len);
                }
                else
                {
                    Array.Copy(m_Buffer, m_Head, buf, index, rightLen);
                    Array.Copy(m_Buffer, 0, buf, rightLen + index, len - rightLen);
                }
            }

            m_Head = (m_Head + len) % m_BufferLen;

            return len;
        }
        /// <summary>
        /// 复制数据到by，为简化逻辑，约定:
        /// 1.by必须满足m_Head<=m_Tail
        /// 2.by必须有足够空间
        /// </summary>
        /// <param name="by"></param>
        /// <param name="len"></param>
        public int Read(ByteArray by, int len)
        {
            if (len <= 0)
                return 0;

            if (len > Available)//不够数据
                return 0;

            if (by.m_Head > by.m_Tail || (by.Capacity - by.Available) < len)//超出容量
                return 0;

            if (m_Head < m_Tail)
            {
                Array.Copy(m_Buffer, m_Head, by.m_Buffer, by.m_Tail, len);
            }
            else
            {
                int rightLen = m_BufferLen - m_Head;
                if (len <= rightLen)
                {
                    Array.Copy(m_Buffer, m_Head, by.m_Buffer, by.m_Tail, len);
                }
                else
                {
                    Array.Copy(m_Buffer, m_Head, by.m_Buffer, by.m_Tail, rightLen);
                    Array.Copy(m_Buffer, 0, by.m_Buffer, rightLen + by.m_Tail, len - rightLen);
                }
            }

            by.m_Tail += len;
            m_Head = (m_Head + len) % m_BufferLen;

            return len;
        }

        public int WriteByte(byte buf)
        {
            return Write(BitConverter.GetBytes(buf), 1);
        }
        public int WriteBool(bool buf)
        {
            int b = buf ? 1 : 0;
            return Write(BitConverter.GetBytes(b), 1);
        }
        public int WriteShort(short buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(short));
        }
        public int WriteUShort(ushort buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(ushort));
        }
        public int WriteInt(int buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(int));
        }
        public int WriteUInt(uint buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(uint));
        }
        public int WriteLong(long buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(long));
        }
        public int WriteULong(ulong buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(ulong));
        }
        public int WriteFloat(float buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(float));
        }
        public int WriteDouble(double buf)
        {
            return Write(BitConverter.GetBytes(buf), sizeof(double));
        }
        public int WriteBytes(byte[] buf, int len)
        {
            return Write(buf, len);
        }
        public void WriteString(string val)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val);
            ushort length = (ushort)bytes.Length;
            WriteUShort(length);
            WriteBytes(bytes, length);
        }
        public void WriteChars(char[] val, int index, int count)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(val, index, count);
            ushort length = (ushort)bytes.Length;
            WriteUShort(length);
            WriteBytes(bytes, length);
        }
        public int Write(byte[] buf, int len)
        {
            //					//
            //     T  H			//    H   T			LEN=10
            // 0123456789		// 0123456789
            // abcd...efg		// ...abcd...
            //					//
            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));
            if (len >= nFree)
            {
                if (!Resize(len - nFree + 1))
                    return 0;
            }

            if (m_Head <= m_Tail)
            {
                if (m_Head == 0)
                {
                    nFree = m_BufferLen - m_Tail - 1;
                    Array.Copy(buf, 0, m_Buffer, m_Tail, len);
                }
                else
                {
                    nFree = m_BufferLen - m_Tail;
                    if (len <= nFree)
                    {
                        Array.Copy(buf, 0, m_Buffer, m_Tail, len);
                    }
                    else
                    {
                        Array.Copy(buf, 0, m_Buffer, m_Tail, nFree);
                        Array.Copy(buf, nFree, m_Buffer, 0, len - nFree);
                    }
                }
            }
            else
            {
                Array.Copy(buf, 0, m_Buffer, m_Tail, len);
            }

            m_Tail = (m_Tail + len) % m_BufferLen;

            return len;
        }
        /// <summary>
        /// 写入空
        /// </summary>
        public int WriteEmpty(int len)
        {
            //					//
            //     T  H			//    H   T			LEN=10
            // 0123456789		// 0123456789
            // abcd...efg		// ...abcd...
            //					//
            int nFree = ((m_Head <= m_Tail) ? (m_BufferLen - m_Tail + m_Head - 1) : (m_Head - m_Tail - 1));
            if (len >= nFree)
            {
                if (!Resize(len - nFree + 1))
                    return 0;
            }

            m_Tail = (m_Tail + len) % m_BufferLen;

            return len;
        }

        public int ModifyByte(byte buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), 1, position);
        }
        public int ModifyBool(bool buf, int position)
        {
            int b = buf ? 1 : 0;
            return Modify(BitConverter.GetBytes(b), 1, position);
        }
        public int ModifyShort(short buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(short), position);
        }
        public int ModifyUShort(ushort buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(ushort), position);
        }
        public int ModifyInt(int buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(int), position);
        }
        public int ModifyUInt(uint buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(uint), position);
        }
        public int ModifyLong(long buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(long), position);
        }
        public int ModifyULong(ulong buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(ulong), position);
        }
        public int ModifyFloat(float buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(float), position);
        }
        public int ModifyDouble(double buf, int position)
        {
            return Modify(BitConverter.GetBytes(buf), sizeof(double), position);
        }
        public int ModifyBytes(byte[] buf, int len, int position)
        {
            return Modify(buf, len, position);
        }
        public void ModifyString(string val, int position)
        {
            if (val == null)
            {
                ModifyUShort(0, position);
                ModifyByte(((byte)'\0'), position+2);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(val);
                ushort length = (ushort)bytes.Length;
                ModifyUShort(length, position);
                ModifyBytes(bytes, length, position+2);
            }
        }
        public int Modify(byte[] buf, int len, int position)
        {
            //判断起始位置是否有效
            if (m_Head <= m_Tail && (position < m_Head || position > m_Tail))
                return 0;
            else if (m_Head > m_Tail && (position > m_Tail || position < m_Head))
                return 0;
            //判断需要修改的数据是否足够
            if(m_Head <= m_Tail)
            {
                if (position + len > m_Tail) return 0;
            }
            else
            {
                if (position + len - m_BufferLen > m_Head) return 0;
            }
            //写数据
            if (m_Head > m_Tail)
            {
                int nFree = m_BufferLen - position;
                if (len <= nFree)
                {
                    Array.Copy(buf, 0, m_Buffer, position, len);
                }
                else
                {
                    Array.Copy(buf, 0, m_Buffer, position, nFree);
                    Array.Copy(buf, nFree, m_Buffer, 0, len - nFree);
                }
            }
            else
            {
                Array.Copy(buf, 0, m_Buffer, position, len);
            }

            return len;
        }

        public bool Peek(ref byte[] buf, int len)
        {
            if (len == 0)
                return false;

            if (len > Available)
                return false;

            if (m_Head < m_Tail)
            {
                Array.Copy(m_Buffer, m_Head, buf, 0, len);
            }
            else
            {
                int rightLen = m_BufferLen - m_Head;
                if (len <= rightLen)
                {
                    Array.Copy(m_Buffer, m_Head, buf, 0, len);
                }
                else
                {
                    Array.Copy(m_Buffer, m_Head, buf, 0, rightLen);
                    Array.Copy(m_Buffer, 0, buf, rightLen, len - rightLen);
                }
            }

            return true;
        }

        public bool Skip(int len)
        {
            if (len == 0)
                return false;

            if (len > Available)
                return false;

            m_Head = (m_Head + len) % m_BufferLen;

            return true;
        }

        public int Head
        {
            get { return m_Head; }
        }
        public int Tail
        {
            get { return m_Tail; }
        }
        /// <summary>
        /// 设置读取位置
        /// </summary>
        /// <param name="pos"></param>
        public void SetHead(int pos)
        {
            if (pos < 0) return;
            pos = pos % m_BufferLen;

            if (m_Head < m_Tail)
            {
                if (pos < m_Head) return;
                else if (pos > m_Tail) return;
                m_Head = pos;
            }
            else if (m_Head > m_Tail)
            {
                if (pos > m_Tail && pos < m_Head) return;
                m_Head = pos;
            }
        }

        public int Available
        {
            get
            {
                if (m_Head < m_Tail)
                    return m_Tail - m_Head;
                else if (m_Head > m_Tail)
                    return m_BufferLen - m_Head + m_Tail;

                return 0;
            }
        }
        public int Capacity
        {
            get { return m_BufferLen; }
        }
        public byte[] GetBuffer()
        {
            return m_Buffer;
        }
        /// <summary>
        /// 这个谨慎使用，如果外部直接做修改，m_Head/m_Tail并不会改变，需要自己处理
        /// </summary>
        public byte[] Buffer
        {
            get { return m_Buffer; }
        }
    }
}
