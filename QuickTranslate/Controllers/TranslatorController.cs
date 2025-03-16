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

        [HttpPost]
        public async Task<String> AsyncTranslate([FromBody] TranslationRequest translationRequest)
        {
            return await _translatorService.AsyncTranslate(translationRequest);
        }
    }
}