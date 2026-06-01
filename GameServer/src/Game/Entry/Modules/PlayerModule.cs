using Framework.Core.Net;
using Framework.Core.Serializer;
using Framework.Core.task;
using Framework.Core.Utils;
using Game.Core.Cache;
using Game.Core.EmailMessage;
using Game.Core.EntityMgr;
using Game.Core.GameBonues;
using Game.Core.GM_Backpack;
using Game.Core.GM_Rank;
using Game.Core.GM_Task;
using Game.Core.GM_Trading;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using Game.LogicServer;
using System.Collections.Generic;
using System.Configuration;
using System.Numerics;

namespace Game.Core.Entry.Modules
{
    public class PlayerModule
    {
        public static PlayerModule Instance = new PlayerModule();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private bool GatewayMode = false;
        public void Init()
        {
            PlayerIDCache.Instance.Init();
            PlayerAccountIDCache.Instance.Init();
            PlayerLoginbonuesCache.Instance.Init();
            this.GatewayMode = bool.Parse(ConfigurationManager.AppSettings["GatewayMode"]);
        }

        void CheckLoginBonues(long playerId, PlayerInfo playerInfo)
        {
            playerInfo.hasBonues = 0;
            Loginbonues data = PlayerLoginbonuesCache.Instance.GetOrCreate(playerId);
            if (data == null)
            {
                logger.Error($"每日登录奖励获取失败{playerId}");
                return;
            }
            if (data.status == 0)
            {
                playerInfo.hasBonues = 1;
                playerInfo.days = data.days;
                playerInfo.bonuesNum = data.bonues;
            }
            bool hasLoginbonues = data.bonues_time < UtilsHelper.TimestampToday();
            if (!hasLoginbonues)
            {
                return;
            }

            bool isSustain = (data.bonues_time < UtilsHelper.TimestampToday()) && (data.bonues_time >= UtilsHelper.TimestampYesterday());

            data.days = isSustain ? data.days + 1 : 1;
            data.bonues = 100;
            data.status = 0;
            data.bonues_time = UtilsHelper.Timestamp();
            playerInfo.hasBonues = 1;
            playerInfo.days = data.days;
            playerInfo.bonuesNum = data.bonues;

            PlayerLoginbonuesCache.Instance.UpdateDataToDB(data);
        }

        public ResPullingPlayerData HandlerReqPullingPlayerData(IdSession s, ReqPullingPlayerData req, long accountId)
        {
            ResPullingPlayerData res = new ResPullingPlayerData();
            if (accountId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }

            int job = req.job;

            Player player = PlayerAccountIDCache.Instance.TryGetPlayer(accountId, job);
            if (player == null)
            {
                res.status = (int)Respones.PlayerIsNotExist;
                return res;
            }

            if (player.status != 0)
            {
                res.status = (int)Respones.PlayerIsFreeze;
                return res;
            }


            res.status = (int)Respones.OK;
            res.pInfo = new PlayerInfo();
            res.pInfo.exp = (int)player.exp;
            res.pInfo.hp = (int)player.HP;
            res.pInfo.mp = (int)player.MP;
            res.pInfo.umoney = (int)player.umoney;
            res.pInfo.uname = player.name;
            res.pInfo.ucoin = (int)player.ucoin;
            res.pInfo.usex = (int)player.usex;
            s.playerId = player.id;
            s.accountIdAndJob = PlayerAccountIDCache.Instance.Key(accountId);
            this.CheckLoginBonues(player.id, res.pInfo);

            //创建玩家Entity
            int ret = GM_EntityMgr.Instance.AddPlayerEntity(s, player.id);
            res.isReConnectGame = ret;

            if (GatewayMode)
            {
                res.playerId = player.id;
            }
            return res;
        }

