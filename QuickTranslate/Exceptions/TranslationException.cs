using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class TranslationException : Exception
    {
        public TranslationErrorCode TranslationErrorCode { get; }

        public TranslationException(string message, TranslationErrorCode translationErrorCode)
        : base(message)
        {
            TranslationErrorCode = translationErrorCode;
        }
    }
}
