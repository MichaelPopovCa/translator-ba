namespace QuickTranslate.Models.Request
{
    public class TranslationRequest : ITranslationRequest
    {
        public int TranslatorType { get; set; }
        public string SourceLanguage { get; set; }
        public string SourceText { get; set; }
        public string TargetLanguage { get; set; }
    }
}
