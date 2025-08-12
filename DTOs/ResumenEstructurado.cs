using System.Text.Json.Serialization;

namespace study_ia_web_app.DTOs
{
    public class ResumenEstructurado
    {
        [JsonPropertyName("introduccion")]
        public string Introduccion { get; set; }
        [JsonPropertyName("puntos_clave")]
        public List<string> PuntosClave { get; set; }
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; }
    }
}
