namespace QuickTranslate.Models.Response
{
    public class TranslatedTextResponse : ITranslatedTextResponse
    {
        public required string TranslatedText { get; set; }
    }
}
