using Microsoft.Extensions.Caching.Memory;

namespace UtilsPackages.Common.Cache
{
    public class InMemoryCache<TKey> : ICache<TKey>
    {
        private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public bool Contains(TKey key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public T Get<T>(TKey key)
        {
            return (T)_cache.Get(key);
        }

        public bool TryGetValue<T>(TKey key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Remove(TKey key)
        {
            if (key == null)
            {
                return;
            }

            _cache.Remove(key);
        }

        public void Set<T>(TKey key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _cache.Set(key, value);
        }

        public void Set<T>(TKey key, T value, TimeSpan expirationTime)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _cache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime
            });
        }

        public void SetIfNotExist<T>(TKey key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!Contains(key))
            {
                Set(key, value);
            }
        }

        public void SetIfNotExist<T>(TKey key, T value, TimeSpan expirationTime)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!Contains(key))
            {
                Set(key, value, expirationTime);
            }
        }
    }
}
