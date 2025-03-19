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

        public async Task ReceiveTextForTranslation(TranslationRequest translationRequest)
        {
            string translatedText = await _translatorService.TranslateAsync(translationRequest);
            await Clients.Caller.SendAsync("ReceiveTranslatedText", translatedText);
        }
    }
}
