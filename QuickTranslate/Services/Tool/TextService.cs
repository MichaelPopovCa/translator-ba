using QuickTranslate.Enums;
using QuickTranslate.Exceptions;

namespace QuickTranslate.Services.Tool
{
    public class TextService : ITextService
    {
        public string ConvertToLowerCaseExceptFirst(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidTranslationDataException($"The text is null or empty", TranslationErrorCode.InvalidTranslationRequestData);
            }
            var words = text.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0)
                {
                    words[i] = words[i].ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}