        public ResSelectPlayer HandlerReqSelectPlayer(IdSession s, ReqSelectPlayer req, long accountId)
        {
            ResSelectPlayer res = new ResSelectPlayer();

            if (req.job <= 0)
            {
                res.status = (int)Respones.InvalidParams;
                return res;
            }

            Player player = PlayerAccountIDCache.Instance.GetOrCreate(accountId, req);
            if (player == null)
            {
                res.status = (int)Respones.PlayerIsNotExist;
                return res;
            }

            if (player.status != 0)
            {
                res.status = (int)Respones.PlayerIsFreeze;
                return res;
            }

            res.status = (int)Respones.OK;
            res.pInfo = new PlayerInfo();
            res.pInfo.exp = (int)player.exp;
            res.pInfo.hp = (int)player.HP;
            res.pInfo.mp = (int)player.MP;
            res.pInfo.umoney = (int)player.umoney;
            res.pInfo.uname = player.name;
            res.pInfo.ucoin = (int)player.ucoin;
            res.pInfo.usex = (int)player.usex;
            s.playerId = player.id;
            s.accountIdAndJob = PlayerAccountIDCache.Instance.Key(accountId);
            this.CheckLoginBonues(player.id, res.pInfo);

            //创建玩家Entity
            //int ret = GM_EntityMgr.Instance.AddPlayerEntity(s, player.id);
            //res.isReConnectGame = ret;
            res.isReConnectGame = 0;
            if (GatewayMode)
            {
                res.playerId = player.id;
            }

            return res;
        }

        public ResRecvLoginBonues HandlerReqRecvLoginBonues(IdSession s, ReqRecvLoginBonues req, long playerId)
        {
            ResRecvLoginBonues res = new ResRecvLoginBonues();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            Loginbonues loginbonues = PlayerLoginbonuesCache.Instance.Get(playerId);
            if (loginbonues == null)
            {
                res.status = (int)Respones.SystemErr;
                return res;
            }
            if (loginbonues.days == 0 || loginbonues.status != 0)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            res.status = (int)Respones.OK;
            res.num = loginbonues.bonues;


            Player player = PlayerIDCache.Instance.Get(playerId);
            if (player != null)
            {
                player.ucoin += loginbonues.bonues;
            }
            PlayerIDCache.Instance.UpdateDataToDB(player);

            loginbonues.status = 1;
            loginbonues.bonues = 0;
            PlayerLoginbonuesCache.Instance.UpdateDataToDB(loginbonues);

            return res;
        }

        public ResPullingBonuesList HandlerReqPullingBonuesList(IdSession s, ReqPullingBonuesList req, long playerId)
        {
            ResPullingBonuesList res = new ResPullingBonuesList();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            Bonues[] datas = BonuesMgr.Instance.PullingBonuesData(playerId, req.typeId);
            if (datas != null && datas.Length > 0)
            {
                res.bonuesArray = new BonuesItem[datas.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    res.bonuesArray[i] = new BonuesItem();
                    res.bonuesArray[i].bonuesId = datas[i].id;
                    res.bonuesArray[i].bonuesDesic = datas[i].bonuesDesic;
                    res.bonuesArray[i].status = (int)datas[i].status;
                    res.bonuesArray[i].typeId = (int)datas[i].tid;
                }
            }
            res.status = (int)Respones.OK;
            return res;
        }

        public ResRecvBonues HandlerReqRecvBonues(IdSession s, ReqRecvBonues req, long playerId)
        {
            ResRecvBonues res = new ResRecvBonues();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            Bonues bonues = BonuesMgr.Instance.GetBonuesByID(req.bonuesId);
            if (bonues == null || bonues.uid != playerId)
            {
                res.status = (int)Respones.InvalidOpt;
            }

            BonuesMgr.Instance.RecvBonues(req.bonuesId, bonues);
            res.status = (int)Respones.OK;
            res.b1 = (int)bonues.bonues1;
            res.b2 = (int)bonues.bonues2;
            res.b3 = (int)bonues.bonues3;
            res.b4 = (int)bonues.bonues4;
            res.b5 = (int)bonues.bonues5;
            res.typeId = (int)bonues.tid;

            return res;
        }

        public ResPullingTaskList HandlerReqPullingTaskList(IdSession s, ReqPullingTaskList req, long playerId)
        {
            ResPullingTaskList res = new ResPullingTaskList();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }


            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            List<GM_Task.GM_Task> gM_Tasks = GM_TaskMgr.Instance.PullingTaksData(playerEntity, req.typeId);

