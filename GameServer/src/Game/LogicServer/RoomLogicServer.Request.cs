using Framework.Core.Net;
using Framework.Core.Utils;
using Game.Core.EntityMgr;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;

namespace Game.LogicServer
{

    public partial class RoomLogicServer : CommonLogicServer
    {
        /// <summary>
        /// 玩家坐下
        /// </summary>
        /// <param name="s"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [RequestMapping]
        public object DoReqPlayerSitdown(IdSession s, ReqSitdown req, long playerId)
        {
            ResSitdown res = new ResSitdown();
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return (int)Respones.InvalidOpt;
            }
            res.status = this.PlayerSitdown(playerId, req.seatId, playerEntity.uGameRoom.roomId);
            res.seatId = playerEntity.uGameRoom.seatId;
            return res;
        }
        public int PlayerSitdown(long playerId, int seatId, int roomId)
        {
            RoomMgr room = GetRoomByRoomId(roomId);
            if (room == null)
            {
                return (int)Respones.InvalidOpt;
            }
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return (int)Respones.InvalidOpt;
            }
            return room.PlayerSitDown(playerEntity, playerId, seatId);

        }

        /// <summary>
        /// 玩家站起
        /// </summary>
        /// <param name="s"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [RequestMapping]
        public object DoReqPlayerStandup(IdSession s, ReqStandup req, long playerId)
        {
            ResStandup res = new ResStandup();
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            res.status = this.PlayerStandup(playerId, playerEntity.uGameRoom.roomId);
            return res;
        }
        public int PlayerStandup(long playerId, int roomId)
        {
            RoomMgr room = GetRoomByRoomId(roomId);

            if (room == null)
            {
                return (int)Respones.InvalidOpt;
            }

            return room.PlayerStandUp(playerId);

        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="s"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [RequestMapping]
        public object DoReqUserSendChatMessage(IdSession s, ReqSendChatMessage req, long playerId)
        {
            ResSendChatMessage res = new ResSendChatMessage();
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                return (int)Respones.InvalidOpt;
            }
            RoomMgr room = GetRoomByRoomId(playerEntity.uGameRoom.roomId);
            if (room == null)
            {
                return (int)Respones.InvalidOpt;
            }
            room.SendChatMessage(playerId, req);
            return null;
        }

        /// <summary>
        /// 玩家准备
        /// </summary>
        /// <param name="s"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [RequestMapping]
        public object DoReqPlayerReady(IdSession s, ReqPlayerReady req, long playerId)
        {
            ResPlayerReady res = new ResPlayerReady();
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            RoomMgr room = GetRoomByRoomId(playerEntity.uGameRoom.roomId);
            if (room == null)
            {
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            int status = room.PlayerReady(playerEntity, playerId);
            if (status != (int)Respones.OK)
            {

                res.status = status;
                res.seatId = -1;
                return res;
            }
            return null;
        }

        [RequestMapping]
        public object DoReqPlayerOperation(IdSession s, ReqPlayerOperation req, long playerId)
        {
            GM_PlayerEntity playerEntity = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (playerEntity == null || playerEntity.uGameRoom.roomId == -1)
            {
                ResPlayerOperation res = new ResPlayerOperation();
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            RoomMgr room = GetRoomByRoomId(playerEntity.uGameRoom.roomId);
            if (room == null)
            {
                ResPlayerOperation res = new ResPlayerOperation();
                res.status = (int)Respones.InvalidOpt;
                return res;
            }
            return room.PlayerOperation(playerEntity, playerId, req);
        }


        [RequestMapping]
        public object DoPlayerStartSkill(IdSession s, ReqStartSkill req, long playerId)
        {
            GM_PlayerEntity player = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (player == null || player.uGameRoom.roomId == -1)
            {
                return null;
            }

            RoomMgr room = this.GetRoomByRoomId(player.uGameRoom.roomId);
            if (room == null)
            {
                return null;
            }

            room.DoPlayerStartSkill(player, playerId, req);

            return null;
        }

        [RequestMapping]
        public object DoPlayerStartBuff(IdSession s, ReqStartBuff req, long playerId)
        {
            GM_PlayerEntity player = GM_EntityMgr.Instance.GetPlayerEntity(playerId);
            if (player == null || player.uGameRoom.roomId == -1)
            {
                return null;
            }

            RoomMgr room = this.GetRoomByRoomId(player.uGameRoom.roomId);
            if (room == null)
            {
                return null;
            }

            room.DoPlayerStartBuff(player, playerId, req);

            return null;
        }
    }
}
