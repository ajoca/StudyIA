using study_ia_web_app.DTOs;
using System.Text.Json;
using System.Text;

namespace study_ia_web_app.Services.studio
{
    public class AiPlanService : IAiPlanService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string _geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public AiPlanService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }


        public async Task<List<TareaPorDia>> GenerarPlanEstudioJsonAsync(
            string texto, DateTime fechaInicio, DateTime fechaFin, int horasPorDia, 
            string nivel, List<string>? tiposDeTarea)
        {
            var tipos = tiposDeTarea != null && tiposDeTarea.Any()
            ? string.Join(", ", tiposDeTarea)
            : "lectura, resumen";

            var prompt = $@"Eres un asistente experto en crear planes de estudio personalizados a partir de textos académicos estructurados. El usuario subió un documento que contiene capítulos, secciones y subtemas.
            Condiciones del usuario:

            - Crea un plan de estudio en español desde el {fechaInicio:dd/MM/yyyy} hasta el {fechaFin:dd/MM/yyyy}.
            - Usa exclusivamente los títulos y subtítulos reales del texto proporcionado para identificar las secciones (por ejemplo: Capítulo I, Marco teórico, Análisis, etc.).
            - Cada día debe tener 2 a 4 tareas organizadas por fecha, incluyendo los tipos que el usuario desea: {tipos}.
            - Asegúrate de que las tareas se alineen **directamente con el contenido del texto** (no inventes títulos como 'Introducción' o 'Conceptos Fundamentales' si no existen en el documento).
            - Tiempo máximo por día: {horasPorDia} horas.
            - Nivel de detalle: {nivel}.
            - No agregues otros tipos.
            - No uses todo en mayúsculas aunque el texto original lo esté. Ajusta el formato al estilo académico estándar.
            - Evita repeticiones innecesarias o frases genéricas.

            Devuelve solo un **array JSON válido** (sin texto adicional) con el siguiente formato:

            [
              {{
                ""fecha"": ""YYYY-MM-DD"",
                ""tituloSeccion"": ""Nombre real de la sección del documento"",
                ""tareas"": [
                  ""Tarea 1 relacionada con esa sección"",
                  ""Tarea 2 relacionada con esa sección"",
                  ""Tarea 3 relacionada con esa sección""
                ]
              }},
              ...
            ]
            Importante:
            - Texto académico para generar plan: {texto}.
            - No agregues ningún comentario ni explicación fuera del JSON";


            var requestBody = new GeminiRequest
            {
                contents = new List<Content>
                {
                    new Content
                    {
                        parts = new List<Part>
                        {
                            new Part { text = prompt }
                        }
                    }
                }
            };

            var apiKey = _configuration["GeminiIA:ApiKey"]?.Trim();

            var requestJson = JsonSerializer.Serialize(requestBody);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_geminiEndpoint}?key={apiKey}")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al consumir Gemini API");



            var responseJson = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

            var rawContent = geminiResponse?.candidates?[0]?.content?.parts?[0]?.text;
            Console.WriteLine("RESPUESTA" + rawContent);


            rawContent = rawContent?.Trim();

            Console.WriteLine("🔍 Contenido generado por la IA:");
            Console.WriteLine(rawContent);


            if (string.IsNullOrWhiteSpace(rawContent))
                throw new ApplicationException("La respuesta de la IA está vacía.");

            if (rawContent.StartsWith("```"))
            {
                var start = rawContent.IndexOf('[');
                var end = rawContent.LastIndexOf(']');

                if (start != -1 && end != -1)
                    rawContent = rawContent.Substring(start, end - start + 1);
            }

            return JsonSerializer.Deserialize<List<TareaPorDia>>(rawContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
