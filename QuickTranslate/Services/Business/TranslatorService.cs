using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using QuickTranslate.Repositories.DBContext;
using QuickTranslate.Services.Validation;
using QuickTranslate.Socket;
using System.Text;

namespace QuickTranslate.Services.Business
{
    public class TranslatorService : ITranslatorService
    {
        private readonly ILogger<TranslatorService> _logger;
        private readonly IValidationService _validationService;
        private readonly IOptions<TranslationAPI> _translationAPI;
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<TranslationHub> _hubContext;
        private readonly IOptions<TranslationVendorSecret> _translationVendorSecret;



        public TranslatorService(ILogger<TranslatorService> logger, IValidationService validationService, IOptions<TranslationAPI> translationAPI, HttpClient httpClient, AppDbContext appDbContext, IHubContext<TranslationHub> hubContext, IOptions<TranslationVendorSecret> translationVendorSecret)
        {
            _logger = logger;
            _validationService = validationService;
            _translationAPI = translationAPI;
            _httpClient = httpClient;
            _appDbContext = appDbContext;
            _hubContext = hubContext;
            _translationVendorSecret = translationVendorSecret;
        }

        public async Task<string> TranslateAsync(TranslationRequest translationRequest)
        {
            _validationService.ValidateTranslationRequest(translationRequest);

            _translationAPI.Value.Api.TryGetValue(translationRequest.TranslatorType.ToString(), out var apiValue);

            var requestBody = new
            {
                q = ConvertToLowerCaseExceptFirst(translationRequest.SourceText),
                source = translationRequest.SourceLanguage,
                target = translationRequest.TargetLanguage,
                format = "text",
                alternatives = 3,
                api_key = _translationVendorSecret.Value.Api.First().Value,
            };

            var jsonData = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(apiValue, content);

            string result = await _validationService.ValidateTranslatedTextResponse(translationRequest.TranslatorType, httpResponseMessage);

            Console.WriteLine(result);

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

         
            existingLanguage.Enabled = enable;
            await _appDbContext.SaveChangesAsync();
           

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

        public string ConvertToLowerCaseExceptFirst(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new InvalidTranslationDataException($"The text is null or empty", TranslationErrorCode.InvalidTranslationRequestData);
            }
            var words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0)
                {
                    words[i] = words[i].ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}
