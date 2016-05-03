using System;

namespace Common.Caching
{
    public interface ICache
    {
        T Get<T>(string key);
        void Set(string key, object value, DateTimeOffset expirationTime);
        void Remove(string key);
    }
}
