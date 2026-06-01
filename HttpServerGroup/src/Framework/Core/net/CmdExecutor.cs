using System;
using System.Reflection;

namespace Framework.Core.Net {
    /// <summary>
    /// 存储执行方法的信息（命令执行器）
    /// </summary>
    public class CmdExecutor {
        // 方法所在的实例对象
        public object handler;

        // 方法
        public MethodInfo method;

        // 方法的参数类型
        public Type[] @params;


        public static CmdExecutor Create(MethodInfo method, Type[] @params, object handler) {
            CmdExecutor executor = new CmdExecutor();
            executor.method = method;
            executor.@params = @params;
            executor.handler = handler;

            return executor;
        }
    }

}

