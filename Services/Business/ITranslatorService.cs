using QuickTranslate.Models.Request;

namespace QuickTranslate.Services.Business
{
    public interface ITranslatorService
    {
        Task<string> AsyncTranslate(TranslationRequest translator);
    }
}
