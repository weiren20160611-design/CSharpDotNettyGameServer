using Framework.Core.Utils;
using Game.Core.Db;
using Game.Datas.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.EmailMessage
{
    public enum EmailStatus
    {
        UnRead = 0,
        Read = 1,
        Delete = 2
    }
    public class GM_EmailMgr
    {
        public static GM_EmailMgr Instance = new GM_EmailMgr();
        public void Init()
        {

        }

        public void SendEmail(long fromPlayerId, long toPlayerId, string msgBody, long udata)
        {
            Emailmessage msg = new Emailmessage();
            msg.fromPlayerId = fromPlayerId;
            msg.toPlayerId = toPlayerId;
            msg.msgBody = msgBody;
            msg.userData = udata;
            msg.status = (int)EmailStatus.UnRead;
            msg.sendTime = (int)UtilsHelper.Timestamp();
            msg.readTime = -1;
            DBService.Instance.GetGameInstance().Insertable(msg).ExecuteCommand();
        }

        public Emailmessage[] PullingMessageList(long playerId, int status)
        {
            Emailmessage[] msg = null;
            msg = DBService.Instance.GetGameInstance().Queryable<Emailmessage>().Where(it => it.toPlayerId == playerId && it.status != status).ToArray();
            return msg;
        }

        public bool UpdateMessageStatus(long msgId, int status)
        {
            if (status < (int)EmailStatus.UnRead || status > (int)EmailStatus.Delete)
            {
                return false;
            }
            Emailmessage msg = DBService.Instance.GetGameInstance().Queryable<Emailmessage>().Where(it => it.id == msgId).First();
            if (msg != null)
            {
                msg.status = status;
                if (status == (int)EmailStatus.Read)
                {
                    msg.readTime = (int)UtilsHelper.Timestamp();
                }               
                DBService.Instance.GetGameInstance().Updateable(msg).Where(it => it.id == msgId).ExecuteCommand();
                return true;
            }
            return false;
        }
    }
}
