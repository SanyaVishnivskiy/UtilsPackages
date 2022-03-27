namespace Translator
{
    public class ValidationFactory
    {
        public ITranslationConfigurationValidator Create()
        {
            return new TranslationConfigurationValidator();
        }
    }
}
