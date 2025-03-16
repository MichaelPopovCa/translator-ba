using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class NoDataException : Exception
    {
        public TranslationErrorCode TranslationErrorCode { get; }

        public NoDataException(string message, TranslationErrorCode translationErrorCode)
        : base(message)
        {
            TranslationErrorCode = translationErrorCode;
        }
    }
}
