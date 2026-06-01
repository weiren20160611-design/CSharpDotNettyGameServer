using Game.Core.Db;
using Game.Core.GM_Backpack;
using Game.Datas.DBEntities;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game.Datas.GMEntities
{
    public struct PropsComponent
    {
        public float speed;
        public float hp;
        public float attack;
        public float defense;

        public static void Init(GM_PlayerEntity player)
        {
            player.uProps.speed = 5.0f;
            player.uProps.hp = 500;
            player.uProps.attack = 50.0f;
            player.uProps.defense = 15.0f;
        }

        public static void Reset(GM_PlayerEntity player)
        {
            player.uProps.speed = 5.0f;
            player.uProps.hp = 500;
            player.uProps.attack = 50.0f;
            player.uProps.defense = 15.0f;
        }

        public static void Exit(GM_PlayerEntity playerEntity)
        {

        }
    }
}
