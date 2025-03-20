using Microsoft.EntityFrameworkCore;
using QuickTranslate.Entities;
using QuickTranslate.Models.Request;

namespace QuickTranslate.Services.Validation
{
    public interface IValidationService
    {
        void ValidateTranslationRequest(TranslationRequest translationRequest);
        Task<string> ValidateTranslatedTextResponse(int translatorType, HttpResponseMessage httpResponseMessage);
        bool StringIsValid(string value);
        void ValidateLanguage(Language language);
        void ValidateLanguageCode(DbSet<Language> supportedLanguages, string languageCode);
    }
}
