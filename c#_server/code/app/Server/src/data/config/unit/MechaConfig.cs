using System;
using System.Collections.Generic;

namespace dc
{
    public class StdMechaInfo
    {
        public int  ID;
        public int  Type;
        public string Name;
        public int NameIdx;
        public int DefaultOwn;          //默认拥有

        public int Mano;                //机动性
        public int Damage;              //杀伤力
        public int Range;               //射程
        public int Detection;           //探测范围
        public int Live;                //生存能力
        public int Assist;              //辅助能力

        public int MainWpn;             //主武器
        public int AuxWpn;              //副武器
        public int SkillId;             //技能id
        public int SPSkillId;           //SP技能Id
        public float GoldPrice;         //金币价格
        public float SilverPrice;       //银币价格
        public float DiamondPrice;      //钻石价格
        public int SourceType;          //来源类型
        public int SaleType;            //销售类型
        public int ItemColor;           //道具品质（1-白色;2-绿色;3-蓝色;4-紫色;5-橙色;）
        public int IconIdx;             //头像Icon索引
        public string PicPath;          //图片路径
        public string ModelPath;        //模型路径
        public int EnterAudio;          //入场音效
        public int MoveAudio;           //移动音效
        public int DieAudio;            //死亡音效
        public int ReviveAudio;         //复活音效
        public int DropAudio;           //掉落声音
        public string DropEffect;       //掉落特效
        public float DropCamera;        //掉落镜头震动
        public float GoldDiscount;      //金币折扣
        public float SilverDiscount;    //银币折扣
        public float DiamondDiscount;   //钻石折扣
        public float AttackSpeed;       //攻击速度
        public float MoveSpeed;         //移动速度
        public float MaxHp;             //最大生命值
        public float HpSpeed;           //生命恢复速度
        public float MaxSp;             //SP最大值
        public float SpSpeed;           //SP恢复速度
        public float DefVal;            //防御力
        public float Crit;              //穿甲率%
        public float DetRange;          //探测范围
        public float Hide;              //隐藏性
        public float EyeRange;          //物理视野
        public float eyeHide;	        //视野隐蔽性 
        public float critDamage;	    //暴击伤害率
        public int DescIdx;             //机甲描述id
        public string Desc;             //机甲描述
        public int SkinID;              //皮肤
    }

    public class MechaConfig : ConfigBase
    {
        static private MechaConfig m_Instance;

        private Dictionary<int, StdMechaInfo> m_infos = new Dictionary<int, StdMechaInfo>();

