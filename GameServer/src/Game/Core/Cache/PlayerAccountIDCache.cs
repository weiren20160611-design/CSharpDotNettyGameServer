using Framework.Core.Cache;
using Framework.Core.Utils;
using Game.Datas.Messages;
using Game.Core.Db;
using Game.Datas.DBEntities;

namespace Game.Core.Cache
{
    public class PlayerAccountIDCache : BaseCacheSerivce<long, Player>
    {
        public static PlayerAccountIDCache Instance = new PlayerAccountIDCache();
        public void Init()
        {

        }

        public override Player Load(long accountIdAndJob)
        {
            long accoutID = (accountIdAndJob >> 3);
            long job = accountIdAndJob & 0x07; // 
            return DBService.Instance.GetGameInstance().Queryable<Player>().First(it => it.accountId == accoutID);
        }

        public Player TryGetPlayer(long accountId, int job)
        {
            long accountIdAndJob = Key(accountId); // 一个账号只允许一个职业;
            Player dbPlayer = Get(accountIdAndJob);
            if (dbPlayer != null)
            {
                return dbPlayer;
            }
            return null;
        }

        public long Key(long accountId, int job = 0)
        {
            return (accountId << 3) | (long)job;
        }

        public Player GetOrCreate(long accountId, ReqSelectPlayer req)
        {
            long accountIdAndJob = Key(accountId);
            Player dbPlayer = Get(accountIdAndJob);
            if (dbPlayer != null)
            {
                return dbPlayer;
            }

            dbPlayer = new Player();
            dbPlayer.id = IdGenerator.GetNextId();
            dbPlayer.accountId = accountId;
            dbPlayer.job = (byte)req.job;
            dbPlayer.exp = 0;
            dbPlayer.name = req.name;
            dbPlayer.HP = 100;
            dbPlayer.MP = 0;
            dbPlayer.status = 0;
            dbPlayer.lastDailyReset = 0;
            dbPlayer.level = 1;
            dbPlayer.ucoin = 100;
            dbPlayer.umoney = 1000;
            dbPlayer.status = 0;
            dbPlayer.vipRightJson = "";
            dbPlayer.usex = req.usex;
            DBService.Instance.GetGameInstance().Insertable(dbPlayer).ExecuteCommandAsync();

            Put(accountIdAndJob, dbPlayer);

            PlayerIDCache.Instance.Put(dbPlayer.id, dbPlayer);
            return dbPlayer;
        }
    }
}
