using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Entities;
using QuickTranslate.Enums;
using QuickTranslate.Exceptions;
using QuickTranslate.Models.Request;
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

        public async Task<string> AsyncTranslate(TranslationRequest translationRequest)
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

        public async Task<IEnumerable<string>> AsyncGetSupportedLanguages()
        {

            return await _appDbContext.LanguageSupports
                                .Join(_appDbContext.Languages,
                                    ls => ls.ForeignKeyLanguageId,  
                                    l => l.Id,                      
                                    (ls, l) => l.LanguageCode)      
                                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AsyncGetAllAppLanguages()
        {
            return await _appDbContext.Languages
                            .Select(l => l.LanguageCode)
                            .ToListAsync();
        }

        public async Task<IEnumerable<string>> AsyncAddNewSupportedLanguage(string languageCode)
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

            var languageSupport = await _appDbContext.LanguageSupports
                .FirstOrDefaultAsync(ls => ls.ForeignKeyLanguageId == existingLanguage.Id);

            if (languageSupport != null)
            {
                throw new InvalidLanguageException($"The language with code {languageCode} already has support", TranslationErrorCode.InvalidLanguageException);
            }

            var newLanguageSupport = new LanguageSupport
            {
                ForeignKeyLanguageId = existingLanguage.Id
            };

            _appDbContext.LanguageSupports.Add(newLanguageSupport);
            await _appDbContext.SaveChangesAsync();

            Console.WriteLine($"Added new language support for language code: {languageCode}");

            var supportedLanguages = await _appDbContext.LanguageSupports
                .Select(ls => ls.ForeignKeyLanguageId)
                .ToListAsync();

            var result = await _appDbContext.Languages
                .Where(l => supportedLanguages.Contains(l.Id))
                .Select(l => l.LanguageCode)
                .ToListAsync();

            Console.WriteLine("Current supported languages: " + string.Join(", ", result));

            return result;
        }
    }
}
