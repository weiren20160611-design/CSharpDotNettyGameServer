using Framework.Core.Serializer;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System.Collections.Generic;

namespace Game.LogicServer
{
    class AOIBlock
    { // 每个格子，就是一个AOIBlock;
        public Dictionary<long, GM_PlayerEntity> players;

        // NPC,
        // 敌人
        // 物品
    }

    public struct AOIPoint
    {
        public int gridX;
        public int gridY;

        public AOIPoint(int x, int z)
        {
            this.gridX = x;
            this.gridY = z;
        }
    }

    public class NineGridAOIMgr : AOIMgr
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int viewSize = 128 * 3; // 可以去做这个测试 
        private int blockSize;

        private int horBlockCount;
        private int verBlockCount;

        private AOIBlock[,] aoiBlocks = null;

        public override void Init(IMapWrapper mapWrapper, int viewSize = 128 * 3)
        {
            base.Init(mapWrapper, viewSize);

            // 九宫格AOI的初始化;
            this.viewSize = viewSize;
            this.blockSize = this.viewSize / 3;

            // xoz平面, 左下角为原点, Transform坐标，换成Block会快;
            this.horBlockCount = (mapWrapper.MapWidth() + this.blockSize - 1) / this.blockSize;
            this.verBlockCount = (mapWrapper.MapHeight() + this.blockSize - 1) / this.blockSize;

            this.aoiBlocks = new AOIBlock[this.horBlockCount, this.verBlockCount];

            for (int i = 0; i < this.horBlockCount; i++)
            {
                for (int j = 0; j < this.verBlockCount; j++)
                {
                    this.aoiBlocks[i, j] = new AOIBlock();
                    this.aoiBlocks[i, j].players = new Dictionary<long, GM_PlayerEntity>();
                }
            }
        }

        private void BroadMsgInGrid(AOIBlock block, Message s, long ignorePlayerId = -1, List<GM_PlayerEntity> toPlayers = null)
        {
            foreach (var playerId in block.players.Keys)
            {
                if (playerId == ignorePlayerId)
                {
                    continue;
                }

                this.SendMsg(block.players[playerId], s);
                if (toPlayers != null)
                {
                    toPlayers.Add(block.players[playerId]);
                }
            }
        }

