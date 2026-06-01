using Framework.Core.task;
using Framework.Core.Utils;
using Game.Datas.Excels;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using Game.LogicServer.RVOMapConfig;
using LitJson;
using RVO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.LogicServer
{
    public class RVOMapWrapper : IMapWrapper
    {
        private List<Vector3[]> obsSet = null;
        private int mapId;
        private LevelConfig mapData = null;
        private Simulator simulator = null;
        public RVOMapWrapper(Simulator simulator)
        {
            this.simulator = simulator;
        }
        public void LoadMapData(int mapId)
        {
            this.obsSet = null;

            this.mapId = mapId;
            // 根据mapId,加载我们的地图数据，配置好RVO的障碍物
            string path = $"Configs/Jsons/MapDatas/{mapId}.json";
            string mapJsonStr = File.ReadAllText(path);
            this.mapData = JsonMapper.ToObject<LevelConfig>(mapJsonStr);

            RVOSystem.RVOAddObstacles(this.simulator, this.mapData.rvoObstacles);
            this.obsSet = this.mapData.rvoObstacles;
            // end
        }


        public void NavUpdate(AOIMgr aoi, BaseEntity e, float dt)
        {

            RVOSystem.RvoUpdate(this.simulator, aoi, e, dt);
            // 测试,客户端不跑，服务端直接同步过去，可视化查看服务端上的行走的真实情况
            //ResSyncCharactorStatus res = EntityHelper.GetResSyncCharactorStatus(e);
            //aoi.BroadMessageToAOI(e, res);
        }

        public void NavLateUpdate(AOIMgr aoiMgr, BaseEntity player, float dt)
        {
            RVOSystem.RvoLateUpdate(this.simulator, aoiMgr, player, dt);
        }

        public int MapHeight()
        {
            throw new NotImplementedException();
        }

        public int MapWidth()
        {
            throw new NotImplementedException();
        }

        public void PlayerSpawnAtMap(BaseEntity e, int spawnIndex = -1)
        {
            if (this.mapData.roles.Length <= 0)
            {
                return;
            }

            spawnIndex = (spawnIndex == -1) ? 0 : spawnIndex % this.mapData.roles.Length;
            RoleConfig role = this.mapData.roles[spawnIndex];

            CharactorConfig charactorConfig = ExcelUtils.GetConfigData<CharactorConfig>(role.typeId.ToString());

            e.uRVO.agentId = RVOSystem.CreateAgent(this.simulator, role.pos, charactorConfig.radius);

            e.uTransform.pos = role.pos;
            Vector3 eulerAngle = role.rot;
            eulerAngle.y = role.rot.y - 90;
            e.uTransform.eulerAngles = eulerAngle;
        }

        public void NavToDst(BaseEntity e, AOIMgr aoi, float x, float y, float z)
        {
            e.uStatus.status = (int)CharactorStatus.Run;
            RVOSystem.StartRvoAction(e, x, y, z);
        }

        public void NavToDir(BaseEntity player, AOIMgr aoi, int x, int y)
        {
            RVOSystem.StartRvoAction(this.simulator, aoi, player, x, y);
        }
    }
}
