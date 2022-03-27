using System.Collections.Generic;

namespace Translator
{
    internal interface ITranslationsReader
    {
        List<string> GetLanguages();
        TranslationsConfig Read(string language);
    }
}
