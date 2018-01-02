using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 2d位置
    /// </summary>
    public struct Position2D : ISerializeObject
    {
        public int x;
        public int y;

        public Position2D(int _x, int _y)
        {
            x = _x; y = _y;
        }
        public void Set(int _x, int _y)
        {
            x = _x; y = _y;
        }
        public static bool operator ==(Position2D pos1, Position2D pos2)
        {
            return pos1.x == pos2.x && pos1.y == pos2.y;
        }
        public static bool operator !=(Position2D pos1, Position2D pos2)
        {
            return pos1.x != pos2.x || pos1.y != pos2.y;
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
            x = by.ReadInt();
            y = by.ReadInt();
        }
        public void Write(ByteArray by)
        {
            by.WriteInt(x);
            by.WriteInt(y);
        }
    }
    /// <summary>
    /// 方向
    /// </summary>
    public enum eDirection
    {
        NONE = 0,
        RIGHT,
        RIGHT_BOTTOM,
        BOTTOM,
        LEFT_BOTTOM,
        LEFT,
        LEFT_TOP,
        TOP,
        RIGHT_TOP,
        MID,
    }
}
