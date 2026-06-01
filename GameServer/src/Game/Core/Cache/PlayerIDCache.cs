using Framework.Core.Cache;
using Game.Core.Db;
using Game.Datas.DBEntities;

namespace Game.Core.Cache
{
    public class PlayerIDCache : BaseCacheSerivce<long, Player>
    {
        public static PlayerIDCache Instance = new PlayerIDCache();
        public void Init()
        {

        }

        public override Player Load(long playerId)
        {
            return DBService.Instance.GetGameInstance().Queryable<Player>().First(it => it.id == playerId);
        }

        public void UpdateDataToDB(Player player)
        {
            DBService.Instance.GetGameInstance().Updateable(player).Where(it => it.id == player.id).ExecuteCommandAsync();
        }
    }
}
