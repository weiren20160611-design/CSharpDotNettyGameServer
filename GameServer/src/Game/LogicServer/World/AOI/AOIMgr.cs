using Framework.Core.Serializer;
using Game.Core.EntityMgr;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public abstract class AOIMgr
    {

        protected IMapWrapper mapWrapper = null;
        public virtual void Init(IMapWrapper mapWrapper, int viewSize = 0)
        {
            this.mapWrapper = mapWrapper;
        }

        public abstract void OnUpdate(float dt);

        public abstract void LateUpdate(float dt);

        public abstract void EnterToAOI(GM_PlayerEntity player);

        public abstract void LeaveFromAOI(GM_PlayerEntity player);

        public abstract void BroadMessageToAOI(BaseEntity player, Message msg, long ignorePlayerId = -1, List<GM_PlayerEntity> toPlayers = null);


        public void SendMsg(long playerId, Message msg)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uPlayer.session == null)
            {
                return;
            }
            MessagePusher.PushMessage(playerEntity.uPlayer.session, msg, playerId);
        }
        public void SendMsg(GM_PlayerEntity playerEntity, Message msg)
        {
            if (playerEntity == null || playerEntity.uPlayer.session == null)
            {
                return;
            }
            MessagePusher.PushMessage(playerEntity.uPlayer.session, msg, playerEntity.uPlayer.playerInfo.id);
        }


        protected CharactorArrive GetCharactorArrive(GM_PlayerEntity player)
        {
            CharactorArrive charactorArrive = new CharactorArrive();
            charactorArrive.worldId = player.uWorld.worldId;


            charactorArrive.charactorInfo = new CharactorInfo();
            charactorArrive.charactorInfo.unick = player.uPlayer.playerInfo.name;
            charactorArrive.charactorInfo.job = player.uPlayer.playerInfo.job;
            charactorArrive.charactorInfo.sex = player.uPlayer.playerInfo.usex;
            charactorArrive.charactorInfo.charactorId = -1;

            charactorArrive.transform = new CharactorTransform();
            charactorArrive.transform.pos = new float[3];
            charactorArrive.transform.pos[0] = player.uTransform.pos.x;
            charactorArrive.transform.pos[1] = player.uTransform.pos.y;
            charactorArrive.transform.pos[2] = player.uTransform.pos.z;
            charactorArrive.transform.eulerAngles = new float[3];
            charactorArrive.transform.eulerAngles[0] = player.uTransform.eulerAngles.x;
            charactorArrive.transform.eulerAngles[1] = player.uTransform.eulerAngles.y;
            charactorArrive.transform.eulerAngles[2] = player.uTransform.eulerAngles.z;
            charactorArrive.charactorStatus = player.uStatus.status;

            return charactorArrive;
        }


    }
}
