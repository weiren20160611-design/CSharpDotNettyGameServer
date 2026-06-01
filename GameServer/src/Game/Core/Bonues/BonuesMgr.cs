using Framework.Core.Utils;
using Game.Datas.DBEntities;
using Game.Core.Db;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Core.GameBonues
{
    public class BonuesMgr
    {
        public static BonuesMgr Instance = new BonuesMgr();
        private Dictionary<int, MethodInfo> rulesFunc = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> calcFunc = new Dictionary<int, MethodInfo>();
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void Init()
        {
            // 初始化
            IEnumerable<Type> bonuesRules = TypeScanner.ListTypesWithAttribute(typeof(BonuesClass));
            foreach (Type rule in bonuesRules)
            {
                try
                {
                    BonuesClass ruleAttribute = rule.GetCustomAttribute<BonuesClass>();
                    MethodInfo[] methods = rule.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                    foreach (MethodInfo method in methods)
                    {
                        BonuesApply mapperAttribute = method.GetCustomAttribute<BonuesApply>();
                        if (mapperAttribute != null)
                        {
                            int key = ruleAttribute.mainType + mapperAttribute.subType;

                            if (rulesFunc.ContainsKey(key))
                            {
                                logger.Error("重复注册BonuesRule:" + key);
                                continue;
                            }
                            rulesFunc.Add(key, method);
                        }

                        BonuesCalculate bonuesCalculate = method.GetCustomAttribute<BonuesCalculate>();
                        if (bonuesCalculate != null)
                        {
                            int key = ruleAttribute.mainType + bonuesCalculate.subType;

                            if (calcFunc.ContainsKey(key))
                            {
                                logger.Error("重复注册BonuesCalculate:" + key);
                                continue;
                            }
                            calcFunc.Add(key, method);
                        }


                    }
                }
                catch
                {

                }

            }
        }


        public bool GennerateBonues(long playerId, int typeId, int duration, string desic, int b1 = 0, int b2 = 0, int b3 = 0, int b4 = 0, int b5 = 0)
        {
            Bonues bonues = new Bonues();
            bonues.uid = playerId;
            bonues.tid = typeId;
            bonues.time = (int)UtilsHelper.Timestamp();
            bonues.endTime = duration <= 0 ? -1 : (int)UtilsHelper.Timestamp() + duration;
            bonues.bonuesDesic = desic;
            bonues.bonues1 = b1;
            bonues.bonues2 = b2;
            bonues.bonues3 = b3;
            bonues.bonues4 = b4;
            bonues.bonues5 = b5;
            DBService.Instance.GetGameInstance().Insertable(bonues).ExecuteCommandAsync();
            return true;
        }

        /// <summary>
        /// 将奖励数据应用到玩家
        /// </summary>
        /// <param name="bonues"></param>
        /// <returns></returns>
        public bool ApplyBonuesToPlayer(Bonues bonues)
        {
            if (bonues == null || bonues.status != 0)
            {
                return false;
            }
            MethodInfo method = null;
            if (rulesFunc.ContainsKey((int)bonues.tid))
            {
                method = rulesFunc[(int)bonues.tid];
            }
            else
            {
                int mainKey = ((int)bonues.tid / 100000) * 100000;
                int defaultKey = mainKey - 1;
                if (rulesFunc.ContainsKey(defaultKey))
                {
                    method = rulesFunc[defaultKey];
                }
            }
            if (method == null)
            {
                logger.Error("没有找到对应的bonuesRule:" + bonues.tid);
            }
            method.Invoke(null, new object[] { bonues });
            return true;
        }

        public bool RecvBonues(long bonuesId, Bonues bonues = null)
        {
            if (bonues == null)
            {
                bonues = DBService.Instance.GetGameInstance().Queryable<Bonues>().First(it => it.id == bonuesId);
            }

            if (!ApplyBonuesToPlayer(bonues))
            {
                return false;
            }
            ;
            bonues.status = 1;
            DBService.Instance.GetGameInstance().Updateable(bonues).Where(it => it.id == bonuesId).ExecuteCommandAsync();
            return true;
        }

        public Bonues GennerateAndRecvive(long playerId, int typeId, int duration, string desic, int b1 = 0, int b2 = 0, int b3 = 0, int b4 = 0, int b5 = 0)
        {
            Bonues bonues = new Bonues();
            bonues.uid = playerId;
            bonues.tid = typeId;
            bonues.time = (int)UtilsHelper.Timestamp();
            bonues.endTime = duration <= 0 ? -1 : (int)UtilsHelper.Timestamp() + duration;
            bonues.bonuesDesic = desic;
            bonues.bonues1 = b1;
            bonues.bonues2 = b2;
            bonues.bonues3 = b3;
            bonues.bonues4 = b4;
            bonues.bonues5 = b5;
            bool ret = ApplyBonuesToPlayer(bonues);
            if (!ret)
            {
                return null;
            }
            bonues.status = 1;
            DBService.Instance.GetGameInstance().Insertable(bonues).ExecuteCommandAsync();
            return bonues;
        }


        /// <summary>
        /// 根据不同奖励的规则计算奖励
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="typeId"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Bonues CalcBonuesToTarget(long playerId, int typeId, object param = null)
        {
            MethodInfo method = null;
            if (calcFunc.ContainsKey(typeId))
            {
                method = calcFunc[typeId];
            }
            else
            {
                int mainKey = (typeId / 100000) * 100000;
                int defaultKey = mainKey - 1;
                if (calcFunc.ContainsKey(defaultKey))
                {
                    method = calcFunc[defaultKey];
                }
            }
            if (method == null)
            {
                logger.Error("没有找到对应的bonuesRule:" + typeId);
                return null;
            }
            Bonues bonues = (Bonues)method.Invoke(null, new object[] { playerId, typeId, param });
            return bonues;
        }

        /// <summary>
        /// 生成奖励并绑定玩家
        /// </summary>
        /// <param name="playerId">绑定的玩家ID</param>
        /// <param name="typeId">领取奖励的类型</param>
        /// <param name="param"></param>
        public void GenBonuesToTarget(long playerId, int typeId, object param = null)
        {
            Bonues bonues = CalcBonuesToTarget(playerId, typeId, param);
            if (bonues == null)
            {
                return;
            }
            DBService.Instance.GetGameInstance().Insertable(bonues).ExecuteCommandAsync();
        }

        /// <summary>
        /// 生成奖励并领取
        /// </summary>
        /// <param name="playerId">玩家ID</param>
        /// <param name="typeId">领取奖励的类型</param>
        /// <param name="param"></param>
        public void GenAndRecvBonuesToTarget(long playerId, int typeId, object param = null)
        {
            Bonues bonues = CalcBonuesToTarget(playerId, typeId, param);
            if (bonues == null)
            {
                return;
            }
            ApplyBonuesToPlayer(bonues);
            bonues.status = 1;
            DBService.Instance.GetGameInstance().Insertable(bonues).ExecuteCommandAsync();
        }

        public Bonues[] PullingBonuesData(long playerId, int typeId)
        {
            return DBService.Instance.GetGameInstance().Queryable<Bonues>().Where(it => it.uid == playerId).ToArray();
        }

        public Bonues GetBonuesByID(long bonuesId)
        {
            Bonues bonues = DBService.Instance.GetGameInstance().Queryable<Bonues>().First(it => it.id == bonuesId);
            return bonues;
        }
    }
}
