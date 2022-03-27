using System;

namespace Translator
{
    public class CacheOptions
    {
        public CacheType Type { get; init; }
        public TimeSpan ExpireTime { get; init; } = TimeSpan.MinValue;

        public static CacheOptions NoCache()
        {
            return new CacheOptions {
                Type = CacheType.NoCache
            };
        }

        public static CacheOptions UseInMemoryCache(TimeSpan expireTime)
        {
            return new CacheOptions
            {
                Type = CacheType.UseInMemoryCache,
                ExpireTime = expireTime
            };
        }

        public static CacheOptions ReadAllToCache()
        {
            return new CacheOptions
            {
                Type = CacheType.ReadAllToCache
            };
        }
    }
}