            if (gM_Tasks != null && gM_Tasks.Count > 0)
            {
                res.taskArray = new TaskItem[gM_Tasks.Count];
                for (int i = 0; i < gM_Tasks.Count; i++)
                {
                    res.taskArray[i] = new TaskItem();
                    res.taskArray[i].status = (int)gM_Tasks[i].taskInstanceDb.status;
                    res.taskArray[i].typeId = (int)gM_Tasks[i].taskInstanceDb.tid;
                    res.taskArray[i].taskDesic = gM_Tasks[i].taskDesic;

                }
            }
            res.status = (int)Respones.OK;
            return res;
        }

        public ResTestGetGoods HandlerReqTestGetGoods(IdSession s, ReqTestGetGoods req, long playerId)
        {
            ResTestGetGoods res = new ResTestGetGoods();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            if (req.typeId == 1)
            {
                GM_Task.GM_Task collectTask = GM_TaskMgr.Instance.GetCurrentTask(playerEntity, 100000);
                if (collectTask != null)
                {
                    GM_TaskMgr.Instance.UpdateTaskProgress(playerEntity, collectTask, "damond", req.count, res);
                }
            }
            else if (req.typeId == 2)
            {
                GM_Task.GM_Task collectTask = GM_TaskMgr.Instance.GetCurrentTask(playerEntity, 100000);
                if (collectTask != null)
                {
                    GM_TaskMgr.Instance.UpdateTaskProgress(playerEntity, collectTask, "book", req.count, res);
                }
            }
            return res;

        }
        public ResPullingEmailMsgList HandlerReqPullingEmailMsgList(IdSession s, ReqPullingEmailMsgList req, long playerId)
        {
            ResPullingEmailMsgList res = new ResPullingEmailMsgList();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }

