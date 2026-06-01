using System;
using System.Runtime.Caching;

namespace Framework.Core.Cache
{
    /// <summary>
    /// 缓存容器抽象类
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class AbstractCacheContainer<K, V>
    {
        /// 加载缓存对象的类
        private LoadingCache<K, V> cache = null;

        public AbstractCacheContainer()
        {
            //创建一个缓存并进行设置
            CacheBuilder<K, V> builder = CacheBuilder<K, V>.NewBuilder();
            builder.SetExpireAfterAccessMilliseconds(18000000);
            builder.SetExpireAfterWriteMilliseconds(18000000);
            builder.RemovalListener(this.onRemoval);
            //
            this.cache = builder.Build(DataLoader);

        }

        public virtual V Get(K k)
        {
            try
            {
                return cache.Get(k);
            }
            catch (Exception)
            {
                return default(V);
            }
        }

        public V GetIfPresent(K k)
        {
            try
            {
                return cache.GetIfPresent(k);
            }
            catch (Exception)
            {
                return default(V);
            }
        }

        public void Put(K k, V v)
        {
            cache.Set(k, v);
        }

        public void Remove(K k)
        {
            cache.Invalidate(k);
        }


        protected V DataLoader(K key)
        {
            return LoadFromDb(key);
        }

        protected abstract V LoadFromDb(K key);

        public virtual void onRemoval(CacheEntryRemovedArguments arguments)
        {

        }
    }

}
