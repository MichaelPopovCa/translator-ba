using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Entities;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using QuickTranslate.Repositories.DBContext;
using System.Text.RegularExpressions;

namespace QuickTranslate.Services.Validation
{
    public partial class ValidationService : IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        private readonly TranslationAPI _translationAPI;
        private readonly AppDbContext _appDbContext;
        [GeneratedRegex(@"\b\w+\b")]
        private static partial Regex WordRegex();

        public ValidationService(ILogger<ValidationService> logger, TranslationAPI translationAPI, AppDbContext appDbContext)
        {
            _logger = logger;
            _translationAPI = translationAPI;
            _appDbContext = appDbContext;
        }

        public void ValidateTranslationRequest(TranslationRequest translationRequest)
        {
            int translatorType = translationRequest.TranslatorType;
            if (translatorType == 0 || !_translationAPI.Api.ContainsKey(translatorType.ToString())) {
                throw new InvalidTranslationDataException($"The translatorType with number {translatorType}", TranslationErrorCode.InvalidTranslatorType);
            }

            DbSet<Language> supportedLanguages =  _appDbContext.Languages;

            string sourceLanguage = translationRequest.SourceLanguage;

            ValidateLanguageCode(supportedLanguages, sourceLanguage);

            string targetLanguage = translationRequest.TargetLanguage;
            ValidateLanguageCode(supportedLanguages, targetLanguage);

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

        public void ValidateLanguageCode(DbSet<Language> supportedLanguages, string languageCode)
        {
            if (!StringIsValid(languageCode) || !supportedLanguages.Any(l => l.LanguageCode == languageCode))
            {
                throw new InvalidTranslationDataException($"The languageCode {languageCode} is not supported", TranslationErrorCode.InvalidTranslationRequestData);
            }
        }

        public void ValidateLanguage(Language language)
        {
            if(!StringIsValid(language.LanguageCode) || !StringIsValid(language.LanguageName))
            {
                throw new InvalidTranslationDataException($"The language {language.LanguageCode} or {language.LanguageName} is not supported", TranslationErrorCode.InvalidTranslationRequestData);
            }
        }
    }
}
