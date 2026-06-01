using Framework.Core.Serializer;
using Game.Core.EntityMgr;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class GlobalAOIMgr : AOIMgr
    {
        private Dictionary<long, GM_PlayerEntity> players = null;

        public override void Init(IMapWrapper mapWrapper, int viewSize = 0)
        {
            base.Init(mapWrapper);
            this.players = new Dictionary<long, GM_PlayerEntity>();
        }

        public override void OnUpdate(float dt)
        {
            if (this.players.Count <= 0)
            {
                return;
            }
            foreach (var player in this.players.Values)
            {
                SkillAndBuffSystem.Update(this, player, dt);
                if (player.uStatus.status == (int)CharactorStatus.Run)
                {
                    this.mapWrapper.NavUpdate(this, player, dt);
                }
            }
        }        

        public override void EnterToAOI(GM_PlayerEntity player)
        {
            long playerId = player.uPlayer.playerInfo.id;

            this.players.Add(playerId, player);
            ResEnterAOI res = new ResEnterAOI();
            List<CharactorArrive> charactorArrives = new List<CharactorArrive>();
            CharactorArrive charactorArrive = this.GetCharactorArrive(player);
            charactorArrives.Add(charactorArrive);
            res.charactors = charactorArrives.ToArray();
            //通知其他玩家进入AOI
            this.BroadMessageToAOI(player, res, playerId);



            //将其他玩家的AOI信息发给自己
            res = new ResEnterAOI();
            charactorArrives = new List<CharactorArrive>();
            foreach (var entity in this.players.Values)
            {
                charactorArrive = this.GetCharactorArrive(entity);
                charactorArrives.Add(charactorArrive);
            }
            res.charactors = charactorArrives.ToArray();
            this.SendMsg(player, res);
        }

        public override void LeaveFromAOI(GM_PlayerEntity player)
        {
            long playerId = player.uPlayer.playerInfo.id;
            if (!this.players.ContainsKey(playerId))
            {
                return;
            }
            this.players.Remove(playerId);
            int worldId = player.uWorld.worldId;
            ResLeaveAOI res = new ResLeaveAOI();
            res.leavePlayers = new int[1];
            res.leavePlayers[0] = worldId;
            this.BroadMessageToAOI(player, res, playerId);
        }

        public override void BroadMessageToAOI(GM_PlayerEntity player, Message msg, long ignorePlayerId = -1, List<GM_PlayerEntity> toPlayers = null)
        {
            foreach (var playerId in this.players.Keys)
            {
                if (playerId == ignorePlayerId)
                {
                    continue;
                }
                this.SendMsg(playerId, msg);
            }
        }
    }
}
