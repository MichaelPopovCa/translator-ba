namespace QuickTranslate.Models.Request
{
    public interface ITranslationRequest
    {
        int TranslatorType { get; }
        string SourceLanguage { get; }
        string SourceText { get; }
        string TargetLanguage { get; }
    }
}
