using Microsoft.AspNetCore.Mvc;
using QuickTranslate.Models.Request;
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
            return await _translatorService.AsyncTranslate(translationRequest);
        }

        [HttpGet("all-app-languages")]
        public async Task<IEnumerable<string>> AsyncGetAllAppLanguages()
        {
            return await _translatorService.AsyncGetAllAppLanguages();
        }

        [HttpGet("supported-languages")]
        public async Task<IEnumerable<string>> AsyncGetSupportedLanguages()
        {
            return await _translatorService.AsyncGetSupportedLanguages();
        }

        [HttpPost("add-language")]
        public async Task<IEnumerable<string>> AsyncAddNewSupportedLanguage([FromQuery] string languageCode)
        {
            return await _translatorService.AsyncAddNewSupportedLanguage(languageCode);
        }
    }
}