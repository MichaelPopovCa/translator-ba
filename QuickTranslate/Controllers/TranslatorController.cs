using Microsoft.AspNetCore.Mvc;
using QuickTranslate.Models.Request;
using QuickTranslate.Models.Response;
using QuickTranslate.Services.Business;

namespace QuickTranslate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TranslatorController : ControllerBase
    {
        private readonly ILogger<TranslatorController> _logger;
        private readonly ITranslatorService _translatorService;

        public TranslatorController(ILogger<TranslatorController> logger, ITranslatorService translatorService)
        {
            _logger = logger;
            _translatorService = translatorService;
        }

        [HttpPost("quick-translate")]
        public async Task<string> AsyncTranslate([FromBody] TranslationRequest translationRequest)
        {
            return await _translatorService.TranslateAsync(translationRequest);
        }

        [HttpGet("all-app-languages")]
        public async Task<IEnumerable<LanguageResponse>> AsyncGetAllAppLanguages()
        {
            return await _translatorService.GetAllAppLanguagesAsync();
        }

        [HttpGet("update-language-config")]
        public async Task<IEnumerable<LanguageResponse>> UpdateLanguageConfigurationAsync([FromQuery] string languageCode, [FromQuery] bool enable)
        {
            return await _translatorService.UpdateLanguageConfigurationAsync(languageCode, enable);
        }
    }
}