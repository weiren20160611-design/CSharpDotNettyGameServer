using Game.Datas.GMEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.GM_Backpack
{
    [GoodsRule(30000)]
    public class WeaponRules
    {
        [GoodsProcesser("SellOut", -1)]
        static bool DefaultGoodsSellOut(GM_PlayerEntity player, PackItem item, object param)
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
