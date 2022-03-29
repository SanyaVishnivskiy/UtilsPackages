namespace UtilsPackages.Common.Cache
{
    public interface ICache<TKey>
    {
        T Get<T>(TKey key);
        bool TryGetValue<T>(TKey key, out T value);
        bool Contains(TKey key);
        void SetIfNotExist<T>(TKey key, T value);
        void SetIfNotExist<T>(TKey key, T value, TimeSpan expirationTime);
        void Set<T>(TKey key, T value);
        void Set<T>(TKey key, T value, TimeSpan expirationTime);
        void Remove(TKey key);
    }
}
