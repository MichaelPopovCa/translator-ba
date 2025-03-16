using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Models.Request;
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

        public TranslatorService(ILogger<TranslatorService> logger, IValidationService validationService, TranslationAPI translationAPI, HttpClient httpClient)
        {
            _logger = logger;
            _validationService = validationService;
            _translationAPI = translationAPI;
            _httpClient = httpClient;
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
                api_key = "16a6654b-444e-488e-b577-2b05e53a87d2"
            };
            var jsonData = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(apiValue, content);

            string result = await _validationService.ValidateTranslatedTextResponse(translationRequest.TranslatorType, httpResponseMessage);

            return result;
        }
    }
}
