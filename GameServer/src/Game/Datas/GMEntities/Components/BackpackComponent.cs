using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public class PackItem
    {
        public Gamegoods goods = null;
        public object strengData = null; // 如果有强化数据 protobuf 解码以后的对象，可以参考任务系统;
    }

    public struct BackpackComponent
    {
        // 注意线程安全, 推到后期;
        public Dictionary<int, List<PackItem>> packItems; // 物品ID===》物品数据对象  20001===>[20001实例1(强化数据1), 20001实例2(强化数据2)];


        private static Gamegoods[] LoadDataFromDb(long playerId)
        {
            return DBService.Instance.GetGameInstance().Queryable<Gamegoods>().Where(it => it.uid == playerId).ToArray();
        }

        public int CanExchangeProduct(GM_PlayerEntity gM_PlayerEntity, int productId)
        {
            return (int)Respones.OK;
        }

        public void ExchangeProduct(GM_PlayerEntity gM_PlayerEntity, int productId)
        {

        }

        public static void Init(GM_PlayerEntity entity)
        {


            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            entity.uPack.packItems = new Dictionary<int, List<PackItem>>();

            Gamegoods[] gameGoods = LoadDataFromDb(entity.uPlayer.playerInfo.id);
            if (gameGoods == null || gameGoods.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < gameGoods.Length; i++)
            {
                PackItem item = new PackItem();
                item.goods = gameGoods[i];
                item.strengData = null; // 通过规则模块(StrenRules)来编码解码我们的强化数据，参考任务系统
                List<PackItem> goodsSet = null;
                // 如果有强化数据，你可能还要考虑强化数据是否要作为key;
                if (entity.uPack.packItems.ContainsKey((int)gameGoods[i].tid))
                {
                    goodsSet = entity.uPack.packItems[(int)gameGoods[i].tid];
                    goodsSet.Add(item);
                }
                else
                {
                    goodsSet = new List<PackItem>();
                    entity.uPack.packItems.Add((int)gameGoods[i].tid, goodsSet);
                    goodsSet.Add(item);
                }
            }
        }

        public static void Reset(GM_PlayerEntity entity)
        {
            
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }

        

    }
}