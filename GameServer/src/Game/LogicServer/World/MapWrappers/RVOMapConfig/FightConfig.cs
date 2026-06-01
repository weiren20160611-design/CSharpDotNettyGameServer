using System;

namespace Game.LogicServer.RVOMapConfig
{
    [Serializable]
    public class FightConfig
    {
        public float moveSpeed = 0.05f;

        public int hp;
        public int baseAttack;
        public int baseDefense;

        public float attackR;
        public float visualR;

        public int useAI = 0;
        // ...
    }
}
