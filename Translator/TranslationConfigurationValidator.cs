using System.Collections.Generic;
using System.Linq;
using UtilsPackages.Common.Extensions;

namespace Translator
{
    public interface ITranslationConfigurationValidator
    {
        void Validate(ITranslationProvider provider);
    }

    internal class TranslationConfigurationValidator : ITranslationConfigurationValidator
    {
        public void Validate(ITranslationProvider provider)
        {
            var supportedLanguages = provider.GetSupportedLanguages();
            var keysByLanguage = GetLanguagesByTranlationKeys(provider, supportedLanguages);

            foreach (var key in keysByLanguage)
            {
                var existsForLanguages = key.Value.Distinct().ToList();
                EnsureNoMissedTranslations(existsForLanguages, supportedLanguages, key);
                EnsureNoDuplicatedTranslations(existsForLanguages, key);
            }
        }

        private Dictionary<string, List<string>> GetLanguagesByTranlationKeys(
            ITranslationProvider provider,
            List<string> supportedLanguages)
        {
            var keys = new Dictionary<string, List<string>>();
            foreach (var language in supportedLanguages)
            {
                foreach (var translation in provider.ReadTranslations(language))
                {
                    if (!keys.ContainsKey(translation.Key))
                    {
                        keys.Add(translation.Key, new List<string> { language });
                    }
                    else
                    {
                        keys[translation.Key].Add(language);
                    }
                }
            }

            return keys;
        }

        private void EnsureNoMissedTranslations(
            List<string> existsForLanguages,
            List<string> supportedLanguages,
            KeyValuePair<string, List<string>> key)
        {
            if (existsForLanguages.Count < supportedLanguages.Count)
            {
                var missedLanguages = supportedLanguages.Except(existsForLanguages);
                throw new TranslationsException($"Translation with key {key.Key} " +
                    $"does not exist in languages {string.Join(", ", missedLanguages)}");
            }
        }

        private void EnsureNoDuplicatedTranslations(
            List<string> existsForLanguages,
            KeyValuePair<string, List<string>> key)
        {
            if (existsForLanguages.Count < key.Value.Count)
            {
                var duplicatedLanguages = key.Value.FindDuplicated();
                var stringifiedDuplicationInfo = duplicatedLanguages.Select(x => $"{x.Count} times in language {x.Item}");
                throw new TranslationsException($"Translation with key {key.Key} " +
                    $"appears {string.Join(", ", stringifiedDuplicationInfo)}");
            }
        }
    }
}
