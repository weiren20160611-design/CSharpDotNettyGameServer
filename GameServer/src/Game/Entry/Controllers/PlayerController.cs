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
        public object DoReqPullingPlayerData(IdSession s, ReqPullingPlayerData req, long accountId)
        {
            return PlayerModule.Instance.HandlerReqPullingPlayerData(s, req, accountId);
        }

        [RequestMapping]
        public object DoReqSelectPlayer(IdSession s, ReqSelectPlayer req, long accountId)
        {
            return PlayerModule.Instance.HandlerReqSelectPlayer(s, req, accountId);
        }

        [RequestMapping]
        public object DoReqRecvLoginBonues(IdSession s, ReqRecvLoginBonues req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqRecvLoginBonues(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqPullingBonuesList(IdSession s, ReqPullingBonuesList req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqPullingBonuesList(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqRecvBonues(IdSession s, ReqRecvBonues req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqRecvBonues(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqPullingTaskList(IdSession s, ReqPullingTaskList req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqPullingTaskList(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqTestGetGoods(IdSession s, ReqTestGetGoods req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqTestGetGoods(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqPullingEmailMsgList(IdSession s, ReqPullingEmailMsgList req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqPullingEmailMsgList(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqUpdateEmailMsgStatus(IdSession s, ReqUpdateEmailMsgStatus req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqUpdateEmailMsgStatus(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqPullingRankList(IdSession s, ReqPullingRankList req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqPullingRankList(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqPullingBackpackData(IdSession s, ReqPullingBackpackData req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqPullingBackpackData(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqTestUpdateGoods(IdSession s, ReqTestUpdateGoods req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqReqTestUpdateGoods(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqExchangeProduct(IdSession s, ReqExchangeProduct req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqExchangeProduct(s, req, playerId);
        }

        [RequestMapping]
        public object DoReqEnterLogicServer(IdSession s, ReqEnterLogicServer req, long playerId)
        {
            return PlayerModule.Instance.HandlerReqEnterLogicServer(s, req, playerId);
        }
    }
}
