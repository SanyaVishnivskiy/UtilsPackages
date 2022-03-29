using System;

namespace Translator
{
    public class TranslationsException : Exception
    {
        public TranslationsException(string message) : base(message)
        {
        }

        public TranslationsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class LanguageNotConfiguredException : TranslationsException
    {
        public LanguageNotConfiguredException(string language) : this(language, null)
        {
        }

        public LanguageNotConfiguredException(string language, Exception innerException)
            : base($"Language {language} is not configured", innerException)
        {
        }
    }
}
