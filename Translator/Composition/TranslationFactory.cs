using System;

namespace Translator
{
    public class TranslationFactory
    {
        private readonly ITranslationProvider _provider;

        public TranslationFactory(ITranslationProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public ITranslator CreateForLanguage(string language)
        {
            return new Translator(_provider, language);
        }
    }
}
