using Framework.Core.Utils;
using Game.Datas.Excels;

namespace Game.Core.Config
{
    public class ConfigMgr
    {
        public static ConfigMgr Instance = new ConfigMgr();
        public void Init()
        { 
            ExcelUtils.ReadConfigData<BonuesRuleA>();
            ExcelUtils.ReadConfigData<CollectTask>();
            ExcelUtils.ReadConfigData<ExchangeTrading>();
            ExcelUtils.ReadConfigData<OrderTrading>();
            ExcelUtils.ReadConfigData<LogicServerInstance>();
            ExcelUtils.ReadConfigData<ZoneConfig>();
            ExcelUtils.ReadConfigData<CharactorConfig>();

            ExcelUtils.ReadConfigData<MapConfig>();

            ExcelUtils.ReadConfigData<MedicineGoods>();
            ExcelUtils.ReadConfigData<WeaponGoods>();
            ExcelUtils.ReadConfigData<WearGoods>();


            ExcelUtils.ReadConfigData<BuffAConfig>();
            ExcelUtils.ReadConfigData<SkillAConfig>();
            ExcelUtils.ReadConfigData<SkillBConfig>();

        }
    }
}
