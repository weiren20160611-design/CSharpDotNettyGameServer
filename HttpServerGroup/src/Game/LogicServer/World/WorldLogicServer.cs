using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.EntityMgr;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using Game.Utils;
using System.Collections.Generic;


namespace Game.LogicServer
{
    [LogicServerMeta((int)ServerType.Logic2)]
    public partial class WorldLogicServer : CommonLogicServer
    {
        GameWorld gameWorld = null;


        public override void OnStart()
        {
            base.OnStart();
            LogicServerInstance config = this.instanceConfig as LogicServerInstance;
            this.logger.Info($"逻辑服{this.logicServerId}:{config.desic}开启");
            this.gameWorld = new GameWorld();

            List<int> zones = GameUtils.ParseStringWithIntValue(config.zones);
            if (zones.Count > 1)
            {
                this.logger.Info("逻辑服实例只能创建一个地图");
            }
            this.gameWorld.Init(zones[0], this.timerMgr);
        }

        public void OnUpdate(float dt)
        {
            this.gameWorld.OnUpdate(dt);
        }

        /// <summary>
        /// 进入逻辑服
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="zoneId"></param>
        /// <param name="logicServerId"></param>
        /// <returns></returns>
        public override int EnterLogicServer(long playerId, int zoneId, int logicServerId)
        {
            if (!this.zoneId_ZoneConfig.ContainsKey(zoneId))
            {
                return (int)Respones.InvalidParams;
            }
            LogicServerInstance logicServerInstance = this.instanceConfig as LogicServerInstance;
            if (logicServerInstance != null)
            {
                if (this.onelinePlayers >= logicServerInstance.maxUser)
                {
                    return (int)Respones.LogicServerIsBusy;
                }
            }

            ZoneConfig zoneConfig = ExcelUtils.GetConfigData<ZoneConfig>(zoneId.ToString());
            if (zoneConfig == null)
            {
                return (int)Respones.InvalidParams;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                return (int)Respones.InvalidOpt;
            }

            int status = ZoneRule.PlayerCanEnterZone(zoneConfig.condString, playerEntity);
            if (status != (int)Respones.OK)
            {
                return status;
            }

            status = this.gameWorld.PlayerEnterToWorld(playerEntity, playerId);
            if (status != (int)Respones.OK)
            {
                return status;
            }
            playerEntity.uWorld.zoneId = zoneId;

            this.onelinePlayers++;

            return status;
        }

        public override int QuitLogicServer(long playerId, int reason)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null)
            {
                return (int)Respones.InvalidOpt;
            }

            if (reason == (int)QuitReason.DisconnectQuit)
            {
                playerEntity.uPlayer.session = null;
            }

            int status = this.gameWorld.PlayerExitFromWorld(playerEntity, playerId, reason);
            if (status != (int)Respones.OK)
            {
                return status;
            }
            playerEntity.uWorld.zoneId = -1;
            this.onelinePlayers--;
            return status;
        }

        public override int ReConnectGameInLogicServer(long playerId)
        {
            return 0;
        }
    }
}
