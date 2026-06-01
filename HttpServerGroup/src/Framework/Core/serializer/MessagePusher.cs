using Framework.Core.Net;
using Framework.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Serializer
{
    public class MessagePusher {
        public static void PushMessage(IdSession s, Message m) {
            byte[] msgBody = SerializerHelper.PBEncoder(m);
            if (msgBody == null) {
                return;
            }

            int metaSize = 8; // [2, 2, 4]
            byte[] body = new byte[metaSize + msgBody.Length];

            UtilsHelper.WriteShortLE(body, 0, m.GetModule());
            UtilsHelper.WriteShortLE(body, 2, m.GetCmd());
            UtilsHelper.WriteUintLE(body, 4, 0);
            UtilsHelper.WriteBytes(body, metaSize, msgBody);

            s.Send(body); // Tcp, WebSocket
        }
    }

}
