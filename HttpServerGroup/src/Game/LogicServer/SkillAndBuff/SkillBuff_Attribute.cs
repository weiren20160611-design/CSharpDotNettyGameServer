
using System;

namespace Game.LogicServer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SkillModel : Attribute
    {
        public int mainType; // 1000000

        public SkillModel(int mainType)
        {
            this.mainType = mainType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SkillProcesser : Attribute
    {
        public int subType; // -1为当前类技能默认的处理
        public string funcName;
        public SkillProcesser(string name, int subType)
        {
            this.subType = subType;
            this.funcName = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SkillConfig : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BuffModel : Attribute
    {
        public int mainType;

        public BuffModel(int mainType)
        {
            this.mainType = mainType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BuffProcesser : Attribute
    {
        public int subType; // -1为当前类buf默认的处理
        public string propName; // 要多不同的属性，进行buff计算，

        public BuffProcesser(string propName, int subType)
        {
            this.subType = subType;
            this.propName = propName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BuffConfig : Attribute { }

}
