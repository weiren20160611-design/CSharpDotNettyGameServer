using Game.Core.Db;
using Game.Datas.Messages;
using StackExchange.Redis;

namespace Game.Core.GM_Rank
{
    public class RankData
    {
        public long uid;
        public int value;
    }
    public class GM_RankMgr
    {
        public static GM_RankMgr Instance = new GM_RankMgr();
        public void Init()
        {

        }

        public void FlushRankData(int rankType, long playerId, int value)
        {
            RedisService.Instance.SortedSetAdd(rankType.ToString(), playerId.ToString(), value);
        }

        public RankData[] GetRankData(int rankType, int num)
        {
            SortedSetEntry[] rankData = RedisService.Instance.SortedSetRangeByRankWithScore(rankType.ToString(), num);
            if (rankData.Length > 0)
            {
                RankData[] rankItems = new RankData[rankData.Length];
                for (int i = 0; i < rankData.Length; i++)
                {
                    RankData rankItem = new RankData();
                    rankItem.uid = long.Parse(rankData[i].Element);
                    rankItem.value = (int)rankData[i].Score;
                    rankItems[i] = rankItem;
                }
                return rankItems;
            }
            return null;
        }
    }
}
