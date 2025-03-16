using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;

namespace QuickTranslate.Services.Validation
{
    public interface IValidationService
    {
        void ValidateTranslationRequest(TranslationRequest translationRequest);
        Task<string> ValidateTranslatedTextResponse(int translatorType, HttpResponseMessage httpResponseMessage);
        bool StringIsValid(string value);
    }
}
