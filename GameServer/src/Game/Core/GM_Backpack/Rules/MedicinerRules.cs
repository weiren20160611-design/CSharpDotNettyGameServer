using Game.Datas.GMEntities;


namespace Game.Core.GM_Backpack
{
    [GoodsRule(10000)]
    public class MedicinerRules
    {
        [GoodsProcesser("SellOut", -1)]
        static bool DefaultGoodsSellOut(GM_PlayerEntity player, PackItem item, object param)
        {
            return false;
        }

        [GoodsProcesser("SellOut", 4)] // 10004
        static bool BigBlueGoodsSellOut(GM_PlayerEntity player, PackItem item, object param)
        {
            return false;
        }

        [GoodsProcesser("BuyIn", -1)]
        static bool DefaultGoodsBuyIn(GM_PlayerEntity player, PackItem item, object param)
        {
            return false;
        }

        [GoodsProcesser("PutOn", -1)]
        static bool DefaultGoodsPutOn(GM_PlayerEntity player, PackItem item, object param)
        {
            return false;
        }
    }
}
