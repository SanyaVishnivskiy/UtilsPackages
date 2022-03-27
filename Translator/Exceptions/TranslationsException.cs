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
}
