using Framework.Core.Serializer;
using Framework.Core.Utils;
using ProtoBuf;


namespace Game.Datas.Messages
{
    //游客登录
    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eGuestLoginReq)]
    public class ReqGuestLogin : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string guestKey;

        [ProtoMember(2, IsRequired = true)]
        public int channal;
    }

    [ProtoContract]
    public class AccountInfo
    {
        [ProtoMember(1, IsRequired = true)]
        public string unick;

        [ProtoMember(2, IsRequired = true)]
        public int uface;

        [ProtoMember(3, IsRequired = true)]
        public int uvip;

        [ProtoMember(4, IsRequired = true)]
        public int isGuest;

    }


    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eGuestLoginRes)]
    public class ResGuestLogin : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public AccountInfo uinfo = null;
    }

    //游客账号升级
    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eGuestUpgradeReq)]
    public class ReqGuestUpgrade : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string uname;

        [ProtoMember(2, IsRequired = true)]
        public string upwd;

        [ProtoMember(3, IsRequired = true)]
        public string unick;
    }

    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eGuestUpgradeRes)]
    public class ResGuestUpgrade : Message
    {

        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public AccountInfo accountInfo = null;
    }


    //注册账号
    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eRegisterUserReq)]
    public class ReqRegisterUser : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string uname;

        [ProtoMember(2, IsRequired = true)]
        public string upwd;

        [ProtoMember(3, IsRequired = true)]
        public int channal;
    }

    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eRegisterUserRes)]
    public class ResRegisterUser : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status;

        [ProtoMember(2)]
        public string errorMsg;
    }


    //用户登录
    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eUserLoginReq)]
    public class ReqUserLogin : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public string uname;

        [ProtoMember(2, IsRequired = true)]
        public string upwd;

        [ProtoMember(3, IsRequired = true)]
        public int channal;
    }

    [ProtoContract]
    [MessageMeta((short)Module.AUTH, (short)Cmd.eUserLoginRes)]
    public class ResUserLogin : Message
    {
        [ProtoMember(1, IsRequired = true)]
        public int status = 0;

        [ProtoMember(2)]
        public AccountInfo accountInfo = null;
    }
}

