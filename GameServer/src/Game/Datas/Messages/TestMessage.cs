using Framework.Core.Serializer;
using Framework.Core.Utils;
using ProtoBuf;

namespace Game.Datas.Messages
{
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eTestGetGoodsReq)]
    public class ReqTestGetGoods : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;

        [ProtoMember(2, IsRequired = true)]
        public int count;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eTestGetGoodsRes)]
    public class ResTestGetGoods : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

    }


    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eTestUpdateGoodsReq)]
    public class ReqTestUpdateGoods : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;

        [ProtoMember(2, IsRequired = true)]
        public int count;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eTestUpdateGoodsRes)]
    public class ResTestUpdateGoods : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eTestLogicCmdEchoReq)]
    public class ReqTestLogicCmdEcho : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string content;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eTestLogicCmdEchoRes)]
    public class ResTestLogicCmdEcho : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string content;
    }
}