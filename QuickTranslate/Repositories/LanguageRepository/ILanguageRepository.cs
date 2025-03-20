using QuickTranslate.Models.Response;

namespace QuickTranslate.Repositories.LanguageRepository
{
    public interface ILanguageRepository
    {
        Task<IEnumerable<LanguageResponse>> GetAllAppLanguagesAsync();
        Task FindLanguageByLanguageCodeAndUpdate(string languageCode, bool enable);
    }
}
