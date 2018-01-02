using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class character
    {
        /// <summary>
        /// 角色名最大长度，单位字节
        /// </summary>
        public static int MAX_CHAR_NAME_LENGTH = 16;
    }
    /// <summary>
    /// id和名称二选一
    /// </summary>
    public struct CharacterIdxOrName : ISerializeObject
    {
        // 名称类型
        public enum eIdxNameType
        {
            CHARID,		// 按id
            CHARNAME,	// 按名字
        }
        public eIdxNameType type;
        public long char_idx;       //按id
        public string char_name;    //按名字

        public void Read(ByteArray by)
        {
            type = (eIdxNameType)by.ReadByte();
            if (type == eIdxNameType.CHARID)
                char_idx = by.ReadLong();
            else
                char_name = by.ReadString();
        }
        public void Write(ByteArray by)
        {
            by.WriteByte((byte)type);
            if (type == eIdxNameType.CHARID)
                by.WriteLong(char_idx);
            else
                by.WriteString(char_name);
        }
        public void SetIdx(long id)
        {
            type = eIdxNameType.CHARID;
            char_idx = id;
        }
        public void SetName(string name)
        {
            type = eIdxNameType.CHARNAME;
            char_name = name;
        }
        public bool IsIdxValid()
        {
            if (type == eIdxNameType.CHARID) return true;
            return false;
        }
    }
    /// <summary>
    /// id和名称集合
    /// </summary>
    public struct CharacterIdxName : ISerializeObject
    {
        public long char_idx;       //id
        public string char_name;    //名字

        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
        }
        public void Set(long id, string name)
        {
            char_idx = id;
            char_name = name;
        }
    }
    /// <summary>
    /// 角色基本数据,用于显示
    /// </summary>
    public struct CharacterLogin : ISerializeObject
    {
        public long char_idx;       // 角色id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort level;        // 等级
        public ushort wid;          // 世界服id
        public ushort sid;          // 逻辑服id
        public ushort dbid;         // db服id

        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            level = by.ReadUShort();
            wid = by.ReadUShort();
            sid = by.ReadUShort();
            dbid = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(level);
            by.WriteUShort(wid);
            by.WriteUShort(sid);
            by.WriteUShort(dbid);
        }
    }
    /// <summary>
    /// 创建角色
    /// </summary>
    public struct CreateCharacterInfo : ISerializeObject
    {
        public long char_idx;       // 角色id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort spid;         // 渠道id
        public ushort ws_id;
        public ushort ss_id;
        public ushort fs_id;

        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            spid = by.ReadUShort();
            ws_id = by.ReadUShort();
            ss_id = by.ReadUShort();
            fs_id = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(spid);
            by.WriteUShort(ws_id);
            by.WriteUShort(ss_id);
            by.WriteUShort(fs_id);
        }
    }
    /// <summary>
    /// 创角结果
    /// </summary>
    public enum eCreateCharResult
    {
        E_SUCCESS = 0,
        E_FAILED_COMMONERROR,
        E_FAILED_INTERNALERROR,             // 内部错误:可能是char_idx键重复
        E_FAILED_INVALIDPARAM_REPEATEDNAME, // 角色名重复
        E_FAILED_INVALIDPARAM_OTHER,        // 非法参数
        E_FAILED_CHARCOUNTLIMIT,			// 创建人数已经达到上限
    }
    /// <summary>
    /// 查询角色列表结果
    /// </summary>
    public enum eEnumCharResult
    {
        E_SUCCESS = 0,
        E_FAILED_UNKNOWNERROR,
        E_FAILED_INTERNALERROR,
    }
}
