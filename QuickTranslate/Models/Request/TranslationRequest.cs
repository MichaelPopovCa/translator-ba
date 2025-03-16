namespace QuickTranslate.Models.Request
{
    public class TranslationRequest : ITranslationRequest
    {
        public int TranslatorType { get; }
        public string SourceLanguage { get; }
        public string SourceText { get; }
        public string TargetLanguage { get; }

        public TranslationRequest(int translatorType, string sourceLanguage, string sourceText, string targetLanguage)
        {
            TranslatorType = translatorType;
            SourceLanguage = sourceLanguage;
            SourceText = sourceText;
            TargetLanguage = targetLanguage;
        }
    }
}
