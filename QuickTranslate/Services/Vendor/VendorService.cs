using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuickTranslate.Configurations;
using QuickTranslate.Models.Request;
using QuickTranslate.Services.Tool;
using QuickTranslate.Services.Validation;
using System.Text;

namespace QuickTranslate.Services.Vendor
{
    public class VendorService : IVendorService
    {
        private readonly HttpClient _httpClient;
        private readonly ITextService _textService;
        private readonly IOptions<TranslationVendorSecret> _translationVendorSecret;
        private readonly IOptions<TranslationAPI> _translationAPI;
        private readonly IValidationService _validationService;

        public VendorService(HttpClient httpClient, ITextService textService, IOptions<TranslationVendorSecret> translationVendorSecret, IOptions<TranslationAPI> translationAPI, IValidationService validationService)
        {
            _httpClient = httpClient;
            _textService = textService;
            _translationVendorSecret = translationVendorSecret;
            _translationAPI = translationAPI;
            _validationService = validationService;
        }

        public async Task<string> TranslateText(TranslationRequest translationRequest)
        {
            _translationAPI.Value.Api.TryGetValue(translationRequest.TranslatorType.ToString(), out var apiValue);

            var requestBody = new
            {
                q = _textService.ConvertToLowerCaseExceptFirst(translationRequest.SourceText),
                source = translationRequest.SourceLanguage,
                target = translationRequest.TargetLanguage,
                format = "text",
                alternatives = 3,
                api_key = _translationVendorSecret.Value.Api.First().Value,
            };

            var jsonData = JsonConvert.SerializeObject(requestBody);

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(apiValue, content);

            return await _validationService.ValidateTranslatedTextResponse(translationRequest.TranslatorType, httpResponseMessage);
        }
    }
}
