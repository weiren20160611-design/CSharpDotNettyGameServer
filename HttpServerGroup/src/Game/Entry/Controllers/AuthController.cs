using Framework.Core.Net;
using Framework.Core.Serializer;
using Framework.Core.Utils;
using Game.Datas.Messages;
using Game.Core.Entry.Modules;

namespace Game.Core.Entry.Controllers {

    [Controller]
    public class AuthController
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [RequestMapping]
        public object DoReqGuestLogin(IdSession s, ReqGuestLogin req) {
            return AuthModule.Instance.HandlerReqGuestLogin(s, req);
        }

        [RequestMapping]
        public object DoReqRegisterUser(IdSession s, ReqRegisterUser req)
        {
            return AuthModule.Instance.HandlerReqRegisterUser(s, req);
        }

        [RequestMapping]
        public object DoReqUserLogin(IdSession s, ReqUserLogin req)
        {
            return AuthModule.Instance.HandlerReqUserLogin(s, req);
        }

        [RequestMapping]
        public object DoReqGuestUpgrade(IdSession s, ReqGuestUpgrade req)
        {
            return AuthModule.Instance.HandlerReqGuestUpgrade(s, req);
        }
    }

}


