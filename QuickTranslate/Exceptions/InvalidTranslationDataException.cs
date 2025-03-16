using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class InvalidTranslationDataException : Exception
    {
        public TranslationErrorCode TranslationErrorCode { get; }

        public InvalidTranslationDataException(string message, TranslationErrorCode translationErrorCode)
        : base(message)
        {
            TranslationErrorCode = translationErrorCode;
        }
    }
}
