using Game.Datas.DBEntities;

namespace Game.Datas.GMEntities
{
    public class GM_PlayerEntity
    {
        public PlayerComponent uPlayer;
        public TaskComponent uTask;
        public BackpackComponent uPack;
        public GameRoomComponent uGameRoom;
        public WorldComponent uWorld;
        public TransformComponent uTransform;
        public StatusComponent uStatus;
        public NavComponent uNav;
        public AStarComponent uAStar;
        public NineGridComponent uNineGrid;
        public PropsComponent uProps;
        public SkillAndBuffComponent uSkillAndBuff;
    }
}