        public override void BroadMessageToAOI(GM_PlayerEntity center, Message s, long ignorePlayerId = -1, List<GM_PlayerEntity> toPlayers = null)
        {
            if (center.uNineGrid.gridX < 0 || center.uNineGrid.gridY < 0)
            {
                this.logger.Warn("Invalid BlockX or BlockZ !");
                return;
            }

            // 中心
            int gridX = center.uNineGrid.gridX;
            int gridY = center.uNineGrid.gridY;

            AOIBlock block = this.aoiBlocks[gridX, gridY];
            this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            // end

            // 左边
            if (gridX - 1 >= 0)
            {
                block = this.aoiBlocks[gridX - 1, gridY];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 右边
            if (gridX + 1 < this.horBlockCount)
            {
                block = this.aoiBlocks[gridX + 1, gridY];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 上边
            if (gridY + 1 < this.verBlockCount)
            {
                block = this.aoiBlocks[gridX, gridY + 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end


            // 下边
            if (gridY - 1 >= 0)
            {
                block = this.aoiBlocks[gridX, gridY - 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 左上
            if (gridX - 1 >= 0 && gridY + 1 < this.verBlockCount)
            {
                block = this.aoiBlocks[gridX - 1, gridY + 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 左下
            if (gridX - 1 >= 0 && gridY - 1 >= 0)
            {
                block = this.aoiBlocks[gridX - 1, gridY - 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 右上
            if (gridX + 1 < this.horBlockCount && gridY + 1 < this.verBlockCount)
            {
                block = this.aoiBlocks[gridX + 1, gridY + 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

            // 右下
            if (gridX + 1 < this.horBlockCount && gridY - 1 >= 0)
            {
                block = this.aoiBlocks[gridX + 1, gridY - 1];
                this.BroadMsgInGrid(block, s, ignorePlayerId, toPlayers);
            }
            // end

        }

        private void BroadSelfRunningPlayerToOtherAOI(GM_PlayerEntity e, List<GM_PlayerEntity> toPlayers)
        {
            ResNavToDst res = new ResNavToDst();
            res.worldId = e.uWorld.worldId;
            res.x = e.uNineGrid.dstPosX;
            res.y = e.uNineGrid.dstPosY;
            res.z = e.uNineGrid.dstPosZ;
            res.speed = e.uProps.speed;

            for (int i = 0; i < toPlayers.Count; i++)
            {
                this.SendMsg(toPlayers[i], res);
            }
        }

        private void BroadOtherRunningPlayerToSelfAOI(GM_PlayerEntity e, List<GM_PlayerEntity> toPlayers)
        {
            for (int i = 0; i < toPlayers.Count; i++)
            {
                if (toPlayers[i] == e || toPlayers[i].uStatus.status != (int)CharactorStatus.Run)
                {
                    continue;
                }


                ResNavToDst res = new ResNavToDst();
                res.worldId = toPlayers[i].uWorld.worldId;
                res.x = toPlayers[i].uNineGrid.dstPosX;
                res.y = toPlayers[i].uNineGrid.dstPosY;
                res.z = toPlayers[i].uNineGrid.dstPosZ;
                res.speed = toPlayers[i].uProps.speed;
                this.SendMsg(e, res);

            }
        }


        public override void EnterToAOI(GM_PlayerEntity e)
        {
            long playerId = e.uPlayer.playerInfo.id;
            // 根据你的位置，算出来你是再哪个Block
            int gridX = ((int)(e.uTransform.pos.x)) / this.blockSize;
            int gridY = ((int)(e.uTransform.pos.z)) / this.blockSize;

            e.uNineGrid.gridX = gridX;
            e.uNineGrid.gridY = gridY;

            this.aoiBlocks[gridX, gridY].players.Add(playerId, e);
            // end

            List<GM_PlayerEntity> toPlayers = new List<GM_PlayerEntity>();
            // 告诉别人的客户端，你出现在他们的视野了;
            ResEnterAOI res = new ResEnterAOI();
            List<CharactorArrive> arrivedCharactors = new List<CharactorArrive>();
            CharactorArrive ch = this.GetCharactorArrive(e);
            arrivedCharactors.Add(ch);
            res.charactors = arrivedCharactors.ToArray();
            this.BroadMessageToAOI(e, res, playerId, toPlayers);
            // end


            if (e.uStatus.status == (int)CharactorStatus.Run)
            {
                this.BroadSelfRunningPlayerToOtherAOI(e, toPlayers);
            }


            // 还要把别人 + 你自己，发给你自己这个客户端,告诉客户端，这些人出现在你视野;
            toPlayers.Add(e);
            arrivedCharactors = new List<CharactorArrive>();
            res = new ResEnterAOI();

            for (int i = 0; i < toPlayers.Count; i++)
            {
                ch = this.GetCharactorArrive(toPlayers[i]);
                arrivedCharactors.Add(ch);
            }

            res.charactors = arrivedCharactors.ToArray();
            this.SendMsg(e, res);
            this.BroadOtherRunningPlayerToSelfAOI(e, toPlayers);
            // end
        }

        public override void LeaveFromAOI(GM_PlayerEntity e)
        {
            long playerId = e.uPlayer.playerInfo.id;
            int gridX = e.uNineGrid.gridX;
            int gridY = e.uNineGrid.gridY;

            if (gridX < 0 || gridY < 0)
            {
                return;
            }

            ResLeaveAOI res = new ResLeaveAOI();
            res.leavePlayers = new int[1]; // 当我们一个块进入视野，一个块在视野消失,还是用数组;
            res.leavePlayers[0] = e.uWorld.worldId;

            this.BroadMessageToAOI(e, res, playerId); // 是否排除自己，都可以，看你的需求;

            // 把自己从Block删除掉
            AOIBlock block = this.aoiBlocks[gridX, gridY];
            block.players.Remove(playerId);
            // end

            // 离开AOI;
            e.uNineGrid.gridX = -1;
            e.uNineGrid.gridY = -1;
            // end 

        }

        private void GetAOIPoints(int gridX, int gridY, List<AOIPoint> points)
        {
            points.Add(new AOIPoint(gridX, gridY));
            // 左
            if (gridX - 1 >= 0)
            {
                points.Add(new AOIPoint(gridX - 1, gridY));
            }
            // end

            // 右
            if (gridX + 1 < this.horBlockCount)
            {
                points.Add(new AOIPoint(gridX + 1, gridY));
            }
            // end

            // 上
            if (gridY + 1 < this.verBlockCount)
            {
                points.Add(new AOIPoint(gridX, gridY + 1));
            }
            // end

            // 下
            if (gridY - 1 >= 0)
            {
                points.Add(new AOIPoint(gridX, gridY - 1));
            }
            // end

            // 左上
            if (gridX - 1 >= 0 && gridY + 1 < this.verBlockCount)
            {
                points.Add(new AOIPoint(gridX - 1, gridY + 1));
            }
            // end

            // 左下
            if (gridX - 1 >= 0 && gridY - 1 >= 0)
            {
                points.Add(new AOIPoint(gridX - 1, gridY - 1));
            }
            // end

            // 右上
            if (gridX + 1 < this.horBlockCount && gridY + 1 < this.verBlockCount)
            {
                points.Add(new AOIPoint(gridX + 1, gridY + 1));
            }
            // end

            // 右下
            if (gridX + 1 < this.horBlockCount && gridY - 1 >= 0)
            {
                points.Add(new AOIPoint(gridX + 1, gridY - 1));
            }
            // end
        }

        private bool isInList(List<AOIPoint> list, AOIPoint value)
        {

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].gridX == value.gridX && list[i].gridY == value.gridY)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnGridChangedUpdate(GM_PlayerEntity e)
        {
            long playerId = e.uPlayer.session.playerID;
            // 根据你的位置，算出来你是再哪个Block
            int gridX = ((int)(e.uTransform.pos.x)) / this.blockSize;
            int gridY = ((int)(e.uTransform.pos.z)) / this.blockSize;

            if (gridX == e.uNineGrid.gridX && gridY == e.uNineGrid.gridY)
            { // 九宫格无变化
                return;
            }

            List<AOIPoint> oldGrids = new List<AOIPoint>();
            this.GetAOIPoints(e.uNineGrid.gridX, e.uNineGrid.gridY, oldGrids);
            List<AOIPoint> newGrids = new List<AOIPoint>();
            this.GetAOIPoints(gridX, gridY, newGrids);

            // 玩家发生了格子变化;
            // step1: 找出在新九宫格的格子但是不在旧九宫格里的格子;
            // 通知这些格子，玩家来了;
            ResEnterAOI res = new ResEnterAOI();
            List<CharactorArrive> arrivedCharactors = new List<CharactorArrive>();
            CharactorArrive ch = this.GetCharactorArrive(e);
            arrivedCharactors.Add(ch);
            res.charactors = arrivedCharactors.ToArray();
            // end

            List<GM_PlayerEntity> toPlayers = new List<GM_PlayerEntity>();
            for (int i = 0; i < newGrids.Count; i++)
            {
                if (!this.isInList(oldGrids, newGrids[i]))
                {
                    AOIBlock block = this.aoiBlocks[newGrids[i].gridX, newGrids[i].gridY];
                    this.BroadMsgInGrid(block, res, playerId, toPlayers);
                }
            }

            // 通知玩家，你有新的人进来了
            if (toPlayers.Count > 0)
            {
                this.BroadSelfRunningPlayerToOtherAOI(e, toPlayers);
                arrivedCharactors = new List<CharactorArrive>();
                res = new ResEnterAOI();

                for (int i = 0; i < toPlayers.Count; i++)
                {
                    ch = this.GetCharactorArrive(toPlayers[i]);
                    arrivedCharactors.Add(ch);
                }
                res.charactors = arrivedCharactors.ToArray();
                this.SendMsg(e, res);
                this.BroadOtherRunningPlayerToSelfAOI(e, toPlayers);
            }

            // end

            // step2: 找出不在新九宫格里的格子，但是在旧九宫格里的格子;
            ResLeaveAOI leaveRes = new ResLeaveAOI();
            leaveRes.leavePlayers = new int[1]; // 当我们一个块进入视野，一个块在视野消失,还是用数组;
            leaveRes.leavePlayers[0] = e.uWorld.worldId;
            toPlayers.Clear();

            for (int i = 0; i < oldGrids.Count; i++)
            {
                if (!this.isInList(newGrids, oldGrids[i]))
                {
                    AOIBlock block = this.aoiBlocks[oldGrids[i].gridX, oldGrids[i].gridY];
                    // 广播告诉这些块里面的玩家，你离开视野了;
                    this.BroadMsgInGrid(block, leaveRes, playerId, toPlayers);
                }
            }
            // end

            // 再来通知玩家对应的客户端，toPlayers 这些玩家要移出你的视野
            if (toPlayers.Count > 0)
            {
                leaveRes = new ResLeaveAOI();
                leaveRes.leavePlayers = new int[toPlayers.Count];

                for (int i = 0; i < toPlayers.Count; i++)
                {
                    leaveRes.leavePlayers[i] = toPlayers[i].uWorld.worldId;
                }

                this.SendMsg(e, leaveRes);
            }

            // 更新格子
            AOIBlock oldBlock = this.aoiBlocks[e.uNineGrid.gridX, e.uNineGrid.gridY];
            oldBlock.players.Remove(playerId);

            AOIBlock newBlock = this.aoiBlocks[gridX, gridY];
            newBlock.players.Add(playerId, e);

            e.uNineGrid.gridX = gridX;
            e.uNineGrid.gridY = gridY;
        }

        private void OnNavSystemUpdate(float dt)
        {
            foreach (var block in this.aoiBlocks)
            {
                foreach (var e in block.players.Values)
                {
                    if (e.uStatus.status == (int)CharactorStatus.Run)
                    {
                        this.mapWrapper.NavUpdate(this, e, dt);

                        // 做好格子的变化处理;
                        this.OnGridChangedUpdate(e);
                        // end
                    }
                }
            }

        }

        private void OnSkillAndBuffUpdate(float dt)
        {
            foreach (var block in this.aoiBlocks)
            {
                foreach (var e in block.players.Values)
                {
                    SkillAndBuffSystem.Update(this, e, dt);
                }
            }
        }

        public override void OnUpdate(float dt)
        {
            // 其它System的迭代;
            // end

            // NavSystem迭代
            this.OnNavSystemUpdate(dt);
            // end

            // SkillAndBuff迭代
            this.OnSkillAndBuffUpdate(dt);
            // end
        }
    }
}
