namespace QuickTranslate.Entities
{
    public class Language : ILanguage
    {
        public long Id { get; set; }
        public string LanguageCode { get; set; }
        public string LanguageName { get; set; }
    }
}
