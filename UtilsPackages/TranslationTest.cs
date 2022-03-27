using System;
using System.IO;
using Translator;

namespace UtilsPackages
{
    public class TranslationTest
    {
        public void Main()
        {
            var provider = new TranslationProviderBuilder()
                .SetBaseDirectory(Directory.GetCurrentDirectory())
                .AddJsonFile("languages/en.json")
                .AddJsonFile("languages/ua.json")
                .SetDefaultLanguage("en")
                .UseInMemoryCache(CacheOptions.ReadAllToCache())
                .Build();

            var validator = new ValidationFactory().Create();
            validator.Validate(provider);

            var factory = new TranslationFactory(provider);

            var uaTranslator = factory.CreateForLanguage("ua");
            var enTranslator = factory.CreateForLanguage("en");
            var unknownTranslator = factory.CreateForLanguage("unknown");

            PrintTranslations(uaTranslator);
            PrintTranslations(enTranslator);
            PrintTranslations(unknownTranslator);
        }

        private static void PrintTranslations(ITranslator translator)
        {
            Console.WriteLine($"Language {translator.Language}");
            Console.WriteLine(Translate(translator, "hello text"));
            Console.WriteLine(Translate(translator, "world text"));
            Console.WriteLine(Translate(translator, "another text"));
            Console.WriteLine(Translate(translator, "Unknown text"));
        }

        private static string Translate(ITranslator translator, string text)
        {
            try
            {
                return translator.Translate(text);
            }
            catch (Exception e)
            {
                return $"Translation failed: {e.Message}";
            }
        }
    }
}
