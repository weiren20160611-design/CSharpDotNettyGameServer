using Framework.Core.Cache;
using Framework.Core.Utils;
using Game.Core.Db;
using Game.Core.GM_Task;
using Game.Datas.DBEntities;
using System.Threading.Tasks;

namespace Game.Core.Cache
{
    public class PlayerLoginbonuesCache : BaseCacheSerivce<long, Loginbonues>
    {
        public static PlayerLoginbonuesCache Instance = new PlayerLoginbonuesCache();

        public void Init()
        {

        }
        public override Loginbonues Load(long playerID)
        {
            return DBService.Instance.GetGameInstance().Queryable<Loginbonues>().First(it => it.uid == playerID);
        }


        public  Loginbonues GetOrCreate(long playerId)
        {

            Loginbonues dbLoginbonues = Get(playerId);
            if (dbLoginbonues != null)
            {
                return dbLoginbonues;
            }

            dbLoginbonues = new Loginbonues();
            dbLoginbonues.uid = playerId;
            dbLoginbonues.bonues = 100;
            dbLoginbonues.status = 0;
            dbLoginbonues.bonues_time = UtilsHelper.Timestamp();
            dbLoginbonues.days = 1;
            dbLoginbonues.id = DBService.Instance.GetGameInstance().Insertable(dbLoginbonues).ExecuteReturnIdentity();
            Put(playerId, dbLoginbonues);


            return dbLoginbonues;
        }

        public void UpdateDataToDB(Loginbonues loginbonues)
        {
            DBService.Instance.GetGameInstance().Updateable(loginbonues).Where(it => it.uid == loginbonues.uid).ExecuteCommandAsync();
        }
    }
}
