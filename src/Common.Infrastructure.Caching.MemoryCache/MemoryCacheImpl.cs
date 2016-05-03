using Common.Caching;
using System;
using MemCache = System.Runtime.Caching.MemoryCache;

namespace Common.Infrastructure.Caching.MemoryCache
{
    public class MemoryCacheImpl : ICache
    {
        private readonly MemCache _cache;

        public MemoryCacheImpl(MemCache memoryCache)
        {
            _cache = memoryCache;
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Set(string key, object value, DateTimeOffset expirationTime)
        {
            _cache.Set(key, value, expirationTime);
        }
    }
}
