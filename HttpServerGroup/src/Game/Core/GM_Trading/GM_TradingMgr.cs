using Game;
using Game.Datas.GMEntities;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.GM_Trading
{
    public class GM_TradingMgr
    {
        public static GM_TradingMgr Instance = new GM_TradingMgr();
        private NLog.Logger logger = null;
        private Dictionary<int, Dictionary<string, MethodInfo>> tradingRuleFuncDic = new Dictionary<int, Dictionary<string, MethodInfo>>();

        public void Init()
        {
            this.logger = NLog.LogManager.GetCurrentClassLogger();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] allTypes = assembly.GetTypes();
                for (int i = 0; i < allTypes.Length; i++)
                {
                    Type t = allTypes[i];
                    TradingRule tradingRule = t.GetCustomAttribute<TradingRule>();
                    if (tradingRule != null)
                    {
                        this.ScaneOneTradingRuleRule(t, tradingRule);
                    }
                }
            }
        }

        private int MainType(int tid)
        {
            return (tid / 1000000) * 1000000;
        }


        private void ScaneOneTradingRuleRule(Type t, TradingRule tradingRule)
        {
            MethodInfo[] funcs = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int j = 0; j < funcs.Length; j++)
            {
                TradingProcesser p = funcs[j].GetCustomAttribute<TradingProcesser>();
                if (p == null)
                {
                    continue;
                }

                int key = tradingRule.mainType + p.subType;
                Dictionary<string, MethodInfo> processFuncs = null;
                if (!this.tradingRuleFuncDic.ContainsKey(key))
                {
                    processFuncs = new Dictionary<string, MethodInfo>();
                    this.tradingRuleFuncDic.Add(key, processFuncs);
                }
                else
                {
                    processFuncs = this.tradingRuleFuncDic[key];
                }

                processFuncs.Add(p.funcName, funcs[j]);
            }
        }

        private MethodInfo GetProcesserFunc(string funcName, Dictionary<string, MethodInfo> funMap, Dictionary<string, MethodInfo> defaultFunMap)
        {
            if (funMap.ContainsKey(funcName))
            {
                return funMap[funcName];
            }
            if (defaultFunMap.ContainsKey(funcName))
            {
                return defaultFunMap[funcName];
            }

            return null;
        }

        private MethodInfo GetProcesserFuncWithTid(string actionName, int tid)
        {
            int mainType = this.MainType(tid);
            int subType = tid % 1000000;


            int key = mainType;
            key = key - 1;

            Dictionary<string, MethodInfo> funMap = null;
            Dictionary<string, MethodInfo> defaultFunMap = null;

            if (this.tradingRuleFuncDic.ContainsKey(key))
            {
                defaultFunMap = this.tradingRuleFuncDic[key];
            }

            if (this.tradingRuleFuncDic.ContainsKey(tid))
            {
                funMap = this.tradingRuleFuncDic[tid];
            }
            else
            {
                funMap = defaultFunMap;
            }
            if (funMap == null)
            {
                return null;
            }

            MethodInfo actionFunc = this.GetProcesserFunc(actionName, funMap, defaultFunMap);
            return actionFunc;
        }

        public int CanExchangeProduct(GM_PlayerEntity gM_PlayerEntity, int productId)
        {
            MethodInfo methodInfo = GetProcesserFuncWithTid("CanExchange", productId);
            if (methodInfo != null)
            {
                object ret = methodInfo.Invoke(null, new object[] { gM_PlayerEntity, productId });
                return (int)ret;
            }
            return (int)Respones.InvalidOpt;
        }

        public void DoExchangeProduct(GM_PlayerEntity gM_PlayerEntity, int productId)
        {
            MethodInfo methodInfo = GetProcesserFuncWithTid("DoExchange", productId);
            if (methodInfo != null)
            {
                bool ret = (bool)methodInfo.Invoke(null, new object[] { gM_PlayerEntity, productId });
                if (!ret)
                {
                    return;
                }
            }
        }

        public long GenerateProductOrder(GM_PlayerEntity gM_PlayerEntity, int productId, int payChannel)
        {
            MethodInfo methodInfo = GetProcesserFuncWithTid("GenerateOrder", productId);
            if (methodInfo != null)
            {
                object ret = methodInfo.Invoke(null, new object[] { gM_PlayerEntity, productId, payChannel });
                return (long)ret;
            }
            return -1;
        }

        public void UserOrderDelivery(GM_PlayerEntity gM_PlayerEntity, int productId, long orderId)
        {
            MethodInfo methodInfo = GetProcesserFuncWithTid("OrderDelivery", productId);
            if (methodInfo != null)
            {
                bool ret = (bool)methodInfo.Invoke(null, new object[] { gM_PlayerEntity, productId, orderId }); 
                if (!ret)
                {
                    return;
                }
            }
        }


    }
}
