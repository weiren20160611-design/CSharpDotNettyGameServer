using Game.Core.GM_Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Game.Utils
{
    public class GameUtils
    {
        public static int Random(int min, int max)
        {
            Random random = new Random();
            int s = random.Next(max) % (max - min + 1) + min;
            return s;
        }

        public static List<int> ParseStringWithIntValue(string formatString)
        {
            List<int> props = new List<int>();
            string[] conditions = formatString.Split('|');
            for (int i = 0; i < conditions.Length; i++)
            {
                int value = int.Parse(conditions[i]);
                props.Add(value);
            }
            return props;
        }


        public static Dictionary<string, int> ParseStringWithKeyAndValue(string formatString)
        {
            Dictionary<string, int> props = new Dictionary<string, int>();
            string[] conditions = formatString.Split('|');
            for (int i = 0; i < conditions.Length; i++)
            {
                string[] condition = conditions[i].Split('=');
                string key = condition[0];
                int value = int.Parse(condition[1]);
                props.Add(key, value);
            }
            return props;
        }

        public static byte[] EncodeObjectToBytes<T>(object obj)
        {
            MemoryStream stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<T>(stream, (T)obj);
            return stream.ToArray();
        }

        public static T DecodeBytesToObject<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return ProtoBuf.Serializer.Deserialize<T>(stream);
        }

        public static object GetFiled(object inst, string filedName, object defaultObj)
        {
            Type t = inst.GetType();
            FieldInfo f = t.GetField(filedName);
            if (f == null)
            {
                return defaultObj;
            }

            return f.GetValue(inst);
        }

        public static void SetFiled(object inst, string filedName, object value)
        {
            Type t = inst.GetType();
            FieldInfo f = t.GetField(filedName);
            if (f == null)
            {
                return;
            }

            f.SetValue(inst, value);
        }

        public static object GetProperty(object inst, string propertyName, object defaultObj)
        {
            Type t = inst.GetType();
            PropertyInfo p = t.GetProperty(propertyName);
            if (p == null)
            {
                return defaultObj;
            }

            return p.GetValue(inst);
        }

        public static void SetProperty(object inst, string propertyName, object value)
        {
            Type t = inst.GetType();
            PropertyInfo p = t.GetProperty(propertyName);
            if (p == null)
            {
                return;
            }

            p.SetValue(inst, value);
        }
    }

}
