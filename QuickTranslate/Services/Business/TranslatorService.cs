using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Entities;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using QuickTranslate.Repositories.DBContext;
using QuickTranslate.Services.Validation;
using System.Text;

namespace QuickTranslate.Services.Business
{
    public class TranslatorService : ITranslatorService
    {
        private readonly ILogger<TranslatorService> _logger;
        private readonly IValidationService _validationService;
        private readonly TranslationAPI _translationAPI;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _appDbContext;


        public TranslatorService(ILogger<TranslatorService> logger, IValidationService validationService, TranslationAPI translationAPI, HttpClient httpClient, AppDbContext appDbContext)
        {
            _logger = logger;
            _validationService = validationService;
            _translationAPI = translationAPI;
            _httpClient = httpClient;
            _appDbContext = appDbContext;
        }

        public async Task<string> TranslateAsync(TranslationRequest translationRequest)
        {
            _validationService.ValidateTranslationRequest(translationRequest);
            _translationAPI.Api.TryGetValue(translationRequest.TranslatorType.ToString(), out var apiValue);
            var requestBody = new
            {
                q = translationRequest.SourceText, 
                source = "auto",                   
                target = translationRequest.TargetLanguage, 
                format = "text",
                alternatives = 3,                  
                api_key = ""
            };
            var jsonData = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(apiValue, content);

            string result = await _validationService.ValidateTranslatedTextResponse(translationRequest.TranslatorType, httpResponseMessage);

            return result;
        }

        public async Task<IEnumerable<LanguageResponse>> GetAllAppLanguagesAsync()
        {
            return await _appDbContext.Languages
                            .Select(l => new LanguageResponse
                            {
                                LanguageCode = l.LanguageCode,
                                LanguageName = l.LanguageName,
                                Enabled = l.Enabled,
                            })
                            .ToListAsync();
        }

        public async Task<IEnumerable<LanguageResponse>> UpdateLanguageConfigurationAsync(string languageCode, bool enable)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                throw new InvalidLanguageException($"The languageCode {languageCode} is not valid", TranslationErrorCode.InvalidLanguageException);
            }

            var existingLanguage = await _appDbContext.Languages
                .FirstOrDefaultAsync(l => l.LanguageCode == languageCode);

            if (existingLanguage == null)
            {
                throw new InvalidLanguageException($"The language with code {languageCode} is not available", TranslationErrorCode.InvalidLanguageException);
            }

            if (existingLanguage.Enabled != enable)
            {
                existingLanguage.Enabled = enable;
                await _appDbContext.SaveChangesAsync();
            }

            var languageResponses = await _appDbContext.Languages
                .Select(l => new LanguageResponse
                {
                    LanguageCode = l.LanguageCode,
                    LanguageName = l.LanguageName,
                    Enabled = l.Enabled
                })
                .ToListAsync();

            return languageResponses;
        }
    }
}
