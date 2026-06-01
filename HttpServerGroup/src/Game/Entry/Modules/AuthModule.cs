using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.Cache;
using Game.Datas.DBEntities;
using Game.Datas.Messages;
using Game.Core.GameBonues;

namespace Game.Core.Entry.Modules
{
    public class AuthModule
    {
        public static AuthModule Instance = new AuthModule();

        public void Init()
        {
            AccountGuestCache.Instance.Init();
            AccountIDCache.Instance.Init();
            AccountUnameCache.Instance.Init();
        }

        void CopyDbAccountToResponseData(Account dbAccount, AccountInfo aInfo)
        {
            aInfo.uface = dbAccount.uface;
            aInfo.unick = dbAccount.unick;
            aInfo.uvip = dbAccount.uvip;
            aInfo.isGuest = dbAccount.is_guest;
        }

        public ResGuestLogin HandlerReqGuestLogin(IdSession s, ReqGuestLogin req)
        {
            ResGuestLogin res = new ResGuestLogin();
            res.uinfo = null;

            // 检查参数
            if (req == null ||
                req.guestKey == null ||
                req.guestKey.Equals("") ||
                req.channal <= (int)Channal.InvalidChannal)
            {
                res.status = (int)Respones.InvalidParams;
                res.uinfo = null;

                return res;
            }
            // end


            Account dbAccount = AccountGuestCache.Instance.GetOrCreate(req.guestKey, req.channal);
            if (dbAccount.is_guest != 1)
            { // 这个账号是已经升级过的正式账号，不使用游客登录了;
                res.status = (int)Respones.UserIsNotGuest;
                return res;
            }

            if (dbAccount.status != 0) // 账号被冻结了
            {
                res.status = (int)Respones.UserIsFreeze;
                return res;
            }

            res.status = (int)Respones.OK;
            res.uinfo = new AccountInfo();
            CopyDbAccountToResponseData(dbAccount, res.uinfo);

            // 将我们的uid与session关联起来，这样当我们的session收到数据的时候，知道是哪个UID;
            s.accountID = dbAccount.uid;
            // end

            return res;
        }

        public ResGuestUpgrade HandlerReqGuestUpgrade(IdSession s, ReqGuestUpgrade req)
        {
            ResGuestUpgrade res = new ResGuestUpgrade();
            if (s.accountID <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            if (req == null ||
                req.uname == null ||
                req.uname.Equals("") ||
                req.upwd == null ||
                req.upwd.Equals("") ||
                req.unick == null ||
                req.unick.Equals(""))
            {
                res.status = (int)Respones.InvalidParams;

                return res;
            }

            Account dbAccount = AccountUnameCache.Instance.Get(req.uname);
            if (dbAccount != null)
            {
                res.status = (int)Respones.UnameIsExist;
                return res;
            }

            dbAccount = AccountIDCache.Instance.Get(s.accountID);
            if (dbAccount.is_guest == 0)
            {
                res.status = (int)Respones.UserIsNotGuest;
                return res;
            }
            AccountGuestCache.Instance.Remove(dbAccount.guest_key);
            dbAccount.is_guest = 0;
            dbAccount.guest_key = "";
            dbAccount.uname = req.uname;
            dbAccount.upwd = UtilsHelper.Md5(req.upwd);
            dbAccount.unick = req.unick;

            AccountIDCache.Instance.UpdateAccountToDatabase(dbAccount);
            AccountUnameCache.Instance.Put(dbAccount.uname, dbAccount);

            res.status = (int)Respones.OK;
            res.accountInfo = new AccountInfo();
            CopyDbAccountToResponseData(dbAccount, res.accountInfo);

            BonuesMgr.Instance.GenBonuesToTarget(s.playerID, 100001, 170);

            return res;
        }

        public ResRegisterUser HandlerReqRegisterUser(IdSession s, ReqRegisterUser req)
        {
            ResRegisterUser res = new ResRegisterUser();
            res.errorMsg = null;

            // 检查参数
            if (req == null ||
                req.uname == null ||
                req.uname.Equals("") ||
                req.upwd == null ||
                req.upwd.Equals("") ||
                req.channal <= (int)Channal.InvalidChannal)
            {
                res.status = (int)Respones.InvalidParams;

                return res;
            }
            // end


            Account dbAccount = AccountUnameCache.Instance.Get(req.uname);
            if (dbAccount != null)
            {
                res.status = (int)Respones.UnameIsExist;
                return res;
            }

            string md5Password = UtilsHelper.Md5(req.upwd);
            dbAccount = AccountUnameCache.Instance.GetOrCreate(req);
            if (dbAccount == null)
            {
                res.status = (int)Respones.SystemErr;
                return res;
            }

            res.status = (int)Respones.OK;

            return res;
        }

        public ResUserLogin HandlerReqUserLogin(IdSession s, ReqUserLogin req)
        {
            ResUserLogin res = new ResUserLogin();
            // 检查参数
            if (req == null ||
                req.uname == null ||
                req.uname.Equals("") ||
                req.upwd == null ||
                req.upwd.Equals(""))
            {
                res.status = (int)Respones.InvalidParams;

                return res;
            }
            // end

            Account dbAccount = AccountUnameCache.Instance.Get(req.uname);
            if (dbAccount == null)
            {
                res.status = (int)Respones.UserIsNotExist;
                return res;
            }


            if (!UtilsHelper.Md5(req.upwd).Equals(dbAccount.upwd))
            {
                res.status = (int)Respones.UserPasswordError;
                return res;
            }

            res.status = (int)Respones.OK;
            res.accountInfo = new AccountInfo();
            CopyDbAccountToResponseData(dbAccount, res.accountInfo);
            // 将我们的uid与session关联起来，这样当我们的session收到数据的时候，知道是哪个UID;
            s.accountID = dbAccount.uid;
            // end
            return res;
        }

    }

}

