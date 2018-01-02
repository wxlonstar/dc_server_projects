using System;
using System.Collections.Generic;

namespace dc
{
    public class item
    {
        /// <summary>
        /// 物品是否合法
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsValidItem(ItemID item_id)
        {
            switch (item_id.type)
            {
                case eMainItemType.Currency:
                    eCurrencyType sub_type = (eCurrencyType)item_id.obj_type;
                    if (sub_type != eCurrencyType.Diamond && sub_type != eCurrencyType.Gold && sub_type != eCurrencyType.Silver)
                        return false;
                    
                    break;

                case eMainItemType.Item:
                    break;

                default: return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 道具id
    /// </summary>
    public struct ItemID : ISerializeObject
    {
        public eMainItemType type;  //主类型
        public uint obj_type;       //副类型(货币：货币类型，物品：物品id)
        public long obj_value;      //数量

        public void Set(eMainItemType _type, uint _obj_type, long _obj_value)
        {
            this.type = _type;
            this.obj_type = _obj_type;
            this.obj_value = _obj_value;
        }
        public void Read(ByteArray by)
        {
            type = (eMainItemType)by.ReadByte();
            obj_type = by.ReadUInt();
            obj_value = by.ReadLong();
        }
        public void Write(ByteArray by)
        {
            by.WriteByte((byte)type);
            by.WriteUInt(obj_type);
            by.WriteLong(obj_value);
        }
    }
    /// <summary>
    /// 道具主类型
    /// </summary>
    public enum eMainItemType
    {
        None,
        Currency,   // 货币
        Item        // 物品
    }
    /// <summary>
    /// 货币类型
    /// </summary>
    public enum eCurrencyType
    {
        Diamond,    //钻石，点券等需要充值获取
        Gold,       //金币
        Silver,     //银币
    }
}
