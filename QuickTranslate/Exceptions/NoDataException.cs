using QuickTranslate.Enums;

namespace QuickTranslate.Exceptions
{
    public class NoDataException : TranslationException
    {
        public NoDataException(string message, TranslationErrorCode translationErrorCode)
        : base(message, translationErrorCode)
        {}
    }
}
