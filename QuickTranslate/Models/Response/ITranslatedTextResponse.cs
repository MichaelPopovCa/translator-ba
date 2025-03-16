using Newtonsoft.Json;

namespace QuickTranslate.Models.Response
{
    public interface ITranslatedTextResponse
    {
        [JsonProperty("translatedText")]
        string TranslatedText { get; }
    }
}
