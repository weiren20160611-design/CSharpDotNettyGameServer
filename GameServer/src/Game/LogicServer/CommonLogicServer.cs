

using Framework.Core.Net;
using Framework.Core.Serializer;
using Framework.Core.task;
using Framework.Core.Utils;
using Game.Datas.Excels;
using Game.Datas.Messages;
using Game.LogicServer;
using Game.Utils;
using System.Collections.Generic;

namespace Game
{
    public abstract class CommonLogicServer : BaseLogicServer
    {
        //分区ID->分区配置文件
        protected Dictionary<int, ZoneConfig> zoneId_ZoneConfig = null;


        public virtual void OnStart()
        {
            zoneId_ZoneConfig = new Dictionary<int, ZoneConfig>();

            LogicServerInstance logicServerConfigInstance = this.instanceConfig as LogicServerInstance;
            if (logicServerConfigInstance == null)
            {
                return;
            }
            List<int> zoneIdList = GameUtils.ParseStringWithIntValue(logicServerConfigInstance.zones);
            for (int i = 0; i < zoneIdList.Count; i++)
            {
                int zoneId = zoneIdList[i];
                List<long> waitList = new List<long>();
                ZoneConfig zoneConfig = ExcelUtils.GetConfigData<ZoneConfig>(zoneId.ToString());
                zoneId_ZoneConfig.Add(zoneId, zoneConfig);
            }
        }


        public override bool HasZone(int zoneId)
        {
            if (zoneId_ZoneConfig == null || !zoneId_ZoneConfig.ContainsKey(zoneId))
            {
                return false;
            }

            return true;
        }


        [RequestMapping]
        public object DoReqTestEchoLogicServer(IdSession s, ReqTestLogicCmdEcho req, long playerId)
        {
            ResTestLogicCmdEcho res = new ResTestLogicCmdEcho();
            res.content = req.content;
            return res;
        }

        [RequestMapping]
        public object DoReqExitLogicServer(IdSession s, ReqQuitLogicServer req, long playerId)
        {
            int status = this.QuitLogicServer(playerId, req.quitReason);
            ResQuitLogicServer res = null;
            if (req.quitReason != (int)QuitReason.DisconnectQuit)
            {
                res = new ResQuitLogicServer();
            }

            if (req.quitReason != (int)QuitReason.DisconnectQuit)
            {
                res.status = status;
            }
            if (req.quitReason == (int)QuitReason.DisconnectQuit)
            {
                if (status == (int)Respones.UserIsPlaying)
                {

                }
                s.logicServerId = -1;
            }

            if (status != (int)Respones.OK)
            {
                //MessagePusher.PushMessage(s, res, playerId, (short)s.logicServerId);
                return res;
            }

            s.logicServerId = -1;
            //MessagePusher.PushMessage(s, res, playerId, (short)s.logicServerId);
            return res;
        }
    }
}
