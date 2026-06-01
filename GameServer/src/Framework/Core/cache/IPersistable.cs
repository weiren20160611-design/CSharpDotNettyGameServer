using Game.Datas.DBEntities;
using Game.Core.Db;

namespace Framework.Core.Cache
{
    public interface IPersistable<K, V>
    {
        public V Load(K k);
    }

}
