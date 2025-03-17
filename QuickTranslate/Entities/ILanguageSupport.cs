namespace QuickTranslate.Entities
{
    public interface ILanguageSupport
    {
        public long Id { get; set; }
        public long ForeignKeyLanguageId { get; set; }
        public Language Language { get; set; }
    }
}
