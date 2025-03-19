using QuickTranslate.Entities;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;

namespace QuickTranslate.Services.Business
{
    public interface ITranslatorService
    {
        Task<string> TranslateAsync(TranslationRequest translator);
        Task<IEnumerable<LanguageResponse>> UpdateLanguageConfigurationAsync(string languageCode, bool enable);
        Task<IEnumerable<LanguageResponse>> GetAllAppLanguagesAsync();
        private string ConvertToLowerCaseExceptFirst(string input);
    }
}
