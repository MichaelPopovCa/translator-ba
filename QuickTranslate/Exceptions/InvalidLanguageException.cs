using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class InvalidLanguageException : TranslationException
    {
        public InvalidLanguageException(string message, TranslationErrorCode translationErrorCode)
        : base(message, translationErrorCode)
        {}
    }
}
