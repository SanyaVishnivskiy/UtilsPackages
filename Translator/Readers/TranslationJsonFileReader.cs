using System;
using System.Collections.Generic;
using System.Linq;
using UtilsPackages.Common.Extensions;
using UtilsPackages.Common.Json;

namespace Translator
{
    internal class TranslationsJsonFileReader : ITranslationsReader
    {
        private readonly Dictionary<string, string> _languageFileMap = new ();
        private readonly IJsonFileReader _reader;

        public TranslationsJsonFileReader(IJsonFileReader reader, List<string> translationFiles)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            CreateLanguageFileMap(translationFiles);
        }

        private void CreateLanguageFileMap(List<string> translationFiles)
        {
            translationFiles.AsParallel().ForAll(file =>
            {
                var config = _reader.Read<TranslationsConfig>(file);

                ValidateConfig(config, file);

                _languageFileMap.Add(config.Language, file);
            });
        }

        private void ValidateConfig(TranslationsConfig config, string file)
        {
            if (string.IsNullOrEmpty(config.Language))
            {
                throw new TranslationsException($"{nameof(TranslationsConfig.Language).ToCamelCase()} does not exist in file {file}");
            }

            if (config.Translations is null)
            {
                throw new TranslationsException($"{nameof(TranslationsConfig.Translations).ToCamelCase()} does not exist in file {file}");
            }
        }

        public List<string> GetLanguages()
        {
            return _languageFileMap.Keys.ToList();
        }

        public TranslationsConfig Read(string language)
        {
            if (!_languageFileMap.ContainsKey(language))
            {
                throw new TranslationsException($"Language {language} is not configured");
            }

            return _reader.Read<TranslationsConfig>(_languageFileMap[language]);
        }
    }

    internal class TranslationsConfig
    {
        public string Language { get; set; }
        public Dictionary<string, string> Translations { get; } = new ();
    }
}
