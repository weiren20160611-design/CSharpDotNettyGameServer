namespace Game
{
    public enum ServerType
    {
        Logic1 = 1,
        Logic2 = 2,
        Logic2_FB = 3,
        // ...
    }

    public enum OperationType
    {
        None = -1,

    }

    public enum Module
    {

        /** 登录 */
        AUTH = 101,
        /** 玩家 */
        PLAYER = 102,
        /** 场景 */
        SCENE = 103,
        /** 活动 */
        ACTIVITY = 104,
        /** 技能 */
        SKILL = 105,
        /** 聊天 */
        CHAT = 106,

        // ------------------跨服业务功能模块（501开始）---------------------
        /** 跨服天梯 */
        LADDER = 501
    }

    public enum Cmd
    {
        // 游客登录
        eGuestLoginReq = 1,
        eGuestLoginRes,

        //用户注册
        eRegisterUserReq,
        eRegisterUserRes,

        //用户登录
        eUserLoginReq,
        eUserLoginRes,

        // 游客升级
        eGuestUpgradeReq,
        eGuestUpgradeRes,

        //拉取玩家数据
        ePullingPlayerDataReq,
        ePullingPlayerDataRes,

        //拉取玩家Bonues列表
        ePullingBonuesListReq,
        ePullingBonuesListRes,

        //拉取玩家任务列表
        ePullingTaskListReq,
        ePullingTaskListRes,

        //拉取玩家排行榜
        ePullingRankListReq,
        ePullingRankListRes,

        //拉取玩家邮件列表
        ePullingEmailMsgReq,
        ePullingEmailMsgRes,

        //拉取玩家背包数据
        ePullingBackpackDataReq,
        ePullingBackpackDataRes,

        //更新玩家邮件状态
        eUpdateEmailMsgStatusReq,
        eUpdateEmailMsgStatusRes,

        //领取登录Bonues
        eRecvLoginBonuesReq,
        eRecvLoginBonuesRes,

        //领取Bonues
        eRecvBonuesReq,
        eRecvBonuesRes,

        //选择玩家
        eSelectPlayerReq,
        eSelectPlayerRes,

        //兑换商品
        eExchangeProductReq,
        eExchangeProductRes,

        //进入逻辑服
        eEnterLogicServercReq,
        eEnterLogicServercRes,

        //玩家退出逻辑服
        eQuitLogicServerReq,
        eQuitLogicServerRes,

        ePlayerSitdownReq,
        ePlayerSitdownRes,

        ePlayerStandupReq,
        ePlayerStandupRes,

        eSendChatMessageReq,
        eSendChatMessageRes,

        ePlayerReadyReq,
        ePlayerReadyRes,

        ePlayerOperationReq,
        ePlayerOperationRes,

        //房间模式:服务器主动通知消息
        eEnterRoomRes,
        eUserArrivedRes,
        eUserExitSeatRes,
        eReadyGameRes,
        eStartRoundGameRes,
        eCheckOutGameRes,
        eGameOverRes,
        eReConnectRoomRes,
        ePlayerEscape,

        //战斗房间命令
        eStartSkillReq,
        eStartSkillRes,
        eStartBuffReq,
        eStartBuffRes,

        //战斗房间:服务器主动通知消息
        eLostHpRes,


        //开放世界的命令
        ePlayerSpawnReq,
        ePlayerSpawnRes,
        eNavToDstReq,
        eNavToDstRes,

        //开放世界:服务器主动通知消息
        eEnterAOIRes,
        eLeaveAOIRes,
        eSyncCharactorRes,



        //测试获取物品
        eTestGetGoodsReq,
        eTestGetGoodsRes,

        eTestUpdateGoodsReq,
        eTestUpdateGoodsRes,

        eTestLogicCmdEchoReq,
        eTestLogicCmdEchoRes,


    }


    public enum Respones
    {
        OK = 1,
        CollectSuccess = 2,
        TaskDone = 3,


        SystemErr = -100,
        UserIsFreeze = -101,
        UserIsNotGuest = -102,
        InvalidParams = -103,
        UnameIsExist = -104,
        UnameOrUpwdError = -105,
        InvalidOpt = -106,
        PlayerIsNotExist = -107,
        AccountIsNotLogin = -108,
        PlayerIsFreeze = -109,
        UserIsNotExist = -110,
        UserPasswordError = -111,
        NotEnoughMoney = -112,
        LogicServerIsBusy = -113,
        AlreadyInLogicServer = -114,
        LogicServerIsNotExist = -115,
        UserIsPlaying = -116,
        ConditionNotAccord = -117,
        RoomSeatFull = -118,

        // ...
    }

    public enum QuitReason
    {
        VoluntaryQuit = 1,
        ForcedQuit = 2,
        DisconnectQuit = 3,
    }


    public enum Channal
    {
        InvalidChannal = -1,
        SelfChannal = 0,
        DouYin,
        IosAppStore,
        // ...
    }

    public enum BonuesType
    {
        UpgradeBonues = 100000,
        // ...
    }
    public enum RankType
    {
        WorldCoin = 1,
        // ...
    }


    public enum RuleType
    {
        Task = 1,
        Trading,
        Bonues,
        Backpack,
    }
}

