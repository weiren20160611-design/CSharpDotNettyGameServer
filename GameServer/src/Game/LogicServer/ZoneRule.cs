
using Game.Datas.DBEntities;
using Game.Datas.GMEntities;
using System;
using System.Reflection;

namespace Game.LogicServer
{
    public class ZoneRule
    {

        // 可以使用特定的反射来做;
        public static int ParseCondLE(GM_PlayerEntity playerEntity, string lhs, string rhs)
        {
            int status = (int)Respones.InvalidOpt;

            return status;
        }

        public static int ParseCondEqual(GM_PlayerEntity playerEntity, string lhs, string rhs)
        {
            return (int)Respones.OK;
        }

        public static int ParseCondGE(GM_PlayerEntity playerEntity, string lhs, string rhs)
        {
            int status = (int)Respones.InvalidOpt;
            int value = int.Parse(rhs);
            Player playerInfo = playerEntity.uPlayer.playerInfo;
            Type type = playerInfo.GetType();
            PropertyInfo propertyInfo = type.GetProperty(lhs);
            if (propertyInfo == null)
            {
                return (int)Respones.InvalidParams;
            }

            if ((int)propertyInfo.GetValue(playerInfo) >= value)
            {
                return (int)Respones.OK;
            }

            status = (int)Respones.ConditionNotAccord;
            return status;
        }



        public static int PlayerCanEnterZone(string enterCondStr, GM_PlayerEntity playerEntity)
        {
            if (enterCondStr == null || enterCondStr.Length <= 0 ||
                enterCondStr.Equals("none") ||
                enterCondStr.Equals("null"))
            {
                return (int)Respones.OK;
            }
            // 
            string[] conds = enterCondStr.Split('|');
            if (conds.Length <= 0)
            {
                return (int)Respones.OK;
            }
            // end

            int status = (int)Respones.InvalidOpt;
            for (int i = 0; i < conds.Length; i++)
            {
                string cond = conds[i];
                int index = cond.IndexOf(">=");
                if (index > 0)
                {
                    status = ParseCondGE(playerEntity, cond.Substring(0, index), cond.Substring(index + 2));
                    if (status != (int)Respones.OK)
                    {
                        return status;
                    }
                    continue;
                }

                index = cond.IndexOf("==");
                if (index > 0)
                {
                    status = ParseCondEqual(playerEntity, cond.Substring(0, index), cond.Substring(index + 2));
                    if (status != (int)Respones.OK)
                    {
                        return status;
                    }
                    continue;
                }

                index = cond.IndexOf("<=");
                if (index > 0)
                {
                    status = ParseCondLE(playerEntity, cond.Substring(0, index), cond.Substring(index + 2));
                    if (status != (int)Respones.OK)
                    {
                        return status;
                    }
                    continue;
                }
            }

            return (int)status;
        }
    }
}
