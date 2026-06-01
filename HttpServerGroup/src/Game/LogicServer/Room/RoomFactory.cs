

using Framework.Core.task;
using Framework.Core.Utils;
using Game.Datas.Excels;
using Game.Utils;
using System.Collections.Generic;

namespace Game.LogicServer
{
    public class RoomFactory
    {

        public static RoomMgr CreateRoom(int logicServerTid, int zoneId)
        {
            switch (logicServerTid)
            {
                case 1:
                    if (zoneId == 10001)
                    {
                        return new RoundRoomMgr();
                    }
                    else if (zoneId == 10002)
                    {
                        return new FightRoomMgr();
                    }
                    break;
                case 2:
                    return new RoundRoomMgr();
                case 3:
                    if (zoneId == 200020)
                    {
                        return new WorldIndepRoomMgr();
                    }
                    return new WorldIndepRoomMgr();
            }
            return null;
        }

    }
}

