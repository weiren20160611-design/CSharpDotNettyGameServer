using Framework.Core.Serializer;
using Framework.Core.Utils;
using ProtoBuf;


namespace Game.Datas.Messages
{
    //生成玩家请求
    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerSpawnReq)]
    public class ReqPlayerSpawn : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int spawnPoint;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerSpawnRes)]
    public class ResPlayerSpawn : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public int worldId;
    }

    //生成玩家请求
    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eNavToDstReq)]
    public class ReqNavToDst : Message
    {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        [ProtoMember(3)]
        public float z;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eNavToDstRes)]
    public class ResNavToDst : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int worldId;

        [ProtoMember(2)]
        public float x;

        [ProtoMember(3)]
        public float y;

        [ProtoMember(4)]
        public float z;

        [ProtoMember(5)]
        public float speed;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eNavToDirReq)]
    public class ReqNavToDir : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int dirX;

        [ProtoMember(2, IsRequired = true)]
        public int dirY;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eNavToDirRes)]
    public class ResNavToDir : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int worldId;

        [ProtoMember(2)]
        public int dirX;

        [ProtoMember(3)]
        public int dirY;

        [ProtoMember(4)]
        public float speed;
    }





    #region 服务器主动通知的消息
    [ProtoContract]
    public class CharactorInfo
    {
        [ProtoMember(1)]
        public string unick;

        [ProtoMember(2)]
        public int job;

        [ProtoMember(3)]
        public int sex;

        [ProtoMember(4)]
        public int charactorId;
    }

    [ProtoContract]
    public class CharactorTransform
    {
        [ProtoMember(1)]
        public float[] pos;

        [ProtoMember(2)]
        public float[] eulerAngles;

    }


    [ProtoContract]
    public class CharactorArrive
    {
        [ProtoMember(1, IsRequired = true)]
        public int worldId;

        [ProtoMember(2)]
        public CharactorInfo charactorInfo;

        [ProtoMember(3)]
        public CharactorTransform transform;

        [ProtoMember(4)]
        public int charactorStatus;
    }


    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eEnterAOIRes)]
    public class ResEnterAOI : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public CharactorArrive[] charactors;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eLeaveAOIRes)]
    public class ResLeaveAOI : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int[] leavePlayers;
    }

    [ProtoContract]
    public class CharactorStatusData
    {
        [ProtoMember(1, IsRequired = true)]
        public int aniStatus;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eSyncCharactorRes)]
    public class ResSyncCharactorStatus : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int worldId;

        [ProtoMember(2, IsRequired = true)]
        public CharactorStatusData statusData;

        [ProtoMember(3, IsRequired = true)]
        public CharactorTransform transform;

    }
    #endregion 服务器主动通知的消息
}
