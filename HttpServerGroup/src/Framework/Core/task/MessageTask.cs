using Framework.Core.Net;
using Framework.Core.Serializer;
using System;
using System.Reflection;
using System.Text;

namespace Framework.Core.task {
    public class MessageTask : AbstractDistributeTask
    {
        /** io session context */
        private IdSession session;
        /** message controller */
        private object handler;
        /** target method of the controller */
        private MethodInfo method;
        /**arguments passed to the method */
        private object[] @params;



        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 创建一个消息任务
        /// </summary>
        /// <param name="uniqueSessionId">客户端的会话key</param>
        /// <param name="handler"></param>
        /// <param name="method"></param>
        /// <param name="params"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static MessageTask Create(long taskId, object handler, MethodInfo method, object[] @params, IdSession s) {
            MessageTask t = new MessageTask();
            t.taskId = taskId;

            t.handler = handler;
            t.method = method;
            t.@params = @params;
            t.session = s;

            return t;
        }

        public override void DoAction() {
            try
            {
                object response = method.Invoke(handler, @params);
                if (response != null) {
                    MessagePusher.PushMessage(this.session, (Message)response);
                }
            }
            catch (Exception e)
            {
                logger.Warn("message task execute failed" + e.Message);
            }
        }
    }
}
