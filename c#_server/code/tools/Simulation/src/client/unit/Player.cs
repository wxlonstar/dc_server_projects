using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dc
{
    /// <summary>
    /// 玩家
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class Player : Unit
    {
        private string m_char_name;    // 名字
        private byte m_char_type;      // 角色类型(男 or 女 )
        private uint m_flags;          // 特殊标记
        private uint m_model_idx;      // 模型ID
        private byte m_job;            // 职业
        private ushort m_level;        // 角色等级
        private uint m_exp;            // 当前经验
        private uint m_energy;         // 能量
        private uint m_gold;           // 金币（点卷）
        private uint m_coin;           // 游戏币(铜币)
        private uint m_hp;             // 生命
        private uint m_hp_max;         // 生命上限
        private uint m_hurt;           // 伤害
        private uint m_range;          // 攻击范围
        private uint m_run_speed;      // vip等级
        private uint m_vip_grade;
        private uint m_vip_flags;      // vip flags

        public Player()
            :base()
        {
        }

        public override void Setup()
        {
            base.Setup();
        }
        public override void Destroy()
        {
            base.Destroy();
        }
        public override void Update()
        {
            base.Update();
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        public override void LoadData(UnitAOIInfo info)
        {
            base.LoadData(info);

            PlayerAOIInfo data = (PlayerAOIInfo)info;
            m_char_name = data.char_name;
            m_char_type = data.char_type;
            m_pos.x = data.pos_x;
            m_pos.y = data.pos_y;
            m_flags = data.flags;
            m_model_idx = data.model_idx;
            m_job = data.job;
            m_level = data.level;
            m_exp = data.exp;
            m_energy = data.energy;
            m_gold = data.gold;
            m_coin = data.coin;
            m_hp = data.hp;
            m_hp_max = data.hp_max;
            m_hurt = data.hurt;
            m_range = data.range;
            m_run_speed = data.run_speed;
            m_vip_grade = data.vip_grade;
            m_vip_flags = data.vip_flags;
        }

        public string char_name
        {
            get { return m_char_name; }
            set { m_char_name = value; }
        }
        public byte char_type
        {
            get { return m_char_type; }
            set { m_char_type = value; }
        }
        public uint flags
        {
            get { return m_flags; }
            set { m_flags = value; }
        }
        public uint model_idx
        {
            get { return m_model_idx; }
            set { model_idx = value; }
        }
        public byte job
        {
            get { return m_job; }
            set { m_job = value; }
        }
        public ushort level
        {
            get { return m_level; }
            set { m_level = value; }
        }
        public uint exp
        {
            get { return m_exp; }
            set { m_exp = value; }
        }
        public uint energy
        {
            get { return m_energy; }
            set { m_energy = value; }
        }
        public uint gold
        {
            get { return m_gold; }
            set { m_gold = value; }
        }
        public uint coin
        {
            get { return m_coin; }
            set { m_coin = value; }
        }
        public uint hp
        {
            get { return m_hp; }
            set { m_hp = value; }
        }
        public uint hp_max
        {
            get { return m_hp_max; }
            set { m_hp_max = value; }
        }
        public uint hurt
        {
            get { return m_hurt; }
            set { m_hurt = value; }
        }
        public uint range
        {
            get { return m_range; }
            set { m_range = value; }
        }
        public uint run_speed
        {
            get { return m_run_speed; }
            set { m_run_speed = value; }
        }
        public uint vip_grade
        {
            get { return m_vip_grade; }
            set { m_vip_grade = value; }
        }
        public uint vip_flags
        {
            get { return m_vip_flags; }
            set { m_vip_flags = value; }
        }
    }
}
