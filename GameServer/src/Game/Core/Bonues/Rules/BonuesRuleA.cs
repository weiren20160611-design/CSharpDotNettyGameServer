using Framework.Core.Utils;
using Game.Datas.DBEntities;
using System;
using Game.Core.Cache;

namespace Game.Core.GameBonues
{
    [BonuesClass(100000)]
    public class BonuesRuleA
    {
        [BonuesApply( -1)]
        public static void DefaultBonuesApplyToPlayer(Bonues bonues)
        {
            Game.Datas.DBEntities.Player playerInfo = PlayerIDCache.Instance.Get((long)bonues.uid);
            playerInfo.ucoin += 170;

            PlayerIDCache.Instance.UpdateDataToDB(playerInfo);
        }

        [BonuesCalculate(-1)]
        public static Bonues DefautBonuesBindingPlayer(long playerId, int typeId, object param = null)
        {

            Bonues bonues = new Bonues();
            bonues.uid = playerId;
            bonues.tid = typeId;
            bonues.status = 0;
            bonues.time = (int)UtilsHelper.Timestamp();
            bonues.endTime = -1;
            bonues.bonues1 = (int)param;
            Game.Datas.Excels.BonuesRuleA configBonues = ExcelUtils.GetConfigData<Game.Datas.Excels.BonuesRuleA>(bonues.tid.ToString());
            bonues.bonuesDesic = String.Format(configBonues.desicFormat, bonues.bonues1);

            return bonues;
        }

        [BonuesCalculate(2)]
        public static Bonues MoneyBonuesBindingPlayer(long playerId, int typeId, object param = null)
        {
            Bonues bonues = new Bonues();
            bonues.uid = playerId;
            bonues.tid = typeId;
            bonues.status = 0;
            bonues.time = (int)UtilsHelper.Timestamp();
            bonues.endTime = -1;
            bonues.bonues2 = (int)param;
            Game.Datas.Excels.BonuesRuleA configBonues = ExcelUtils.GetConfigData<Game.Datas.Excels.BonuesRuleA>(bonues.tid.ToString());
            bonues.bonuesDesic = String.Format(configBonues.desicFormat, bonues.bonues2);

            return bonues;
        }


        [BonuesApply(2)]
        public static void MoneyBonuesApplyToPlayer(Bonues bonues)
        {
            Game.Datas.DBEntities.Player playerInfo = PlayerIDCache.Instance.Get((long)bonues.uid);
            playerInfo.umoney += (int)bonues.bonues2;

            PlayerIDCache.Instance.UpdateDataToDB(playerInfo);
        }

    }
}
