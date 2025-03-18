namespace QuickTranslate.Models.Response
{
    public class LanguageResponse : ILanguageResponse
    {
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
        public bool Enabled { get; set; }
    }
}
