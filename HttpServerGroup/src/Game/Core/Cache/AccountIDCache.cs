using Framework.Core.Cache;
using Game.Datas.DBEntities;
using Game.Core.Db;

namespace Game.Core.Cache
{
    class AccountIDCache : BaseCacheSerivce<long, Account>
    {
        public static AccountIDCache Instance = new AccountIDCache();

        public void Init()
        {

        }

        public override Account Load(long accountID)
        {
            return DBService.Instance.GetAuthInstance().Queryable<Account>().First(it => it.uid == accountID);
        }

        public void UpdateAccountToDatabase(Account dbAccount)
        {
            DBService.Instance.GetAuthInstance().Updateable(dbAccount).Where(it => it.uid == dbAccount.uid).ExecuteCommandAsync();
        }
    }
}