            Emailmessage[] msgArray = GM_EmailMgr.Instance.PullingMessageList(playerId, req.msgStatus);
            if (msgArray == null || msgArray.Length <= 0)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            res.msgItemArray = new EmailMsgItem[msgArray.Length];
            for (int i = 0; i < msgArray.Length; i++)
            {
                res.msgItemArray[i] = new EmailMsgItem();
                res.msgItemArray[i].emailMsgId = (int)msgArray[i].id;
                res.msgItemArray[i].status = (int)msgArray[i].status;
                res.msgItemArray[i].content = msgArray[i].msgBody;
                res.msgItemArray[i].sendTime = (int)msgArray[i].sendTime;
            }
            res.status = (int)Respones.OK;
            return res;
        }
        public ResUpdateEmailMsgStatus HandlerReqUpdateEmailMsgStatus(IdSession s, ReqUpdateEmailMsgStatus req, long playerId)
        {
            ResUpdateEmailMsgStatus res = new ResUpdateEmailMsgStatus();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            GM_EmailMgr.Instance.UpdateMessageStatus(req.msgId, req.msgStatus);
            res.status = (int)Respones.OK;
            return res;
        }
        public ResPullingRankList HandlerReqPullingRankList(IdSession s, ReqPullingRankList req, long playerId)
        {
            ResPullingRankList res = new ResPullingRankList();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            res.status = (int)Respones.OK;
            res.selfIndex = -1;
            RankData[] rankDatas = GM_RankMgr.Instance.GetRankData(req.typeId, req.num);
            if (rankDatas != null && rankDatas.Length > 0)
            {
                res.rankList = new RankItem[rankDatas.Length];
                for (int i = 0; i < rankDatas.Length; i++)
                {
                    res.rankList[i] = new RankItem();
                    Player player = PlayerIDCache.Instance.Get(rankDatas[i].uid);
                    if (player == null)
                    {
                        continue;
                    }
                    Account account = AccountIDCache.Instance.Get((long)player.accountId);
                    if (account == null)
                    {
                        continue;
                    }
                    res.rankList[i].unick = account.unick;
                    res.rankList[i].uface = account.uface;
                    res.rankList[i].ucoin = rankDatas[i].value;
                    if (s.playerId == rankDatas[i].uid)
                    {
                        res.selfIndex = i + 1;
                    }
                }
            }
            return res;
        }

        public ResPullingBackpackData HandlerReqPullingBackpackData(IdSession s, ReqPullingBackpackData req, long playerId)
        {
            ResPullingBackpackData res = new ResPullingBackpackData();
            // 验证accountId, playerId账号;
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }

            GM_PlayerEntity player = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (player == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }

            Dictionary<int, List<GoodsItem>> ret = GM_BackpackMgr.Instance.GetBackpackData(ref player.uPack);
            //Dictionary<int, GoodsItem[]> ret = GM_BackpackMgr.Instance.GetBackpackData(ref player.uPack);//
            res.packGoods = new DicGoodsItem[ret.Keys.Count];
            int index = 0;
            foreach (var key in ret.Keys)
            {
                DicGoodsItem dic = new DicGoodsItem();
                dic.mainTypeId = key;
                dic.Value = ret[key].ToArray();
                res.packGoods[index] = dic;
                index++;
            }

            res.status = (int)Respones.OK;
            return res;
        }
        public ResTestUpdateGoods HandlerReqReqTestUpdateGoods(IdSession s, ReqTestUpdateGoods req, long playerId)
        {
            ResTestUpdateGoods res = new ResTestUpdateGoods();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            GM_BackpackMgr.Instance.UpdateGoodsWithTid(playerEntity, req.typeId, req.count);
            res.status = (int)Respones.OK;
            return res;
        }

        public ResExchangeProduct HandlerReqExchangeProduct(IdSession s, ReqExchangeProduct req, long playerId)
        {
            ResExchangeProduct res = new ResExchangeProduct();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            int canExchage = GM_TradingMgr.Instance.CanExchangeProduct(playerEntity, req.typeId);
            if (canExchage != (int)Respones.OK)
            {
                res.status = canExchage;
                return res;
            }

            GM_TradingMgr.Instance.DoExchangeProduct(playerEntity, req.typeId);
            res.status = (int)Respones.OK;
            return res;
        }

        //请求进入玩法服务器
        public ResEnterLogicServer HandlerReqEnterLogicServer(IdSession s, ReqEnterLogicServer req, long playerId)
        {
            ResEnterLogicServer res = new ResEnterLogicServer();
            if (playerId <= 0)
            {
                res.status = (int)Respones.AccountIsNotLogin;
                return res;
            }

            if (s.logicServerId != -1 && GatewayMode == false)
            {
                res.status = (int)Respones.AlreadyInLogicServer;
                return res;
            }


            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                GM_EntityMgr.Instance.AddPlayerEntity(s, playerId);
                playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
                if (playerEntity == null)
                {
                    res.status = (int)Respones.InvalidOpt;
                    return res;
                }
            }
            bool isReConnect = false;
            if (playerEntity.uGameRoom.logicServerId != -1 && playerEntity.uGameRoom.playerInRoomState == (int)PlayerInRoom.Started)
            {
                req.instanceId = playerEntity.uGameRoom.logicServerId;
                isReConnect = true;
            }

            if (req.instanceId == -1)
            {
                req.instanceId = LogicServerFactory.Instance.FindLogicServerInstance(req.typeId, req.zoneId);
                if (req.instanceId == -1)
                {
                    res.status = (int)Respones.LogicServerIsBusy;
                    return res;
                }
            }

            res.status = ProcessEnterLogicServer(s, playerId, req.instanceId, req.zoneId, isReConnect);
            if (res.status == (int)Respones.OK)
            {
                MessagePusher.PushMessage(s, res, playerId, (short)req.instanceId);
                return null;
            }


            return res;
        }

        int ProcessEnterLogicServer(IdSession s, long playerId, int logicServerId, int zoneId, bool isReConnect)
        {
            BaseLogicServer server = LogicServerFactory.Instance.GetLogicServer(logicServerId);
            if (server == null)
            {
                return (int)Respones.LogicServerIsNotExist;
            }

            int status = 0;
            if (!isReConnect)
            {
                status = server.EnterLogicServer(playerId, zoneId, logicServerId);
            }
            else
            {
                status = server.ReConnectGameInLogicServer(playerId);
            }
            if (status == (int)Respones.OK)
            {
                s.logicServerId = logicServerId;
            }
            return status;
        }

    }
}
