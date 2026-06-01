using Framework.Core.Net;
using Game.Datas.DBEntities;

namespace Game.Datas.GMEntities
{
    public struct PlayerComponent
    {
        public Account accountInfo;
        public Player playerInfo;
        public IdSession session;
    }
}