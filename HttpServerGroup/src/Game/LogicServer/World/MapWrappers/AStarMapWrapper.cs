using Game.Datas.GMEntities;
using Game.Datas.Messages;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;


namespace Game.LogicServer
{
    public class SpawnPointElement
    {
        public float x;
        public float y;

        public int spawnId;
        public bool defaultSpawn;
    }
    public class AStarMapWrapper : IMapWrapper
    {
        private int mapId;
        private MapData mapData = null;
        private Dictionary<int, SpawnPointElement> spawnPoints = null;


        public void LoadMapData(int mapId)
        {
            this.mapId = mapId;
            string path = $"Configs/Jsons/MapDatas/{mapId}.json";
            string mapJsonStr = File.ReadAllText(path);
            this.mapData = JsonMapper.ToObject<MapData>(mapJsonStr);

            JsonData mapDataJson = JsonMapper.ToObject(mapJsonStr);
            this.InitMapSpawnPoint(mapDataJson["mapItems"]);

            PathFindingAgent.instance.init(this.mapData);
        }

        private void InitMapSpawnPoint(JsonData mapItem)
        {
            spawnPoints = new Dictionary<int, SpawnPointElement>();
            for (int i = 0; i < mapItem.Count; i++)
            {
                JsonData item = mapItem[i];
                InitSpawnInMap(item);
            }
        }

        private void InitSpawnInMap(JsonData item)
        {
            string itemType = (string)item["type"];
            if (itemType == "spawnPoint")
            {
                SpawnPointElement spawnPoint = new SpawnPointElement();
                spawnPoint.spawnId = (int)((item["spawnId"].IsInt) ? (int)item["spawnId"] : (double)(item["spawnId"]));
                spawnPoint.x = (float)((item["x"].IsInt) ? (int)item["x"] : (double)(item["x"]));
                spawnPoint.y = (float)((item["y"].IsInt) ? (int)item["y"] : (double)(item["y"]));
                spawnPoint.defaultSpawn = (bool)item["defaultSpawn"];
                if (spawnPoint != null)
                {
                    this.spawnPoints.Add(spawnPoint.spawnId, spawnPoint);
                }
            }
        }

        
        public void PlayerSpawnAtMap(GM_PlayerEntity player, int spawnIndex = -1)
        {
            Vector2 spawnPos = this.GetSpawnPointPos(spawnIndex);
            player.uTransform.pos = new Vector3(spawnPos.x, 0.0f, spawnPos.y);
            player.uTransform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }


        private Vector2 GetSpawnPointPos(int spawnId)
        {
            Vector2 pos;
            pos.x = pos.y = 0.0f;

            SpawnPointElement first = null;

            if (this.spawnPoints.ContainsKey(spawnId))
            {
                pos.x = this.spawnPoints[spawnId].x;
                pos.y = this.spawnPoints[spawnId].y;
            }
            else
            {
                // 找默认的，如果默认的也没有，我就找第一个;
                foreach (var key in this.spawnPoints.Keys)
                {
                    if (this.spawnPoints[key] != null && first == null)
                    {
                        first = this.spawnPoints[key];
                    }

                    if (this.spawnPoints[key].defaultSpawn)
                    {
                        first = this.spawnPoints[key];
                        break;
                    }
                }
            }

            if (first != null)
            {
                pos.x = first.x;
                pos.y = first.y;
            }

            return pos;
        }

        public void NavToDst(GM_PlayerEntity player, params float[] dst)
        {
            List<RoadNode> roadNodes = PathFindingAgent.instance.seekPath2(player.uTransform.pos.x, player.uTransform.pos.z, dst[0], dst[2]);
            if (roadNodes == null || roadNodes.Count < 2)
            {
                //player.uStatus.status = (int)CharactorStatus.Idle;
                return;
            }
            player.uStatus.status = (int)CharactorStatus.Run;

            AStarNavSystem.StartRoadNavAction(player, roadNodes);
            
        }

        public void NavUpdate(AOIMgr aoiMgr, GM_PlayerEntity player, float dt)
        {
            AStarNavSystem.NavRoadUpdate(player, aoiMgr, dt);

            //实时同步玩家位置，测试证明服务器在跑数据
            //ResSyncCharactorStatus res = EntityHelper.GetResSyncCharactorStatus(player);
            //aoiMgr.BroardMessageToAOI(player, res);
        }

        public int MapWidth()
        {
            return this.mapData.mapWidth;
        }

        public int MapHeight()
        {
            return this.mapData.mapHeight;
        }
    }
}
