using System.Collections.Generic;
using System.IO;
using UtilsPackages.Common.Json;

namespace Translator
{
    public class TranslationProviderBuilder
    {
        private List<string> _jsonTranslationFiles = new List<string>();
        private string _defaultLanguage;
        private CacheOptions _cacheOptions;
        private string _baseDirectory;

        public TranslationProviderBuilder SetBaseDirectory(string path)
        {
            _baseDirectory = path;
            return this;
        }

        public TranslationProviderBuilder AddJsonFile(string filePath)
        {
            if (!File.Exists(Path.Combine(_baseDirectory, filePath)))
            {
                throw new TranslationsException($"File {filePath} does not exists");
            }

            _jsonTranslationFiles.Add(filePath);
            return this;
        }

        public TranslationProviderBuilder AddJsonFilesFromFolder(string folderPath)
        {
            if (!Directory.Exists(Path.Combine(_baseDirectory, folderPath)))
            {
                throw new TranslationsException($"Folder {folderPath} does not exists");
            }

            var files = Directory.GetFiles(folderPath);
            _jsonTranslationFiles.AddRange(files);
            return this;
        }

        public TranslationProviderBuilder SetDefaultLanguage(string language)
        {
            _defaultLanguage = language;
            return this;
        }

        public TranslationProviderBuilder UseInMemoryCache(CacheOptions options)
        {
            _cacheOptions = options;
            return this;
        }

        public ITranslationProvider Build()
        {
            var jsonReader = new JsonFileReader();
            var reader = new TranslationsJsonFileReader(jsonReader, _jsonTranslationFiles);

            var readers = new List<ITranslationsReader> { reader };
            var options = new TranslationsProviderOptions {
                DefaultLanguage = _defaultLanguage,
                Cache = _cacheOptions,
            };
            return new TranslationProvider(readers, options);
        }
    }
}
