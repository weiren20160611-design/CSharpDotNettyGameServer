using Framework.Core.Serializer;
using Framework.Core.Utils;
using ProtoBuf;

namespace Game.Datas.Messages
{
    #region 服务器主动通知消息 
    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eEnterRoomRes)]
    public class ResUserEnterRoom : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int roomId;

        [ProtoMember(2, IsRequired = true)]
        public int roomState;

        [ProtoMember(3, IsRequired = true)]
        public int roomOnLookId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eUserArrivedRes)]
    public class ResUserArrivedSeat : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public string unick;

        [ProtoMember(2, IsRequired = true)]
        public int usex;

        [ProtoMember(3, IsRequired = true)]
        public int uface;

        [ProtoMember(4, IsRequired = true)]
        public int seatId;

        [ProtoMember(5, IsRequired = true)]
        public int playerInRoomState;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eUserExitSeatRes)]
    public class ResUserExitSeat : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int seatId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eReadyGameRes)]
    public class ResReadyGame : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int countDown;

        [ProtoMember(2, IsRequired = true)]
        public int isReadyPlayer;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eStartRoundGameRes)]
    public class ResStartGame : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int gameDuration;

    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eCheckOutGameRes)]
    public class ResCheckOutGame : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int reserve;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eGameOverRes)]
    public class ResGameOver : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int reserve;

    }

    [ProtoContract]
    public class SelfInRoomInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public int roomId;

        [ProtoMember(2, IsRequired = true)]
        public int roomState;

        [ProtoMember(3, IsRequired = true)]
        public int roomOnLookId;

        [ProtoMember(4, IsRequired = true)]
        public int selfSeatId;

    }
    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eReConnectRoomRes)]
    public class ResReConnectSyncData : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public ResUserArrivedSeat[] userArrivedDatas;

        [ProtoMember(2, IsRequired = true)]
        public SelfInRoomInfo selfInRoomInfo;

    }


    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerEscape)]
    public class ResPlayerEscape : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int seatId;

        [ProtoMember(2, IsRequired = true)]
        public string unick;

    }

    #endregion



    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerSitdownReq)]
    public class ReqSitdown : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int seatId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerSitdownRes)]
    public class ResSitdown : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public int seatId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerStandupReq)]
    public class ReqStandup : Message
    {
        [ProtoMember(1)]
        public int seatId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerStandupRes)]
    public class ResStandup : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eSendChatMessageReq)]
    public class ReqSendChatMessage : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int talkType;

        [ProtoMember(2, IsRequired = true)]
        public string talkContent;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eSendChatMessageRes)]
    public class ResSendChatMessage : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int onLookId;

        [ProtoMember(2, IsRequired = true)]
        public int talkType;

        [ProtoMember(3, IsRequired = true)]
        public string talkContent;
    }


    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerReadyReq)]
    public class ReqPlayerReady : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int seatId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerReadyRes)]
    public class ResPlayerReady : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public int seatId;
    }


    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerOperationReq)]
    public class ReqPlayerOperation : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int operationType;

        [ProtoMember(2)]
        public int v1;

        [ProtoMember(3)]
        public int v2;

        [ProtoMember(4)]
        public int v3;
    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.ePlayerOperationRes)]
    public class ResPlayerOperation : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public int seatId;

        [ProtoMember(3)]
        public int operationType;

        [ProtoMember(4)]
        public int v1;

        [ProtoMember(5)]
        public int v2;

        [ProtoMember(6)]
        public int v3;
    }



}
