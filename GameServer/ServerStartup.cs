using System;
using System.Threading.Tasks;

class ServerStartup
{
    static async Task<int> Main(string[] args)
    {
#if true
        Game.GameServer.Instance.Start();

        Console.ReadLine();

        await Game.GameServer.Instance.Shutdown();
#else
        {
            var opt = new SchemaGenerationOptions { 

                Types = {
                    typeof(ReqGuestLogin),
                    typeof(ReqUserLogin),
                    typeof(ResUserLogin),
                    typeof(AccountInfo),
                    typeof(ResGuestLogin),
                    typeof(ReqPullingPlayerData),
                    typeof(ResPullingPlayerData),
                    typeof(ReqSelectPlayer),
                    typeof(ResSelectPlayer),
                    typeof(PlayerInfo),
                    typeof(ReqRegisterUser),
                    typeof(ResRegisterUser),
                    typeof(ReqGuestUpgrade),
                    typeof(ResGuestUpgrade),
                    typeof(ReqRecvLoginBonues),
                    typeof(ResRecvLoginBonues),
                    typeof(ReqPullingBonuesList),
                    typeof(ResPullingBonuesList),
                    typeof(ReqRecvBonues),
                    typeof(ResRecvBonues),
                    typeof(ReqPullingTaskList),
                    typeof(ResPullingTaskList),
                    typeof(ReqTestGetGoods),
                    typeof(ResTestGetGoods),
                    typeof(ReqTestUpdateGoods),
                    typeof(ResTestUpdateGoods),
                    typeof(ReqPullingEmailMsgList),
                    typeof(ResPullingEmailMsgList),
                    typeof(ReqUpdateEmailMsgStatus),
                    typeof(ResUpdateEmailMsgStatus),
                    typeof(ReqPullingRankList),
                    typeof(ResPullingRankList),
                    typeof(ReqPullingBackpackData),
                    typeof(ResPullingBackpackData),
                    typeof(GoodsItem),
                    typeof(DicGoodsItem),
                    typeof(ReqExchangeProduct),
                    typeof(ResExchangeProduct),
                    typeof(ReqEnterLogicServer),
                    typeof(ResEnterLogicServer),
                    typeof(ReqTestLogicCmdEcho),
                    typeof(ResTestLogicCmdEcho),
                    typeof(ReqQuitLogicServer),
                    typeof(ResQuitLogicServer),
                    typeof(ReqSitdown),
                    typeof(ResSitdown),
                    typeof(ReqStandup),
                    typeof(ResStandup),
                    typeof(ResUserEnterRoom),
                    typeof(ResUserArrivedSeat),
                    typeof(ResUserExitSeat),
                    typeof(ReqSendChatMessage),
                    typeof(ResSendChatMessage),
                    typeof(ReqPlayerReady),
                    typeof(ResPlayerReady),
                    typeof(ResReadyGame),
                    typeof(ResStartGame),
                    typeof(ResCheckOutGame),
                    typeof(ResGameOver),
                    typeof(ResSyncRoomData),
                    typeof(ReqPlayerOperation),
                    typeof(ResPlayerOperation),
                    typeof(ResPlayerEscape),
                    typeof(ReqPlayerSpawn),
                    typeof(ResPlayerSpawn),
                    typeof(ResEnterAOI),
                    typeof(ResLeaveAOI),
                    typeof(CharactorArrive),
                    typeof(CharactorInfo),
                    typeof(CharactorTransform),
                    typeof(ResSyncCharactorStatus),
                    typeof(ReqStartSkill),
                    typeof(ResStartSkill),
                    typeof(ReqStartBuff),
                    typeof(ResStartBuff),
                    typeof(ResLostHp),
                    typeof(ReqNavToDir),
                    typeof(ResNavToDir),
                    // ...
                },

                Package = "",
                Syntax = ProtoSyntax.Proto3, 
            };


            string protoString = Serializer.GetProto(opt);
            Console.WriteLine(protoString);
            File.WriteAllText("./game.proto", protoString);
        }
#endif
        return 0;
    }
}
