using System;


namespace Game
{
    //100000----一类
    //200000----二类
    //依此类推
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BonuesClass : Attribute
    {
        public int mainType;
        public BonuesClass(int mainType)
        {
            this.mainType = mainType;
        }
    }

    //100001----一类的第一种
    //100002----一类的第二种
    //200001----二类的第一种
    //200002----二类的第二种
    //依此类推
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BonuesCalculate : Attribute
    {
        public int subType;
        public BonuesCalculate(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BonuesApply : Attribute
    {
        public int subType;
        public BonuesApply(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TaskClass : Attribute
    {
        public int mainType;
        public TaskClass(int mainType)
        {
            this.mainType = mainType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaskDataEncode : Attribute
    {
        public int subType;
        public TaskDataEncode(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaskDataDecode : Attribute
    {
        public int subType;
        public TaskDataDecode(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaskIsStarted : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaskIsCompleted : Attribute
    {
        public int subType;
        public TaskIsCompleted(int subType)
        {
            this.subType = subType;
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TaskDoCompleteAction : Attribute
    {
        public int subType;
        public TaskDoCompleteAction(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UpdateTaskProgress : Attribute
    {
        public int subType;
        public UpdateTaskProgress(int subType)
        {
            this.subType = subType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RuleModule : Attribute
    {
        public int module;
        public int baseId;
        public RuleModule(int module, int baseId)
        {
            this.module = module;
            this.baseId = baseId;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RuleProcesser : Attribute
    {
        public string funcName;
        public int subType;
        public RuleProcesser(string funcName, int subType)
        {
            this.subType = subType;
            this.funcName = funcName;
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GoodsRule : Attribute
    {
        public int mainType;
        public GoodsRule(int mainType)
        {
            this.mainType = mainType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class GoodsProcesser : Attribute
    {
        public int subType;
        public string funcName;
        public GoodsProcesser(string name, int subType)
        {
            this.subType = subType;
            this.funcName = name;
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TradingRule : Attribute
    {
        public int mainType;
        public TradingRule(int mainType)
        {
            this.mainType = mainType;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TradingProcesser : Attribute
    {
        public int subType;
        public string funcName;
        public TradingProcesser(string name, int subType)
        {
            this.subType = subType;
            this.funcName = name;
        }
    }

}
