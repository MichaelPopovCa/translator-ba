using QuickTranslate.Models.Request;

namespace QuickTranslate.Services.Vendor
{
    public interface IVendorService
    {
        Task<string> TranslateText(TranslationRequest translationRequest);
    }
}
