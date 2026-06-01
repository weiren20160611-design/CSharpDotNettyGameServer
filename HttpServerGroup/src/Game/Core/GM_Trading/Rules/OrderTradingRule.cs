using Game.Datas.GMEntities;


namespace Game.Core.GM_Trading
{
    [TradingRule(2000000)]
    public class OrderTradingRule
    {
        [TradingProcesser("GenerateOrder", -1)]
        public static long DefaultGenerateOrder(GM_PlayerEntity player, int productId, int payChannel)
        {
            return 0;
        }

        [TradingProcesser("OrderDelivery", -1)]
        public static bool DefaultDoOrderDelivery(GM_PlayerEntity player, int productId, long orderId)
        {
            return false;
        }

    }
}

