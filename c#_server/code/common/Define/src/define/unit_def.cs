using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    public class unit
    {

        public static UnitAOIInfo GetUnitInfo(eUnitType type)
        {
            switch (type)
            {
                case eUnitType.PLAYER: return new PlayerAOIInfo();
                case eUnitType.MONSTER: return new MonsterAOIInfo();
                case eUnitType.PET: return new PetAOIInfo();
                case eUnitType.NPC: return new NPCAOIInfo();
                case eUnitType.ROBOT: return new RobotAOIInfo();
                case eUnitType.ITEM: return new ItemAOIInfo();
                case eUnitType.OBJ: return new ObjAOIInfo();
                case eUnitType.SKILL: return new SkillAOIInfo();
                default: return null;
            }
        }
    }

    /// <summary>
    /// 唯一标记单位id
    /// </summary>
    public struct UnitID : ISerializeObject
    {
        public eUnitType type;
        public uint obj_type;
        public long obj_idx;
        public void Set(eUnitType _type, uint _obj_type, long _obj_idx)
        {
            this.type = _type;
            this.obj_type = _obj_type;
            this.obj_idx = _obj_idx;
        }
        public void Read(ByteArray by)
        {
            type = (eUnitType)by.ReadUShort();
            obj_type = by.ReadUInt();
            obj_idx = by.ReadLong();
        }
        public void Write(ByteArray by)
        {
            by.WriteUShort((ushort)type);
            by.WriteUInt(obj_type);
            by.WriteLong(obj_idx);
        }
    }

    /// <summary>
    /// 玩家id+名称
    /// </summary>
    public struct PlayerIDName : ISerializeObject
    {
        public long char_idx;
        public string char_name;
        public void Set(long _char_idx, string _char_name)
        {
            this.char_idx = _char_idx;
            this.char_name = _char_name;
        }
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
    }
    /// <summary>
    /// 根据名称查找的角色信息
    /// </summary>
    public struct PlayerInfoByName : ISerializeObject
    {
        public long char_idx;       // 角色id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort level;        // 角色等级

        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            level = by.ReadUShort();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(level);
        }
    }         
    /// <summary>
    /// ss发给客户端的数据
    /// </summary>
    public struct PlayerInfoForClient : ISerializeObject
    {
        public long char_idx;       // 角色id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint energy;         // 能量
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint hp;             // 生命
        public uint hp_max;         // 生命上限
        public uint hurt;           // 伤害
        public uint range;          // 攻击范围
        public uint run_speed;      // vip等级
        public uint vip_grade;
        public uint vip_flags;      // vip flags

        public void Copy(PlayerInfoForSS info)
        {
            char_idx = info.char_idx;
            char_name = info.char_name;
            char_type = info.char_type;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            energy = info.energy;
            gold = info.gold;
            coin = info.coin;
            hp = info.hp;
            hp_max = info.hp_max;
            hurt = info.hurt;
            range = info.range;
            run_speed = info.run_speed;
            vip_grade = info.vip_grade;
            vip_flags = info.vip_flags;
        }
        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            energy = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            hp = by.ReadUInt();
            hp_max = by.ReadUInt();
            hurt = by.ReadUInt();
            range = by.ReadUInt();
            run_speed = by.ReadUInt();
            vip_grade = by.ReadUInt();
            vip_flags = by.ReadUInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(energy);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(hp);
            by.WriteUInt(hp_max);
            by.WriteUInt(hurt);
            by.WriteUInt(range);
            by.WriteUInt(run_speed);
            by.WriteUInt(vip_grade);
            by.WriteUInt(vip_flags);
        }
    }
    /// <summary>
    /// db发给ss的数据
    /// </summary>
    public class PlayerInfoForSS : ISerializeObject
    {
        public long char_idx;       // 角色id
        public long account_index;
        public ushort spid;         // 渠道id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort ws_id;        // 所在大区
        public ushort ss_id;        // 所在逻辑服
        public ushort fs_id;        // 所在战斗服
        public int pos_x;           // 位置
        public int pos_y;           // 
        public uint scene_type_idx; // 所在场景
        public long scene_obj_idx;
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint energy;         // 能量
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint hp;             // 生命
        public uint hp_max;         // 生命上限
        public uint hurt;           // 伤害
        public uint range;          // 攻击范围
        public uint run_speed;      // vip等级
        public uint vip_grade;
        public uint vip_flags;      // vip flags

        public long time_last_login;
        public long time_last_logout;

        public void Copy(PlayerInfoForSS info)
        {
            char_idx = info.char_idx;
            account_index = info.account_index;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            ss_id = info.ss_id;
            fs_id = info.fs_id;
            pos_x = info.pos_x;
            pos_y = info.pos_y;
            scene_type_idx = info.scene_type_idx;
            scene_obj_idx = info.scene_obj_idx;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            energy = info.energy;
            gold = info.gold;
            coin = info.coin;
            hp = info.hp;
            hp_max = info.hp_max;
            hurt = info.hurt;
            range = info.range;
            run_speed = info.run_speed;
            vip_grade = info.vip_grade;
            vip_flags = info.vip_flags;
            time_last_login = info.time_last_login;
            time_last_logout = info.time_last_logout;
        }

        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            account_index = by.ReadLong();
            spid = by.ReadUShort();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            ws_id = by.ReadUShort();
            ss_id = by.ReadUShort();
            fs_id = by.ReadUShort();
            pos_x = by.ReadInt();
            pos_y = by.ReadInt();
            scene_type_idx = by.ReadUInt();
            scene_obj_idx = by.ReadLong();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            energy = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            hp = by.ReadUInt();
            hp_max = by.ReadUInt();
            hurt = by.ReadUInt();
            range = by.ReadUInt();
            run_speed = by.ReadUInt();
            vip_grade = by.ReadUInt();
            vip_flags = by.ReadUInt();
            time_last_login = by.ReadLong();
            time_last_logout = by.ReadLong();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteLong(account_index);
            by.WriteUShort(spid);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(ws_id);
            by.WriteUShort(ss_id);
            by.WriteUShort(fs_id);
            by.WriteInt(pos_x);
            by.WriteInt(pos_y);
            by.WriteUInt(scene_type_idx);
            by.WriteLong(scene_obj_idx);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(energy);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(hp);
            by.WriteUInt(hp_max);
            by.WriteUInt(hurt);
            by.WriteUInt(range);
            by.WriteUInt(run_speed);
            by.WriteUInt(vip_grade);
            by.WriteUInt(vip_flags);
            by.WriteLong(time_last_login);
            by.WriteLong(time_last_logout);
        }
    }
    /// <summary>
    /// ss发给ws的数据
    /// </summary>
    public class PlayerInfoForWS : ISerializeObject
    {
        public long account_idx;    // 账号
        public long char_idx;       // 角色id
        public ushort spid;         // 渠道id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort ws_id;        // 所在大区
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint vip_grade;      // vip等级

        public void Copy(PlayerInfoForWS info)
        {
            account_idx = info.account_idx;
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        /// <summary>
        /// 从PlayerInfoForSS复制数据
        /// </summary>
        public void Copy(PlayerInfoForSS info)
        {
            account_idx = info.account_index;
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        public void Read(ByteArray by)
        {
            account_idx = by.ReadLong();
            char_idx = by.ReadLong();
            spid = by.ReadUShort();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            ws_id = by.ReadUShort();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            vip_grade = by.ReadUInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(account_idx);
            by.WriteLong(char_idx);
            by.WriteUShort(spid);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(ws_id);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(vip_grade);
        }
    }
    /// <summary>
    /// ss发给gl的数据
    /// </summary>
    public class PlayerInfoForGL : ISerializeObject
    {
        public long char_idx;       // 角色id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort spid;         // 渠道id
        public ushort ws_id;         // 所在大区
        public uint scene_type_idx; //所在场景
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint vip_grade;      // vip等级

        public void Copy(PlayerInfoForGL info)
        {
            char_idx = info.char_idx;
            char_name = info.char_name;
            char_type = info.char_type;
            spid = info.spid;
            ws_id = info.ws_id;
            scene_type_idx = info.scene_type_idx;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        /// <summary>
        /// 从PlayerInfoForSS复制数据
        /// </summary>
        public void Copy(PlayerInfoForSS info)
        {
            char_idx = info.char_idx;
            char_name = info.char_name;
            char_type = info.char_type;
            spid = info.spid;
            ws_id = info.ws_id;
            scene_type_idx = info.scene_type_idx;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            spid = by.ReadUShort();
            ws_id = by.ReadUShort();
            scene_type_idx = by.ReadUInt();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            vip_grade = by.ReadUInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(spid);
            by.WriteUShort(ws_id);
            by.WriteUInt(scene_type_idx);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(vip_grade);
        }
    }
    /// <summary>
    /// ss发给fs的数据
    /// </summary>
    public class PlayerInfoForFS : ISerializeObject
    {
        public long char_idx;       // 角色id
        public ushort spid;         // 渠道id
        public string char_name;    // 名字
        public byte char_type;      // 角色类型(男 or 女 )
        public ushort ws_id;         // 所在大区
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint vip_grade;      // vip等级

        public void Copy(PlayerInfoForFS info)
        {
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        /// <summary>
        /// 从PlayerInfoForSS复制数据
        /// </summary>
        public void Copy(PlayerInfoForSS info)
        {
            char_idx = info.char_idx;
            spid = info.spid;
            char_name = info.char_name;
            char_type = info.char_type;
            ws_id = info.ws_id;
            flags = info.flags;
            model_idx = info.model_idx;
            job = info.job;
            level = info.level;
            exp = info.exp;
            gold = info.gold;
            coin = info.coin;
            vip_grade = info.vip_grade;
        }
        public void Read(ByteArray by)
        {
            char_idx = by.ReadLong();
            spid = by.ReadUShort();
            char_name = by.ReadString();
            char_type = by.ReadByte();
            ws_id = by.ReadUShort();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            vip_grade = by.ReadUInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteLong(char_idx);
            by.WriteUShort(spid);
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteUShort(ws_id);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(vip_grade);
        }
    }
    public interface UnitAOIInfo
    {
        void Read(ByteArray by);
        void Write(ByteArray by);
    }
    /// <summary>
    /// 玩家信息
    /// </summary>
    public struct PlayerAOIInfo : UnitAOIInfo
    {
        public string char_name;
        public byte char_type;      // 角色类型(男 or 女 
        public int pos_x;
        public int pos_y;
        public uint flags;          // 特殊标记
        public uint model_idx;      // 模型ID
        public byte job;            // 职业
        public ushort level;        // 角色等级
        public uint exp;            // 当前经验
        public uint energy;         // 能量
        public uint gold;           // 金币（点卷）
        public uint coin;           // 游戏币(铜币)
        public uint hp;             // 生命
        public uint hp_max;         // 生命上限
        public uint hurt;           // 伤害
        public uint range;          // 攻击范围
        public uint run_speed;      // vip等级
        public uint vip_grade;
        public uint vip_flags;      // vip flags

        public void Read(ByteArray by)
        {
            char_name = by.ReadString();
            char_type = by.ReadByte();
            pos_x = by.ReadInt();
            pos_y = by.ReadInt();
            flags = by.ReadUInt();
            model_idx = by.ReadUInt();
            job = by.ReadByte();
            level = by.ReadUShort();
            exp = by.ReadUInt();
            energy = by.ReadUInt();
            gold = by.ReadUInt();
            coin = by.ReadUInt();
            hp = by.ReadUInt();
            hp_max = by.ReadUInt();
            hurt = by.ReadUInt();
            range = by.ReadUInt();
            run_speed = by.ReadUInt();
            vip_grade = by.ReadUInt();
            vip_flags = by.ReadUInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteString(char_name);
            by.WriteByte(char_type);
            by.WriteInt(pos_x);
            by.WriteInt(pos_y);
            by.WriteUInt(flags);
            by.WriteUInt(model_idx);
            by.WriteByte(job);
            by.WriteUShort(level);
            by.WriteUInt(exp);
            by.WriteUInt(energy);
            by.WriteUInt(gold);
            by.WriteUInt(coin);
            by.WriteUInt(hp);
            by.WriteUInt(hp_max);
            by.WriteUInt(hurt);
            by.WriteUInt(range);
            by.WriteUInt(run_speed);
            by.WriteUInt(vip_grade);
            by.WriteUInt(vip_flags);
        }
    }
    public struct MonsterAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct PetAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct NPCAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct RobotAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct ItemAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct ObjAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    public struct SkillAOIInfo : UnitAOIInfo
    {
        public void Read(ByteArray by)
        {
        }
        public void Write(ByteArray by)
        {
        }
    }
    /// <summary>
    /// 单位类型
    /// </summary>
    public enum eUnitType
    {
        NONE,
        PLAYER,
        MONSTER,
        NPC,
        PET,
        ROBOT,
        ITEM,
        OBJ,
        SKILL,
    }
    /// <summary>
    /// 性别
    /// </summary>
    public enum eSexType
    {
        BOY = 0,
        GIRL,
    }
    /// <summary>
    /// unit属性修改类型的定义
    /// </summary>
    public enum eUnitModType
    {
        //////////////////////////////////////////////////////////////////////////
        //基础属性
        //////////////////////////////////////////////////////////////////////////
        UMT_none = 0,
        /// <summary>
        /// 角色类型
        /// </summary>
        UMT_char_type,
        /// <summary>
        /// 角色名称
        /// </summary>
        UMT_char_name,
        /// <summary>
        /// 所在战斗服id
        /// </summary>
        UMT_fs_uid,
        /// <summary>
        /// 角色标记
        /// </summary>
        UMT_flags,
        /// <summary>
        /// 所在场景
        /// </summary>
        UMT_scene_type,
        /// <summary>
        /// 模型ID
        /// </summary>
        UMT_model_idx,
        /// <summary>
        /// 职业
        /// </summary>
        UMT_job,
        /// <summary>
        /// 等级
        /// </summary>
        UMT_level,
        /// <summary>
        /// 经验
        /// </summary>
        UMT_exp,
        /// <summary>
        /// 能量 
        /// </summary>
        UMT_energy,
        /// <summary>
        /// 点券
        /// </summary>
        UMT_gold,
        /// <summary>
        /// 游戏币
        /// </summary>
        UMT_coin,
        /// <summary>
        /// 生命
        /// </summary>
        UMT_hp,
        /// <summary>
        /// vip等级
        /// </summary>
        UMT_vip_grade,
        /// <summary>
        /// vip flags
        /// </summary>
        UMT_vip_flags,
        /// <summary>
        /// 最后登录时间
        /// </summary>
        UMT_time_last_login,
        /// <summary>
        /// 最后登出时间
        /// </summary>
        UMT_time_last_logout,

        //////////////////////////////////////////////////////////////////////////
        //扩展属性
        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 基础伤害
        /// </summary>
        UMT_base_hurt = 100,
        /// <summary>
        /// 基础移动速度
        /// </summary>
        UMT_base_run_speed,
        /// <summary>
        /// 基础能量
        /// </summary>
        UMT_base_energy,
        /// <summary>
        /// 生命上限
        /// </summary>
        UMT_hp_max,
        /// <summary>
        /// 能量上限
        /// </summary>
        UMT_energy_max,
        /// <summary>
        /// 伤害
        /// </summary>
        UMT_hurt,	
        /// <summary>
        /// 范围
        /// </summary>
        UMT_range,
        /// <summary>
        /// 移动速度
        /// </summary>
        UMT_run_speed,				
    }
    /// <summary>
    /// 属性操作方式
    /// </summary>
    public enum eUnitAttrOperate
    {
        Add,
        Sub,
        Mul,
        Div,
    }
    /// <summary>
    /// 通知方式
    /// </summary>
    public enum eUnitAttrObserver
    {
        Single,             //只告诉操作者
        BoardcaseScreen,    //同屏广播：全屏广播，谨慎使用
        Boardcase,          //全服广播：非特殊不要使用
    }
    /// <summary>
    /// 属性改变的原因
    /// </summary>
    public enum eUnitAttrAction
    {
        Unknow,             //未知
    }
}
