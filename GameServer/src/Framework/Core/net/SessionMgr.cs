using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System.Net;
using System.Threading;

namespace Framework.Core.Net
{
    public class SessionMgr
    {
        public static SessionMgr Instance = new SessionMgr();

        private AttributeKey<IdSession> SESSION_KEY = AttributeKey<IdSession>.ValueOf("session");
        private long autoId = 0;
        public IdSession gateWaySession = null;
        public void Init()
        {
            this.autoId = 0;

        }


        public void SetSessionChannel(IdSession s, IChannel channel)
        {
            s.client = channel;
            IAttribute<IdSession> sessionAttr = channel.GetAttribute(SESSION_KEY);
            sessionAttr.CompareAndSet(null, s);
        }

        public void GateWayConnectToServer(IChannel channel)
        {

            if (this.gateWaySession == null)
            {
                this.gateWaySession = this.CreateIdSession(channel, false);
            }
            else
            {
                this.gateWaySession.client = channel;
            }

        }

        public void GatewayDisConnectFromServer()
        {
            this.gateWaySession.client = null;
        }

        public IdSession CreateIdSession(IChannel channel, bool isWebSocket = false)
        {
            //创建一个会话
            IdSession session = new IdSession();
            //会话的玩家ID
            session.accountId = 0;
            session.playerId = 0;
            //会话的ID（从0开始的自增长ID）
            long id = Interlocked.Increment(ref autoId);
            session.sessionId = id;
            //会话的通道
            session.client = channel;
            //会话是否是WebSocket
            session.isWebSocket = isWebSocket;

            // channel --->保存了 session;
            if (channel != null)
            {
                IAttribute<IdSession> sessionAttr = channel.GetAttribute(SESSION_KEY);
                sessionAttr.CompareAndSet(null, session);
            }


            return session;
        }

        public IdSession GetSessionBy(IChannel channel)
        {
            IAttribute<IdSession> sessionAttr = channel.GetAttribute(SESSION_KEY);
            return sessionAttr.Get();
        }

    }
}

