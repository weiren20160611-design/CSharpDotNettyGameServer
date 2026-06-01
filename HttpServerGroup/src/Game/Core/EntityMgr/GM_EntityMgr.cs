using Framework.Core.Net;
using Game.Core.Cache;
using Game.Core.GM_Rank;
using Game.Datas.GMEntities;
using Game.LogicServer;
using System.Collections.Concurrent;
using System.Reflection;

namespace Game.Core.EntityMgr
{
    public class GM_EntityMgr
    {
        public static GM_EntityMgr Instance = new GM_EntityMgr();
        private ConcurrentDictionary<long, GM_PlayerEntity> playerEntities = null;

        public void Init()
        {
            playerEntities = new ConcurrentDictionary<long, GM_PlayerEntity>();
        }


        public int AddPlayerEntity(IdSession s)
        {
            GM_PlayerEntity playerEntity = null;
            if (playerEntities.ContainsKey(s.playerID))
            {
                playerEntity = playerEntities[s.playerID];
                playerEntity.uPlayer.session = s;
                playerEntity.uPlayer.playerInfo = PlayerIDCache.Instance.Get(s.playerID);
                playerEntity.uPlayer.accountInfo = AccountIDCache.Instance.Get(s.accountID);
            }
            else
            {
                playerEntity = new GM_PlayerEntity();
                playerEntities.TryAdd(s.playerID, playerEntity);
                playerEntity.uPlayer.session = s;
                playerEntity.uPlayer.playerInfo = PlayerIDCache.Instance.Get(s.playerID);
                playerEntity.uPlayer.accountInfo = AccountIDCache.Instance.Get(s.accountID);
                FieldInfo[] files = typeof(GM_PlayerEntity).GetFields();
                foreach (var file in files)
                {
                    if (file.FieldType == typeof(PlayerComponent))
                    {
                        continue;
                    }
                    MethodInfo method = file.FieldType.GetMethod("Init", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { playerEntity });
                    }
                    //GM_RankMgr.Instance.FlushRankData((int)RankType.WorldCoin, playerId, playerEntity.uPlayer.playerInfo.ucoin);
                }
            }
            int ret = playerEntity.uGameRoom.playerInRoomState == (int)PlayerInRoom.Started ? 1 : 0;

            return ret;
        }

        public void RemovePlayerEntity(long playerId)
        {
            GM_PlayerEntity playerEntity;
            playerEntities.TryRemove(playerId, out playerEntity);
            FieldInfo[] files = typeof(GM_PlayerEntity).GetFields();
            foreach (var file in files)
            {
                if (file.FieldType == typeof(PlayerComponent))
                {
                    continue;
                }
                MethodInfo method = file.FieldType.GetMethod("Exit", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    method.Invoke(null, new object[] { playerEntity });
                }
            }
        }

        public GM_PlayerEntity GetPlayerEntity(long playerId)
        {
            if (!playerEntities.ContainsKey(playerId))
            {
                return null;
            }
            playerEntities.TryGetValue(playerId, out GM_PlayerEntity entity);
            return entity;
        }
    }
}
