using System.Collections;
using System.Runtime.InteropServices;
using System;


/// <summary>
/// 字节处理
/// @author hannibal
/// @time 2014-12-1
/// </summary>
public class ByteUtils 
{
	public static byte[] StructToBytes(object structType)
	{
		
		int size = Marshal.SizeOf(structType);
		
		byte[] bytes = new byte[size];
		
		IntPtr structPtr = Marshal.AllocHGlobal(size);
		
		Marshal.StructureToPtr(structType, structPtr, false);
		
		Marshal.Copy(structPtr, bytes, 0, size);
		
		Marshal.FreeHGlobal(structPtr);
		
		return bytes;
		
	}

    public static T BytesToStruct<T>(byte[] bytes, Type type)
	{
		T obj = default(T);
		
		int size = Marshal.SizeOf(type);
		
		if (size > bytes.Length)
		{
			return obj;
		}
		
		IntPtr structPtr = Marshal.AllocHGlobal(size);
		
		Marshal.Copy(bytes, 0, structPtr, size);
		
		obj = (T)Marshal.PtrToStructure(structPtr, type);
		
		Marshal.FreeHGlobal(structPtr);
		
		return obj;
	}

    public static uint Byte4ToUInt32(byte[] bytes,int idx = 0)
    {
        uint vl =0;
        if (bytes.Length > idx + 0) vl |= (uint)(bytes[idx] << 0);
        if (bytes.Length > idx + 1) vl |= (uint)(bytes[idx + 1] << 8);
        if (bytes.Length > idx + 2) vl |= (uint)(bytes[idx + 2] << 16);
        if (bytes.Length > idx + 3) vl |= (uint)(bytes[idx + 3] << 24);
        return vl;
    }
    public static uint Byte4ToUInt32(byte byte0,byte byte1,byte byte2,byte byte3)
    {
        uint vl = 0;
        vl |= (uint)(byte0 << 0);
        vl |= (uint)(byte1 << 8);
        vl |= (uint)(byte2 << 16);
        vl |= (uint)(byte3 << 24);
        return vl;
    }
    public static void UInt32ToByte4(uint vl, out byte byte0, out byte byte1, out byte byte2, out byte byte3)
    {
        byte0 = (byte)(vl >> 0);
        byte1 = (byte)(vl >> 8);
        byte2 = (byte)(vl >> 16);
        byte3 = (byte)(vl >> 24);
    }

    public static char[] ByteArrayToCharArray(byte[] buffer,int index,int count)
    {
        char[] charBuffer = new char[count];
        for (int i = 0; i < count; i++)
        {
            charBuffer[i] = System.Convert.ToChar(buffer[index]);
            index++;
        }
        return charBuffer;
    }

    public static byte[] CharArrayToByteArray(char[] charBuffer)
    {
        byte[] buffer = new byte[charBuffer.Length];
        for (int i = 0; i < charBuffer.Length; i++)
        {
            buffer[i] = System.Convert.ToByte(charBuffer[i]);
        }
        return buffer;
    }
    public static uint CharArrayToUInt32(string s)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
        return System.BitConverter.ToUInt32(bytes, 0);
    }
}
