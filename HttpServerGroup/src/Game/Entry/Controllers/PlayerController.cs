using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Datas.Messages;
using Game.Core.Entry.Modules;
using System;

namespace Game.Core.Entry.Controllers
{
    [Controller]
    public class PlayerController
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [RequestMapping]
        public object DoReqPullingPlayerData(IdSession s, ReqPullingPlayerData req)
        {
            return PlayerModule.Instance.HandlerReqPullingPlayerData(s, req);
        }

        [RequestMapping]
        public object DoReqSelectPlayer(IdSession s, ReqSelectPlayer req)
        {
            return PlayerModule.Instance.HandlerReqSelectPlayer(s, req);
        }

        [RequestMapping]
        public object DoReqRecvLoginBonues(IdSession s, ReqRecvLoginBonues req)
        {
            return PlayerModule.Instance.HandlerReqRecvLoginBonues(s, req);
        }

        [RequestMapping]
        public object DoReqPullingBonuesList(IdSession s, ReqPullingBonuesList req)
        {
            return PlayerModule.Instance.HandlerReqPullingBonuesList(s, req);
        }

        [RequestMapping]
        public object DoReqRecvBonues(IdSession s, ReqRecvBonues req)
        {
            return PlayerModule.Instance.HandlerReqRecvBonues(s, req);
        }

        [RequestMapping]
        public object DoReqPullingTaskList(IdSession s, ReqPullingTaskList req)
        {
            return PlayerModule.Instance.HandlerReqPullingTaskList(s, req);
        }

        [RequestMapping]
        public object DoReqTestGetGoods(IdSession s, ReqTestGetGoods req)
        {
            return PlayerModule.Instance.HandlerReqTestGetGoods(s, req);
        }

        [RequestMapping]
        public object DoReqPullingEmailMsgList(IdSession s, ReqPullingEmailMsgList req)
        {
            return PlayerModule.Instance.HandlerReqPullingEmailMsgList(s, req);
        }

        [RequestMapping]
        public object DoReqUpdateEmailMsgStatus(IdSession s, ReqUpdateEmailMsgStatus req)
        {
            return PlayerModule.Instance.HandlerReqUpdateEmailMsgStatus(s, req);
        }

        [RequestMapping]
        public object DoReqPullingRankList(IdSession s, ReqPullingRankList req)
        {
            return PlayerModule.Instance.HandlerReqPullingRankList(s, req);
        }

        [RequestMapping]
        public object DoReqPullingBackpackData(IdSession s, ReqPullingBackpackData req)
        {
            return PlayerModule.Instance.HandlerReqPullingBackpackData(s, req);
        }

        [RequestMapping]
        public object DoReqTestUpdateGoods(IdSession s, ReqTestUpdateGoods req)
        {
            return PlayerModule.Instance.HandlerReqReqTestUpdateGoods(s, req);
        }

        [RequestMapping]
        public object DoReqExchangeProduct(IdSession s, ReqExchangeProduct req)
        {
            return PlayerModule.Instance.HandlerReqExchangeProduct(s, req);
        }

        [RequestMapping]
        public object DoReqEnterLogicServer(IdSession s, ReqEnterLogicServer req)
        {
            return PlayerModule.Instance.HandlerReqEnterLogicServer(s, req);
        }
    }
}
