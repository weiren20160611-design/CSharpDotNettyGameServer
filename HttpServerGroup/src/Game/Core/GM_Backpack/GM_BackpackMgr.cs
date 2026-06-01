using Game.Core.Db;
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using Game.Datas.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.GM_Backpack
{
    public class GM_BackpackMgr
    {
        public static GM_BackpackMgr Instance = new GM_BackpackMgr();

        private NLog.Logger logger = null;

        private Dictionary<int, Dictionary<string, MethodInfo>> goodsRuleSet = new Dictionary<int, Dictionary<string, MethodInfo>>();

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
                    GoodsRule goodsRule = t.GetCustomAttribute<GoodsRule>();
                    if (goodsRule != null)
                    {
                        this.ScaneOneGoodsRule(t, goodsRule);
                    }
                }
            }
        }

        private void ScaneOneGoodsRule(Type t, GoodsRule goodsRule)
        {
            MethodInfo[] funcs = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int j = 0; j < funcs.Length; j++)
            {
                GoodsProcesser p = funcs[j].GetCustomAttribute<GoodsProcesser>();
                if (p == null)
                {
                    continue;
                }

                int key = goodsRule.mainType + p.subType;
                Dictionary<string, MethodInfo> processFuncs = null;
                if (!this.goodsRuleSet.ContainsKey(key))
                {
                    processFuncs = new Dictionary<string, MethodInfo>();
                    this.goodsRuleSet.Add(key, processFuncs);
                }
                else
                {
                    processFuncs = this.goodsRuleSet[key];
                }

                processFuncs.Add(p.funcName, funcs[j]);
            }
        }

        private int MainType(int tid)
        {
            return (tid / 10000) * 10000;
        }

        public Dictionary<int, List<GoodsItem>> GetBackpackData(ref BackpackComponent backpack)
        {
            // 药水类: 30000  ===> [物品1， 物品2， 物品3]
            var ret = new Dictionary<int, List<GoodsItem>>();
            foreach (var k in backpack.packItems.Keys)
            {
                List<PackItem> items = backpack.packItems[k];
                for (int i = 0; i < items.Count; i++)
                {
                    PackItem item = items[i];

                    GoodsItem goods = new GoodsItem();
                    goods.num = (int)item.goods.num;
                    goods.typeId = (int)item.goods.tid;
                    goods.strengData = item.goods.strengData;

                    int mainType = this.MainType((int)item.goods.tid);
                    List<GoodsItem> goodsSet = null;
                    if (ret.ContainsKey(mainType))
                    {
                        goodsSet = ret[mainType];
                    }
                    else
                    {
                        goodsSet = new List<GoodsItem>();
                        ret.Add(mainType, goodsSet);
                    }
                    goodsSet.Add(goods);
                }
            }
            return ret;
        }

        private bool AddGoodsWithTid(ref BackpackComponent backpack, int tid, int num, long playerId)
        {
            if (backpack.packItems.ContainsKey(tid))
            {
                // 
                List<PackItem> items = backpack.packItems[tid];
                if (items.Count > 1)
                {
                    this.logger.Error($"tid:{tid} has multi PackItem");
                    return false;
                }
                // end

                items[0].goods.num += num; // 线程安全

                // 更新到数据库
                DBService.Instance.GetGameInstance().Updateable<Gamegoods>(items[0].goods).Where(it => it.id == items[0].goods.id).ExecuteCommandAsync();
                // end
            }
            else
            { // tid不存在;
                List<PackItem> items = new List<PackItem>();

                backpack.packItems.Add(tid, items);
                PackItem item = new PackItem();
                item.goods = new Gamegoods();
                item.goods.tid = tid;
                item.goods.num = num;
                item.goods.uid = playerId;
                item.strengData = null;
                items.Add(item);

                // 更新到数据库
                item.goods.id = DBService.Instance.GetGameInstance().Insertable<Gamegoods>(item.goods).ExecuteReturnIdentity();
                // end
            }
            return true;
        }

        private bool DecGoodsWithTid(ref BackpackComponent backpack, int tid, int num, long playerId)
        {
            if (backpack.packItems.ContainsKey(tid))
            {
                // 
                List<PackItem> items = backpack.packItems[tid];
                if (items.Count > 1)
                {
                    this.logger.Error($"tid:{tid} has multi PackItem");
                    return false;
                }
                // end

                items[0].goods.num -= num; // 线程安全

                // 更新到数据库
                DBService.Instance.GetGameInstance().Updateable<Game.Datas.DBEntities.Gamegoods>(items[0].goods).Where(it => it.id == items[0].goods.id).ExecuteCommandAsync();
                // end
            }

            return false;
        }

        public bool UpdateGoodsWithTid(GM_PlayerEntity entity, int tid, int num)
        {
            // 验证tid的合法性
            // 验证我们num的合法性;
            if (num > 0)
            { // 增加物品;
                return this.AddGoodsWithTid(ref entity.uPack, tid, num, entity.uPlayer.playerInfo.id);
            }
            else
            { // 消耗物品 
                return DecGoodsWithTid(ref entity.uPack, tid, -num, entity.uPlayer.playerInfo.id);
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
            int subType = tid % 10000;


            int key = mainType;
            key = key - 1;

            Dictionary<string, MethodInfo> funMap = null;
            Dictionary<string, MethodInfo> defaultFunMap = null;

            if (this.goodsRuleSet.ContainsKey(key))
            {
                defaultFunMap = this.goodsRuleSet[key];
            }

            if (this.goodsRuleSet.ContainsKey(tid))
            {
                funMap = this.goodsRuleSet[tid];
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

        public bool DoGoodsActionWithTid(GM_PlayerEntity entity, int tid, string actionName, object param)
        {
            if (entity.uPack.packItems.ContainsKey(tid))
            {
                // 
                List<PackItem> items = entity.uPack.packItems[tid];
                if (items.Count > 1)
                {
                    this.logger.Error($"tid:{tid} has multi PackItem");
                    return false;
                }
                // end

                MethodInfo m = this.GetProcesserFuncWithTid(actionName, tid);
                if (m == null)
                {
                    this.logger.Error($"tid:{tid} has action: {actionName} func is null");
                    return false;
                }

                return (bool)(m.Invoke(null, new object[] { entity, items[0], param }));

            }

            return false;
        }
    }
}
