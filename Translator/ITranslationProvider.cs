using System.Collections.Generic;

namespace Translator
{
    public interface ITranslationProvider
    {
        List<string> GetSupportedLanguages();
        IEnumerable<KeyValuePair<string, string>> ReadTranslations(string language);
        string Translate(string language, string key);
    }
}
