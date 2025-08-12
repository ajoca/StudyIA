using study_ia_web_app.DTOs;
using System.Text.Json;
using System.Text;

namespace study_ia_web_app.Services.quiz
{
    public class AiQuizGenerator : IAiQuizGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string _geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public AiQuizGenerator(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<PreguntaGenerada>> GenerarPreguntasAsync(string texto, int cantidad)
        {
            var prompt = $"Genera {cantidad} preguntas de opción múltiple con " +
                $"4 alternativas sobre el siguiente texto:\n\n\"{texto}\"\n\n" +
                "Incluye en cada pregunta: enunciado, alternativas, e indica cuál es la correcta. " +
                "Devuelve solo un array JSON válido con el siguiente formato y sin ningún texto adicional ni explicación:\n" +
                "[{ \"enunciado\": \"...\", \"alternativas\": [\"...\", \"...\", \"...\", \"...\"], \"respuestaCorrecta\": \"Texto exacto de la alternativa correcta\" }]";


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

            return JsonSerializer.Deserialize<List<PreguntaGenerada>>(rawContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private string LimpiarJsonDeContenido(string rawContent)
        {
            // Elimina los delimitadores ```json y ```
            if (rawContent.StartsWith("```"))
            {
                rawContent = rawContent.Replace("```json", "").Replace("```", "").Trim();
            }

            // Reemplaza comillas simples por dobles, solo si es necesario
            if (rawContent.Contains('\'') && !rawContent.Contains('\"'))
            {
                rawContent = rawContent.Replace('\'', '\"');
            }

            // Opcional: intenta extraer sólo el objeto JSON si hay texto fuera
            int startIndex = rawContent.IndexOf('[');
            int endIndex = rawContent.LastIndexOf(']');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                rawContent = rawContent.Substring(startIndex, endIndex - startIndex + 1);
            }

            return rawContent;
        }
    }
}
