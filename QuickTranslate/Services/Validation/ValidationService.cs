using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using System.Text.RegularExpressions;

namespace QuickTranslate.Services.Validation
{
    public partial class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly TranslationAPI _translationAPI;
        private readonly LanguageSupport _languageSupport;
        [GeneratedRegex(@"\b\w+\b")]
        private static partial Regex WordRegex();

        public ValidationService(ILogger<ValidationService> logger, TranslationAPI translationAPI, LanguageSupport languageSupport)
        {
            _logger = logger;
            _translationAPI = translationAPI;
            _languageSupport = languageSupport;
        }

        public void ValidateTranslationRequest(TranslationRequest translationRequest)
        {
            int translatorType = translationRequest.TranslatorType;
            if (translatorType == 0 || !_translationAPI.Api.ContainsKey(translatorType.ToString())) {
                throw new InvalidTranslationDataException($"The translatorType with number {translatorType}", TranslationErrorCode.InvalidTranslatorType);
            }
            string sourceLanguage = translationRequest.SourceLanguage;
            if (!StringIsValid(sourceLanguage) || !_languageSupport.Support.Contains(translationRequest.SourceLanguage))
            {
                throw new InvalidTranslationDataException($"The sourceLanguage {sourceLanguage} is not supported", TranslationErrorCode.InvalidTranslationRequestData);
            }
            string targetLanguage = translationRequest.TargetLanguage;
            if (!StringIsValid(targetLanguage) || !_languageSupport.Support.Contains(targetLanguage))
            {
                throw new InvalidTranslationDataException($"The targetLanguage {targetLanguage} is not supported", TranslationErrorCode.InvalidTranslationRequestData);
            }
            string sourceText = translationRequest.SourceText;
            if (!StringIsValid(sourceText) || !WordRegex().IsMatch(sourceText))
            {
                throw new InvalidTranslationDataException($"The sourceText is not valid", TranslationErrorCode.InvalidTranslationRequestData);
            }
        }
        public bool StringIsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public async Task<string> ValidateTranslatedTextResponse(int translatorType, HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null)
            {
                throw new NoDataException($"The api call to translator number {translatorType} returned null - no data", TranslationErrorCode.InvalidTranslationResponseData);
            }

            string json = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!StringIsValid(json))
            {
                throw new NoDataException($"No data from translator number {translatorType}", TranslationErrorCode.InvalidTranslationResponseData);
            }

            TranslatedTextResponse translatedTextResponse = JsonConvert.DeserializeObject<TranslatedTextResponse>(json) 
                ?? throw new NoDataException($"No data from translator number {translatorType}", TranslationErrorCode.InvalidTranslationResponseData);

            string translatedText = translatedTextResponse.TranslatedText;

            if (!StringIsValid(translatedText))
            {
                throw new NoDataException($"No data from translator number  {translatorType}", TranslationErrorCode.InvalidTranslationResponseData);
            }
            return translatedText;
        }
    }
}
