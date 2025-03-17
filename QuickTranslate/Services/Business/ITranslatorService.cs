using QuickTranslate.Entities;
using QuickTranslate.Models.Request;

namespace QuickTranslate.Services.Business
{
    public interface ITranslatorService
    {
        Task<string> AsyncTranslate(TranslationRequest translator);
        Task<IEnumerable<string>> AsyncGetSupportedLanguages();    
        Task<IEnumerable<string>> AsyncAddNewSupportedLanguage(string languageCode);
        Task<IEnumerable<string>> AsyncGetAllAppLanguages();
    }
}
