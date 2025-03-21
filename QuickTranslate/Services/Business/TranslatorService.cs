using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using QuickTranslate.Repositories.LanguageRepository;
using QuickTranslate.Services.Validation;
using QuickTranslate.Services.Vendor;

namespace QuickTranslate.Services.Business
{
    public class TranslatorService : ITranslatorService
    {
        private readonly ILogger<TranslatorService> _logger;
        private readonly IValidationService _validationService;
        private readonly ILanguageRepository _languageRepository;
        private readonly IVendorService _vendorService;

        public TranslatorService(ILogger<TranslatorService> logger, IValidationService validationService, ILanguageRepository languageRepository, IVendorService vendorService)
        {
            _logger = logger;
            _validationService = validationService;
            _languageRepository = languageRepository;
            _vendorService = vendorService;
        }

        public async Task<string> TranslateAsync(TranslationRequest translationRequest)
        {
            _logger.LogInformation($"TranslatorService => TranslateAsync with source language {translationRequest.SourceLanguage} and target language {translationRequest.TargetLanguage} started");
            _validationService.ValidateTranslationRequest(translationRequest);

            string result = await _vendorService.TranslateText(translationRequest);
            _logger.LogInformation($"TranslatorService => TranslateAsync with result {result} finished");
            return result;
        }

        public async Task<IEnumerable<LanguageResponse>> GetAllAppLanguagesAsync()
        {
            return await _languageRepository.GetAllAppLanguagesAsync();
        }

        public async Task<IEnumerable<LanguageResponse>> UpdateLanguageConfigurationAsync(string languageCode, bool enable)
        {
            _logger.LogInformation($"TranslatorService => UpdateLanguageConfigurationAsync with languageCode {languageCode} and config {enable} started");

            if (string.IsNullOrEmpty(languageCode))
            {
                throw new InvalidLanguageException($"The languageCode {languageCode} is not valid", TranslationErrorCode.InvalidLanguageException);
            }

            await _languageRepository.FindLanguageByLanguageCodeAndUpdate(languageCode, enable);

            IEnumerable<LanguageResponse> result = await _languageRepository.GetAllAppLanguagesAsync();

            _logger.LogInformation($"TranslatorService => UpdateLanguageConfigurationAsync finished");

            return result;
        }
    }
}