        static public MechaConfig Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new MechaConfig();
                return m_Instance;
            }
        }

        public override bool Load()
        {
            ReadCsvConfig("unit/StdMechaInfo.csv", OnLoad);
            return true;
        }

        public override void Unload()
        {
            m_infos.Clear();
        }

        public override string[] WatcherFiles()
        {
            return new string[1] { "unit/StdMechaInfo.csv" };
        }
        private void OnLoad(CSVDocument doc)
        {
            int totalCount = (int)doc.TableRows();
            for (int i = 0; i < totalCount; ++i)
            {
                StdMechaInfo info = new StdMechaInfo();
                info.ID = doc.GetValue(i, "idx").ToInt32();
                info.Type = doc.GetValue(i, "type").ToInt32();
                info.Name = doc.GetValue(i, "name").ToString();
                info.NameIdx = doc.GetValue(i, "nameIdx").ToInt32();
                info.DefaultOwn = doc.GetValue(i, "defaultOwn").ToInt32();
                info.Mano = doc.GetValue(i, "mano").ToInt32();
                info.Damage = doc.GetValue(i, "damage").ToInt32();
                info.Range = doc.GetValue(i, "range").ToInt32();
                info.Detection = doc.GetValue(i, "detection").ToInt32();
                info.Live = doc.GetValue(i, "live").ToInt32();
                info.Assist = doc.GetValue(i, "assist").ToInt32();
                info.MainWpn = doc.GetValue(i, "mainWpn").ToInt32();
                info.AuxWpn = doc.GetValue(i, "auxWpn").ToInt32();
                info.SkillId = doc.GetValue(i, "skill").ToInt32();
                info.SPSkillId = doc.GetValue(i, "skillSP").ToInt32();
                info.GoldPrice = doc.GetValue(i, "goldPrice").ToFloat();
                info.SilverPrice = doc.GetValue(i, "silverPrice").ToFloat();
                info.DiamondPrice = doc.GetValue(i, "diamondPrice").ToFloat();
                info.SourceType = doc.GetValue(i, "sourceType").ToInt32();
                info.SaleType = doc.GetValue(i, "saleType").ToInt32();
                info.ItemColor = doc.GetValue(i, "itemColor").ToInt32();
                info.IconIdx = doc.GetValue(i, "iconIdx").ToInt32();
                info.PicPath = doc.GetValue(i, "picPath").ToString();
                info.ModelPath = doc.GetValue(i, "Model").ToString();
                info.MoveAudio = doc.GetValue(i, "MoveAudio").ToInt32();
                info.DieAudio = doc.GetValue(i, "DieAudio").ToInt32();
                info.EnterAudio = doc.GetValue(i, "EnterAudio").ToInt32();
                info.DropAudio = doc.GetValue(i, "DropAudio").ToInt32();
                info.DropEffect = doc.GetValue(i, "DropEffect").ToString();
                info.DropCamera = doc.GetValue(i, "DropCamera").ToFloat();
                info.ReviveAudio = doc.GetValue(i, "ReviveAudio").ToInt32();
                info.GoldDiscount = doc.GetValue(i, "goldDiscount").ToFloat();
                info.SilverDiscount = doc.GetValue(i, "silverDiscount").ToFloat();
                info.DiamondDiscount = doc.GetValue(i, "diamondDiscount").ToFloat();
                info.AttackSpeed = doc.GetValue(i, "attackSpeed").ToFloat();
                info.MoveSpeed = doc.GetValue(i, "moveSpeed").ToFloat();
                info.MaxHp = doc.GetValue(i, "hpMax").ToFloat();
                info.HpSpeed = doc.GetValue(i, "hpSpeed").ToFloat();
                info.MaxSp = doc.GetValue(i, "spMax").ToFloat();
                info.SpSpeed = doc.GetValue(i, "spSpeed").ToFloat();
                info.DefVal = doc.GetValue(i, "defVal").ToFloat();
                info.Crit = doc.GetValue(i, "crit").ToFloat() * 10000.0f;
                info.DetRange = doc.GetValue(i, "detRange").ToFloat();
                info.Hide = doc.GetValue(i, "hide").ToFloat();
                info.EyeRange = doc.GetValue(i, "eyeRange").ToFloat();
                info.eyeHide = doc.GetValue(i, "eyeHide").ToFloat();
                info.critDamage = doc.GetValue(i, "critDmg").ToFloat();
                info.DescIdx = doc.GetValue(i, "descIdx").ToInt32();
                info.Desc = doc.GetValue(i, "desc").ToString();
                info.SkinID = doc.GetValue(i, "SkinID").ToInt32();

                if (info.ID > 0 && doc.GetValue(i, "isEnable").ToInt32() == 1)
                {
                    m_infos.Add(info.ID, info);
                }
            }
        }

        public int getNumberOfMecha()
        {
            return m_infos.Count;
        }

        public Dictionary<int, StdMechaInfo> getAllMechaData()
        {
            return m_infos;
        }

        public StdMechaInfo GetMechaInfo(int id)
        {
            StdMechaInfo info;
            if (m_infos.TryGetValue(id, out info))
            {
                return info;
            }
            else
            {
                Log.Warning(string.Format("Not find mechainfo id = {0}", id));
                return null;
            }
        }
    }
}
