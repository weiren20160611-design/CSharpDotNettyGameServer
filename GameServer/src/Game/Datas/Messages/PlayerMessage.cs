using Framework.Core.Serializer;
using Framework.Core.Utils;
using ProtoBuf;


namespace Game.Datas.Messages
{
    //选择角色
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eSelectPlayerReq)]
    public class ReqSelectPlayer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int job;

        [ProtoMember(2)]
        public string name;

        [ProtoMember(3)]
        public int usex;

        [ProtoMember(4)]
        public int charactorId;


    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eSelectPlayerRes)]
    public class ResSelectPlayer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status = 0;

        [ProtoMember(2, IsRequired = true)]
        public int isReConnectGame;

        [ProtoMember(3)]
        public PlayerInfo pInfo = null;

        [ProtoMember(4)]
        public long playerId;
    }

    //拉取玩家数据
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingPlayerDataReq)]
    public class ReqPullingPlayerData : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int job;        
    }

    [ProtoContract]
    public class PlayerInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public string uname;

        [ProtoMember(2, IsRequired = true)]
        public int hp;

        [ProtoMember(3, IsRequired = true)]
        public int exp;

        [ProtoMember(4, IsRequired = true)]
        public int mp;

        [ProtoMember(5, IsRequired = true)]
        public int umoney;

        [ProtoMember(6, IsRequired = true)]
        public int ucoin;

        [ProtoMember(7, IsRequired = true)]
        public int usex;

        [ProtoMember(8, IsRequired = true)]
        public int hasBonues = 0;

        [ProtoMember(9, IsRequired = true)]
        public int days = 0;

        [ProtoMember(10, IsRequired = true)]
        public int bonuesNum = 0;
    }



    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingPlayerDataRes)]
    public class ResPullingPlayerData : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status = 0;

        [ProtoMember(2, IsRequired = true)]
        public int isReConnectGame;

        [ProtoMember(3)]
        public PlayerInfo pInfo = null;

        [ProtoMember(4)]
        public long playerId;

    }

    //领取登录奖励
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eRecvLoginBonuesReq)]
    public class ReqRecvLoginBonues : Message
    {
        [ProtoMember(1)]
        public int type;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eRecvLoginBonuesRes)]
    public class ResRecvLoginBonues : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;


        [ProtoMember(2, IsRequired = true)]
        public int num;
    }


    //拉取背包数据
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingBackpackDataReq)]
    public class ReqPullingBackpackData : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;
    }

    [ProtoContract]
    public class GoodsItem
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;

        [ProtoMember(2, IsRequired = true)]
        public int num;

        [ProtoMember(3)]
        public byte[] strengData = null;

    }

    [ProtoContract]
    public class DicGoodsItem
    {
        [ProtoMember(1, IsRequired = true)]
        public int mainTypeId;

        [ProtoMember(2, IsRequired = true)]
        public GoodsItem[] Value = null;

    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingBackpackDataRes)]
    public class ResPullingBackpackData : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public DicGoodsItem[] packGoods = null;
    }

    //拉取奖励列表
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingBonuesListReq)]
    public class ReqPullingBonuesList : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;
    }

    [ProtoContract]
    public class BonuesItem
    {
        [ProtoMember(1, IsRequired = true)]
        public long bonuesId;

        [ProtoMember(2, IsRequired = true)]
        public string bonuesDesic;

        [ProtoMember(3, IsRequired = true)]
        public int status;

        [ProtoMember(4)]
        public int typeId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingBonuesListRes)]
    public class ResPullingBonuesList : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public BonuesItem[] bonuesArray = null;
    }

    //拉取排行数据
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingRankListReq)]
    public class ReqPullingRankList : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;

        [ProtoMember(2, IsRequired = true)]
        public int num;
    }

    [ProtoContract]
    public class RankItem
    {
        [ProtoMember(1, IsRequired = true)]
        public string unick;

        [ProtoMember(2, IsRequired = true)]
        public int ucoin;

        [ProtoMember(3, IsRequired = true)]
        public int uface;
    }


    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingRankListRes)]
    public class ResPullingRankList : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public RankItem[] rankList;

        [ProtoMember(3, IsRequired = true)]
        public int selfIndex;
    }

    //拉取任务数据
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingTaskListReq)]
    public class ReqPullingTaskList : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;
    }

    [ProtoContract]
    public class TaskItem
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public string taskDesic;

        [ProtoMember(3)]
        public int typeId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingTaskListRes)]
    public class ResPullingTaskList : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public TaskItem[] taskArray = null;
    }


    //拉取邮件数据
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingEmailMsgReq)]
    public class ReqPullingEmailMsgList : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int msgStatus;
    }

    [ProtoContract]
    public class EmailMsgItem
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2, IsRequired = true)]
        public string content;

        [ProtoMember(3, IsRequired = true)]
        public int emailMsgId;

        [ProtoMember(4, IsRequired = true)]
        public int sendTime;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.ePullingEmailMsgRes)]
    public class ResPullingEmailMsgList : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public EmailMsgItem[] msgItemArray = null;
    }

    //更新邮件状态
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eUpdateEmailMsgStatusReq)]
    public class ReqUpdateEmailMsgStatus : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public long msgId;

        [ProtoMember(2, IsRequired = true)]
        public int msgStatus;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eUpdateEmailMsgStatusRes)]
    public class ResUpdateEmailMsgStatus : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;
    }


    //兑换商品
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eExchangeProductReq)]
    public class ReqExchangeProduct : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eExchangeProductRes)]
    public class ResExchangeProduct : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;
    }

    //领取奖励
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eRecvBonuesReq)]
    public class ReqRecvBonues : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public long bonuesId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eRecvBonuesRes)]
    public class ResRecvBonues : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public int b1;

        [ProtoMember(3)]
        public int b2;

        [ProtoMember(4)]
        public int b3;

        [ProtoMember(5)]
        public int b4;

        [ProtoMember(6)]
        public int b5;

        [ProtoMember(7)]
        public int typeId;

    }


    //进入逻辑服
    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eEnterLogicServercReq)]
    public class ReqEnterLogicServer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int typeId;

        [ProtoMember(2, IsRequired = true)]
        public int zoneId;

        [ProtoMember(3, IsRequired = true)]
        public int instanceId;
    }

    [ProtoContract]
    [MessageMeta((short)Module.PLAYER, (short)Cmd.eEnterLogicServercRes)]
    public class ResEnterLogicServer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

    }

    //退出逻辑服
    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eQuitLogicServerReq)]
    public class ReqQuitLogicServer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int quitReason;

    }

    [ProtoContract]
    [MessageMeta((short)Module.SCENE, (short)Cmd.eQuitLogicServerRes)]
    public class ResQuitLogicServer : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

    }
}
