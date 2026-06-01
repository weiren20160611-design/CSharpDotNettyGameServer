using System.Collections.Generic;

namespace Game.LogicServer.RVOMapConfig
{
    public class ItemBase
    {
        public int typeId = 0;
        public string desicName = "";

        public string prefabPath = "";
        public float raduis = 0.0f;
    }

    public class MapItemBase
    {
        public static Dictionary<int, ItemBase> Datas = new Dictionary<int, ItemBase>() {
        { 10001, new ItemBase(){ typeId = 10001, desicName = "an", prefabPath = "Maps/Prefabs/10001"} },
        { 10002, new ItemBase(){ typeId = 10002, desicName = "anS", prefabPath = "Maps/Prefabs/10002"} },

        { 20001, new ItemBase(){ typeId = 20001, desicName = "ÇÅ¶Õ", prefabPath = "Maps/Prefabs/20001"} },
        { 20002, new ItemBase(){ typeId = 20002, desicName = "µ¥¶Õ", prefabPath = "Maps/Prefabs/20002"} },
        { 20003, new ItemBase(){ typeId = 20003, desicName = "¹â¶Õ", prefabPath = "Maps/Prefabs/20003"} },

        { 30001, new ItemBase() { typeId = 30001, desicName = "Î×ÆÅ", prefabPath = "NPC/Prefabs/30001"} },
        { 30002, new ItemBase() { typeId = 30002, desicName = "Î×Ê¦", prefabPath = "NPC/Prefabs/30002"} },


        { 40001, new ItemBase() { typeId = 40001, desicName = "Ö©Öë¾«", prefabPath = "Monster/Prefabs/40001", raduis = 1.2f} },
        { 40002, new ItemBase() { typeId = 40002, desicName = "ÌøÌøÃ¨", prefabPath = "Monster/Prefabs/40002", raduis = 1.0f} },
        { 40003, new ItemBase() { typeId = 40003, desicName = "·ÉÁú", prefabPath = "Monster/Prefabs/40003", raduis = 1.0f} },
        { 40004, new ItemBase() { typeId = 40004, desicName = "º£Íõ", prefabPath = "Monster/Prefabs/40004", raduis = 1.0f} },
        { 40005, new ItemBase() { typeId = 40005, desicName = "Boss", prefabPath = "Monster/Prefabs/40005", raduis = 1.0f} },

        { 50001, new ItemBase() { typeId = 50001, desicName = "Å®Íæ¼Ò", prefabPath = "Role/Prefabs/50001", raduis = 0.5f} },

    };
    }
}