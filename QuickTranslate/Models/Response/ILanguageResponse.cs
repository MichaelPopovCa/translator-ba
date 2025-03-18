namespace QuickTranslate.Models.Response
{
    public interface ILanguageResponse
    {
        string LanguageCode { get; set; }
        string LanguageName { get; set; }
        public bool Enabled { get; set; }
    }
}
