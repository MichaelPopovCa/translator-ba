namespace QuickTranslate.Entities
{
    public interface ILanguage
    {
        public long Id { get; set; }
        string LanguageCode { get; set; }
        string LanguageName { get; set; }
    }
}
