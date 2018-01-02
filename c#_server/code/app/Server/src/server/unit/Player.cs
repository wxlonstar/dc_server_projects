using System;
using System.Collections.Generic;

namespace dc
{
    /// <summary>
    /// 玩家
    /// @author hannibal
    /// @time 2016-8-14
    /// </summary>
    public class Player : Unit
    {
        private long m_account_idx = 0;
        private ushort m_spid = 0;
        private DBID m_db_id;                   //数据所在db
        private ClientUID m_client_uid;         //所在网关

        private bool m_is_dirty = false;        //数据是否有更改
        private long m_last_save_time = 0;      //最后保存时间

        public Player()
            :base()
        {
            m_unit_type = eUnitType.PLAYER;
            m_unit_attr = new PlayerAttribute(this);
        }
        /// <summary>
        /// 对象池初始化
        /// </summary>
        public override void Init()
        {
            base.Init();
            m_account_idx = 0;
            m_spid = 0;
            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        public override void OnEnter()
        {
            base.OnEnter();

            EventController.TriggerEvent(EventID.PLAYER_ENTER_GAME, m_obj_idx);
        }
        public override void OnLeave()
        {
            EventController.TriggerEvent(EventID.PLAYER_LEAVE_GAME, m_obj_idx);

            m_unit_attr.SetAttribInteger(eUnitModType.UMT_time_last_logout, Time.second_time, true, false, false, false, false);
            this.Save();

            base.OnLeave();
        }
        public override void EnterScene(uint scene_type_idx, long scene_obj_idx)
        {
            base.EnterScene(scene_type_idx, scene_obj_idx);
        }
        public override void LeaveScene()
        {
            base.LeaveScene();
        }  
        public override void Update()
        {
            base.Update();
        }

        #region 数据加载和保存
        /// <summary>
        /// 加载数据
        /// </summary>
        public override void LoadData(object info)
        {
            base.LoadData(info);

            PlayerInfoForSS data = (PlayerInfoForSS)info;
            m_account_idx = data.account_index;
            m_spid = data.spid;
            m_obj_type = 0;
            m_obj_idx = data.char_idx;
            m_pos.Set(data.pos_x, data.pos_y);
            m_scene_obj_idx = data.scene_obj_idx;
            m_scene_type_idx = data.scene_type_idx;

            m_db_id.game_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Game);
            m_db_id.center_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Center);
            m_db_id.log_id = ServerConfig.GetDBByAccountIdx(m_account_idx, eDBType.Log);

            m_unit_attr.player_info.Copy(data);
            m_unit_attr.SetAttribInteger(eUnitModType.UMT_time_last_login, Time.second_time, false, false, false, false, false);
        }      
        /// <summary>
        /// 保存到db
        /// </summary>
        public void Save()
        {
            SQLCharHandle.SaveCharacterInfo(m_db_id, m_unit_attr.player_info);

            m_is_dirty = false;
            m_last_save_time = Time.timeSinceStartup;
        }
        /// <summary>
        /// 是否需要存盘
        /// </summary>
        /// <returns></returns>
        public bool NeedSave()
        {
            if (m_is_dirty && Time.timeSinceStartup - m_last_save_time > 1000 * 60 * 5)//5分钟保存一次
                return true;
            return false;
        }
        public void SetDirty()
        {
            m_is_dirty = true;
        }
        #endregion

        #region 消耗/获取
        /// <summary>
        /// 消耗物品
        /// </summary>
        /// <param name="info"></param>
        public void Consume(ItemID item_id, eClientEventAction action = eClientEventAction.Unknow)
        {
            switch (item_id.type)
            {
                case eMainItemType.Item:
                    ConsumeItem(item_id.obj_type, item_id.obj_value);
                    break;
                case eMainItemType.Currency:
                    ConsumeCurrency((eCurrencyType)item_id.obj_type, item_id.obj_value);
                    break;
            }
        }
        /// <summary>
        /// 消耗物品
        /// </summary>
        public void ConsumeItem(uint type, long value)
        {

        }
        /// <summary>
        /// 消耗货币
        /// </summary>
        public void ConsumeCurrency(eCurrencyType type, long value)
        {

        }
        #endregion

        #region 处理其他模块事件
        /// <summary>
        /// 处理db事件
        /// </summary>
        /// <param name="info"></param>
        public bool HandleDBEvent(DBEventInfo info)
        {
            return true;
        }
        #endregion

        #region 位置
        /// <summary>
        /// 修改位置
        /// </summary>
        public override void ModifyPos(int x, int y)
        {
            m_unit_attr.player_info.pos_x = x;
            m_unit_attr.player_info.pos_y = y;
        }
        public override int pos_x
        {
            get { return m_unit_attr.player_info.pos_x; }
        }
        public override int pos_y
        {
            get { return m_unit_attr.player_info.pos_y; }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 拷贝数据到unitinfo
        /// </summary>
        /// <param name="info"></param>
        public override UnitAOIInfo GetUnitAOIInfo()
        {
            PlayerAOIInfo player_info = (PlayerAOIInfo)unit.GetUnitInfo(m_unit_type);
            player_info.char_name = char_name;
            player_info.char_type = char_type;
            player_info.pos_x = pos_x;
            player_info.pos_y = pos_y;
            player_info.flags = flags;
            player_info.model_idx = model_idx;
            player_info.job = job;
            player_info.level = level;
            player_info.exp = exp;
            player_info.energy = energy;
            player_info.gold = gold;
            player_info.coin = coin;
            player_info.hp = hp;
            player_info.hp_max = hp_max;
            player_info.hurt = hurt;
            player_info.range = range;
            player_info.run_speed = run_speed;
            player_info.vip_grade = vip_grade;
            player_info.vip_flags = vip_flags;
            return player_info;
        }

        public ClientUID client_uid
        {
            get { return m_client_uid; }
            set { m_client_uid = value; }
        }
        public long account_idx
        {
            get { return m_account_idx; }
        }
        public ushort spid
        {
            get { return m_spid; }
        }
        public DBID db_id
        {
            get { return m_db_id; }
        }
        public ushort fs_uid
        {
            get { return (ushort)m_unit_attr.GetAttribInteger(eUnitModType.UMT_fs_uid); }
            set { m_unit_attr.SetAttribInteger(eUnitModType.UMT_fs_uid, value); }
        }
        public long char_idx
        {
            get { return m_obj_idx; }
        }
        public string char_name
        {
            get { return m_unit_attr.GetAttribString(eUnitModType.UMT_char_name); }
        }
        public byte char_type
        {
            get { return (byte)m_unit_attr.GetAttribInteger(eUnitModType.UMT_char_type); }
        }
        public uint flags
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_flags); }
        }
        public uint model_idx
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_model_idx); }
        }
        public byte job
        {
            get { return (byte)m_unit_attr.GetAttribInteger(eUnitModType.UMT_job); }
        }
        public ushort level
        {
            get { return (ushort)m_unit_attr.GetAttribInteger(eUnitModType.UMT_level); }
        }
        public uint exp
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_exp); }
        }
        public uint energy
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_energy); }
        }
        public uint gold
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_gold); }
        }
        public uint coin
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_coin); }
        }
        public uint hp
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_hp); }
        }
        public uint hp_max
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_hp_max); }
        }
        public uint hurt
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_hurt); }
        }
        public uint range
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_range); }
        }
        public uint run_speed
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_run_speed); }
        }
        public uint vip_grade
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_vip_grade); }
        }
        public uint vip_flags
        {
            get { return (uint)m_unit_attr.GetAttribInteger(eUnitModType.UMT_vip_flags); }
        }
        #endregion
    }
}
