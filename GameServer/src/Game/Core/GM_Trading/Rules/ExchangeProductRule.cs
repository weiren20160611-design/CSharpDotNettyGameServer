using Framework.Core.Utils;
using Game.Core.Cache;
using Game.Core.GM_Backpack;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.GM_Trading
{
    [TradingRule(1000000)]
    public class ExchangeProductRule
    {
        [TradingProcesser("CanExchange", -1)]
        public static int DefaultCanExchangeProduct(GM_PlayerEntity player, int productId)
        {
            ExchangeTrading exchangeTrading = ExcelUtils.GetConfigData<ExchangeTrading>(productId.ToString());
            if (exchangeTrading == null)
            {
                return (int)Respones.InvalidParams;
            }
            if (player.uPlayer.playerInfo.umoney < exchangeTrading.Price)
            {
                return (int)Respones.NotEnoughMoney;
            }

            return (int)Respones.OK;
        }

        [TradingProcesser("DoExchange", -1)]
        public static bool DefaultDoExchangeProduct(GM_PlayerEntity player, int productId)
        {
            ExchangeTrading config = ExcelUtils.GetConfigData<ExchangeTrading>(productId.ToString());
            if (config == null)
            {
                return false;
            }
            Dictionary<string, int> products = GameUtils.ParseStringWithKeyAndValue(config.Product);
            if (products.Count <= 0)
            {
                return false;
            }
            player.uPlayer.playerInfo.umoney -= config.Price;
            PlayerIDCache.Instance.UpdateDataToDB(player.uPlayer.playerInfo);
            foreach (var key in products.Keys)
            {
                int typeId = int.Parse(key);
                int value = products[key];
                GM_BackpackMgr.Instance.UpdateGoodsWithTid(player, typeId, value);
            }


            return true;
        }

    }
}
