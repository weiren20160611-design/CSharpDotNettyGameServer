namespace Framework.Core.Cache {
    /// <summary>
    /// 缓存协议服务
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class BaseCacheSerivce<K, V> : IPersistable<K, V>
    {
        AbstractCacheContainer<K, V> container = null;

        public BaseCacheSerivce() {
            this.container = new DefaultCacheContainer<K, V>(this);
        }

        public abstract V Load(K k);

        public V Get(K key)
        {
            return this.container.Get(key);
        }

        public V GetIfPresent(K k)
        {
            return this.container.GetIfPresent(k);
        }

        public void Put(K key, V v)
        {
            this.container.Put(key, v);
        }

        public void Remove(K k)
        {
            this.container.Remove(k);
        }

    }
}
