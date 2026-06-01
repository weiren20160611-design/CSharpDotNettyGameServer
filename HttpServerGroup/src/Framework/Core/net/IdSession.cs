using DotNetty.Buffers;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;
using System.Net;

namespace Framework.Core.Net {
    public class IdSession {

        public IChannel client = null;

        public long sessionId = 0; //  线程池分发器的索引, 每个IdSession都对应唯一的编号; 

        public long playerID = 0; // 玩家对应的游戏ID号;

        public long accountID = 0; // 玩家对应的账号ID号;

        public long accountIdAndJob = 0; // 玩家ID和职业ID;

        public int logicServerId = -1; //玩家分配的逻辑服ID;

        public bool isWebSocket = false; // 当前的session是WebSocket还是TcpSocket;

        public string GetIp() {
            return ((IPEndPoint)this.client.RemoteAddress).Address.ToString().Substring(7);
        }

        public string GetRemoteAddress()
        {
            return (this.client.RemoteAddress.ToString());
        }



        public void Send(byte[] data)
        {
            IByteBuffer buffer = Unpooled.Buffer();
            buffer.WriteBytes(data);

            // WebSockt:  Text文本模式丢弃，arraybuffer 二进制模式;
            if (this.isWebSocket) {
                BinaryWebSocketFrame f = new BinaryWebSocketFrame(buffer);
                this.client.WriteAndFlushAsync(f); // eventLoop线程里面
                /*this.client.EventLoop.Execute(()=> {
                    this.client.WriteAsync(f);
                });*/
            }
            else {  
                // TcpSocket的Sender ---> IByteBuffer数据对象，在Pipeline里面处理;
                this.client.WriteAndFlushAsync(buffer); // eventLoop线程里面
            }
        }
    }
}
