using System.Text.Json.Serialization;

namespace study_ia_web_app.DTOs
{
    public class ElevenLabsSttResponse
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
