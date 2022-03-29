using System;
using System.Collections.Generic;
using System.Linq;
using UtilsPackages.Common.Cache;

namespace Translator
{
    internal class TranslationProvider : ITranslationProvider
    {
        private readonly List<ITranslationsReader> _readers;
        private readonly TranslationsProviderOptions _options;

        private readonly ICache<TranslationKey> _cache;

        public TranslationProvider(
            List<ITranslationsReader> readers,
            TranslationsProviderOptions options,
            ICache<TranslationKey> cache)
        {
            _readers = readers ?? new List<ITranslationsReader>();
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            ReadTranslationsIfStoreInMemory();
        }

        private void ReadTranslationsIfStoreInMemory()
        {
            if (_options.Cache.Type != CacheType.ReadAllToCache)
            {
                return;
            }

            var languages = GetSupportedLanguages();
            foreach (var language in languages)
            {
                foreach (var translation in ReadTranslations(language))
                {
                    var cacheKey = new TranslationKey(language, translation.Key);
                    _cache.SetIfNotExist(cacheKey, translation.Value);
                }
            }
        }

        public List<string> GetSupportedLanguages() => _readers
            .SelectMany(x => x.GetLanguages())
            .Distinct()
            .ToList();

        public string Translate(string language, string key)
        {
            var translation = TranslateTo(language, key);
            if (translation is not null)
            {
                return translation;
            }

            if (string.IsNullOrEmpty(_options.DefaultLanguage) || language == _options.DefaultLanguage)
            {
                return null;
            }

            return TranslateTo(_options.DefaultLanguage, key);
        }

        private string TranslateTo(string language, string key)
        {
            var translationKey = new TranslationKey(language, key);
            if (TrySearchInCache(translationKey, out var cachedTranslation))
            {
                return cachedTranslation;
            }

            if (_options.Cache.Type == CacheType.ReadAllToCache)
            {
                if (!GetSupportedLanguages().Any(x => x == language))
                {
                    throw new TranslationsException($"Language {language} is not configured");
                }

                return null;
            }

            var translation = ReadTranslation(language, key);
            if (translation is null)
            {
                return null;
            }

            if (_options.Cache.Type == CacheType.UseInMemoryCache)
            {
                _cache.Set(translationKey, translation, _options.Cache.ExpireTime);
            }

            return key;
        }

        private bool TrySearchInCache(TranslationKey translationKey, out string translation)
        {
            return _cache.TryGetValue(translationKey, out translation);
        }

        public IEnumerable<KeyValuePair<string, string>> ReadTranslations(string language)
        {
            var deduplicationCache = new HashSet<string>();
            var knownLanguage = false;
            foreach (var reader in _readers)
            {
                var config = ReadSafely(reader, language);
                if (config is not null)
                {
                    knownLanguage = true;
                }

                foreach (var translation in config?.Translations ?? new Dictionary<string, string>())
                {
                    if (!deduplicationCache.Contains(translation.Key))
                    {
                        deduplicationCache.Add(translation.Key);
                        yield return translation;
                    }
                }
            }

            if (!knownLanguage)
            {
                throw new TranslationsException($"Language {language} is not configured");
            }
        }

        private TranslationsConfig ReadSafely(ITranslationsReader reader, string language)
        {
            try
            {
                return reader.Read(language);
            }
            catch (TranslationsException)
            {
                return null;
            }
        }

        private string ReadTranslation(string language, string key)
        {
            foreach (var translation in ReadTranslations(language))
            {
                if (translation.Key == key)
                {
                    return translation.Value;
                }
            }

            return null;
        }
    }

    internal class TranslationsProviderOptions
    {
        public CacheOptions Cache { get; init; }
        public string DefaultLanguage { get; init; }
    }

    internal record TranslationKey(string Language, string Key);
}
