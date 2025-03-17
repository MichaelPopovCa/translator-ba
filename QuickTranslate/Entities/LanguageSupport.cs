namespace QuickTranslate.Entities
{
    public class LanguageSupport : ILanguageSupport
    {
        public long Id { get; set; }
        public long ForeignKeyLanguageId { get; set; }
        public Language Language { get; set; } 
    }
}
