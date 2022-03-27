using System;

namespace Translator
{
    public interface ITranslator
    {
        string Language { get; }
        string Translate(string key);
        string TranslateTo(string language, string key);
    }

    internal class Translator : ITranslator
    {
        private readonly ITranslationProvider _provider;

        public string Language { get; }

        public Translator(ITranslationProvider provider, string language)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Language = language ?? throw new ArgumentNullException(nameof(language));
        }

        public string Translate(string key)
        {
            return TranslateTo(Language, key);
        }

        public string TranslateTo(string language, string key)
        {
            return _provider.Translate(language, key);
        }
    }
}
