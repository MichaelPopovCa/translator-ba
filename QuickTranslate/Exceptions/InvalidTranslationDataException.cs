using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class InvalidTranslationDataException : TranslationException
    {
        public InvalidTranslationDataException(string message, TranslationErrorCode translationErrorCode)
        : base(message, translationErrorCode)
        {}
    }
}
