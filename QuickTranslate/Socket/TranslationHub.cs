using Microsoft.AspNetCore.SignalR;
using QuickTranslate.Models.Request;
using QuickTranslate.Services.Business;

namespace QuickTranslate.Socket
{
    public class TranslationHub : Hub
    {
        private readonly ITranslatorService _translatorService;

        public TranslationHub(ITranslatorService translatorService)
        {
            _translatorService = translatorService;
        }

        public async Task ReceiveTextForTranslation(int translatorType, string sourceText, string sourceLanguage, string targetLanguage)
        {
            var translationRequest = new TranslationRequest
            {
                TranslatorType = translatorType,
                SourceText = sourceText,
                SourceLanguage = sourceLanguage,
                TargetLanguage = targetLanguage
            };
            string translatedText = await _translatorService.TranslateAsync(translationRequest);
            await Clients.Caller.SendAsync("ReceiveTranslatedText", translatedText);
        }
    }
}
