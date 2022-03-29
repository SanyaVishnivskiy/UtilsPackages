using System.Collections.Generic;

namespace Translator
{
    internal class TranslationsConfig
    {
        public string Language { get; init; }
        public Dictionary<string, string> Translations { get; init; } = new ();
    }
}
