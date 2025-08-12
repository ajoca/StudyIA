using Microsoft.AspNetCore.Http;
using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace study_ia_web_app.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly string _geminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

        public AiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ResumenEstructurado> GenerarResumenConGeminiAsync(string texto)
        {
            string prompt =
            "Resume el siguiente texto académico en un máximo de 200 palabras. Devuélvelo en formato JSON con esta estructura:\n\n" +
            "{\n" +
            "  \"introduccion\": \"Texto introductorio del resumen...\",\n" +
            "  \"puntos_clave\": [\"Primer punto clave...\", \"Segundo punto clave...\", \"...\"],\n" +
            "  \"conclusion\": \"Texto de conclusión del resumen...\"\n" +
            "}\n\n" +
            "Asegúrate de que los puntos clave estén en forma de lista clara y que la información sea detallada y específica. Texto para resumir:\n\n" +
            texto;


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


            if (string.IsNullOrWhiteSpace(rawContent))
                throw new Exception("El resumen generado está vacío.");

            var jsonLimpio = LimpiarJsonDeContenido(rawContent);

            try
            {
                var resumenEstructurado = JsonSerializer.Deserialize<ResumenEstructurado>(jsonLimpio);
                return resumenEstructurado!;
            }
            catch (JsonException ex)
            {
                // Opcional: puedes guardar el contenido original para depurar
                throw new Exception("Error al deserializar el resumen: " + ex.Message);
            }
        }

        public async Task<DescripcionFile> ArchivoGetDescription(string texto)
        {
            string prompt = $"""
            Devuelve solo la descripción del texto en 150 palabras, sin explicaciones 
            adicionales ni texto extra. {texto} 
            """;

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


            if (string.IsNullOrWhiteSpace(rawContent))
                throw new Exception("La pregunta generada está vacía.");

            return new DescripcionFile { Descripcion = rawContent.Trim() };
        }



        public async Task<ResumenEstructurado> GenerarResumenAsync(string texto)
        {

            string promp =
            "Resume el siguiente texto académico en un máximo de 200 palabras. Devuélvelo en formato JSON con esta estructura:\n\n" +
            "{\n" +
            "  \"introduccion\": \"Texto introductorio del resumen...\",\n" +
            "  \"puntos_clave\": [\"Primer punto clave...\", \"Segundo punto clave...\", \"...\"],\n" +
            "  \"conclusion\": \"Texto de conclusión del resumen...\"\n" +
            "}\n\n" +
            "Asegúrate de que los puntos clave estén en forma de lista clara y que la información sea detallada y específica. Texto para resumir:\n\n" +
            texto;

            var request = new
            {
                model = "mistralai/mistral-small-3.2-24b-instruct:free",
                messages = new[]
                {
                    new { role = "system", content = "Eres un asistente experto en resumir" +
                    " documentos académicos." },
                    new { role = "user", content = promp }
                }
            };

            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://openrouter.ai/api/v1/chat/completions"),
                Content = JsonContent.Create(request)
            };

            var apiKey = _configuration["OpenRouter:ApiKey"]?.Trim();


            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            httpRequest.Headers.Referrer = new Uri("http://localhost");

            var response = await _httpClient.SendAsync(httpRequest);

            response.EnsureSuccessStatusCode();


            var rawResponse = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();

            var rawContent = rawResponse?.Choices?[0]?.Message?.Content;

            Console.WriteLine("RESPUESTA" + rawContent);

            if (string.IsNullOrWhiteSpace(rawContent))
                throw new Exception("El resumen generado está vacío.");

            var jsonLimpio = LimpiarJsonDeContenido(rawContent);

            try
            {
                var resumenEstructurado = JsonSerializer.Deserialize<ResumenEstructurado>(jsonLimpio);
                return resumenEstructurado!;
            }
            catch (JsonException ex)
            {
                // Opcional: puedes guardar el contenido original para depurar
                throw new Exception("Error al deserializar el resumen: " + ex.Message);
            }

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
            int startIndex = rawContent.IndexOf('{');
            int endIndex = rawContent.LastIndexOf('}');
            if (startIndex >= 0 && endIndex > startIndex)
            {
                rawContent = rawContent.Substring(startIndex, endIndex - startIndex + 1);
            }

            return rawContent;
        }

        // GENERAR PREGUNTA ALEATORIA
        public async Task<PreguntaEntrevista> GenerarPreguntaAsync(TipoEntrevista tipo, string carrera)
        {
            var tipoStr = tipo switch
            {
                TipoEntrevista.Tecnica => "entrevista técnica",
                TipoEntrevista.RecursosHumanos => "entrevista de recursos humanos",
                TipoEntrevista.Ingles => "entrevista en inglés",
                _ => "entrevista"
            };

            string prompt = $"""
            Simula una {tipoStr} para un estudiante o profesional en el área de {carrera}.
            Genera una única pregunta clara, relevante y desafiante para el nivel junior o intermedio.
            Devuelve solo la pregunta, sin explicaciones adicionales ni texto extra.
            """;

            var request = new
            {
                model = "mistralai/mistral-small-3.2-24b-instruct:free",
                messages = new[]
            {
                new { role = "system", content = "Eres un experto entrevistador profesional que formula preguntas de entrevistas según el área y tipo de entrevista." },
                new { role = "user", content = prompt }
            }
            };

            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://openrouter.ai/api/v1/chat/completions"),
                Content = JsonContent.Create(request)
            };

            var apiKey = _configuration["OpenRouter:ApiKey"]?.Trim();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Headers.Referrer = new Uri("http://localhost");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();
            var content = rawResponse?.Choices?[0]?.Message?.Content;

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("La pregunta generada está vacía.");

            return new PreguntaEntrevista { Pregunta = content.Trim() };
        }

        public async Task<DescripcionFile> FileGetDescription(string texto)
        {
            string prompt = $"""
            Devuelve solo la descripción del texto en 150 palabras, sin explicaciones adicionales ni texto extra. {texto} 
            """;

            var request = new
            {
                model = "mistralai/mistral-small-3.2-24b-instruct:free",
                messages = new[]
            {
                new { role = "system", content = "Eres un experto en realizar y obtener la descripción del contenido del texto." },
                new { role = "user", content = prompt }
            }
            };

            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://openrouter.ai/api/v1/chat/completions"),
                Content = JsonContent.Create(request)
            };

            var apiKey = _configuration["OpenRouter:ApiKey"]?.Trim();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Headers.Referrer = new Uri("http://localhost");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var rawResponse = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();
            var content = rawResponse?.Choices?[0]?.Message?.Content;

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("La pregunta generada está vacía.");

            return new DescripcionFile { Descripcion = content.Trim() };
        }

        // GENERAR VOZ DE PREGUNTA GENERADA
        public async Task<byte[]> ConvertirTextoAVozAsync(string texto)
        {
            var apiKey = _configuration["ElevenLabs:ApiKey"];
            var voiceId = _configuration["ElevenLabs:VoiceId"];

            var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}")
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    text = texto,
                    model_id = "eleven_multilingual_v2",
                    voice_settings = new
                    {
                        stability = 0.5,
                        similarity_boost = 0.75
                    }
                }), Encoding.UTF8, "application/json")
            };

            request.Headers.Add("xi-api-key", apiKey);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var audioBytes = await response.Content.ReadAsByteArrayAsync();

            return audioBytes;
        }

        // TRANSCRIBIR AUDIO A TEXTO
        public async Task<string> TranscribirAudioAsync(IFormFile audioFile)
        {
            var apiKey = _configuration["ElevenLabs:ApiKey"];

            
            using var content = new MultipartFormDataContent();

            using var stream = audioFile.OpenReadStream();

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(audioFile.ContentType);

            content.Add(fileContent, "file", audioFile.FileName);

            // agregamos contenido model_id
            content.Add(new StringContent("scribe_v1"), "model_id");

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.elevenlabs.io/v1/speech-to-text");
            request.Headers.Add("xi-api-key", apiKey);
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var parsed = JsonSerializer.Deserialize<ElevenLabsSttResponse>(json);

            return parsed?.Text ?? throw new Exception("No se obtuvo transcripción.");
        }

        // EVALUAR RESPUESTA:
        public async Task<string> EvaluarRespuestaAsync(string pregunta, string respuesta)
        {
            var prompt = $"""
            Simula que eres un entrevistador profesional.

            Esta fue la pregunta realizada:
            "{pregunta}"

            Y esta fue la respuesta del candidato:
            "{respuesta}"

            Por favor, evalúa la respuesta con base en los siguientes criterios:

            1. Claridad y coherencia.
            2. Precisión técnica o conceptual.
            3. Seguridad/confianza en la respuesta.
            4. Áreas de mejora.

            Devuelve un texto estructurado con observaciones y recomendaciones constructivas, en máximo 200 palabras.
            """;

            var request = new
            {
                model = "mistralai/mistral-small-3.2-24b-instruct:free",
                messages = new[]
                {
                    new { role = "system", content = "Eres un experto entrevistador técnico y de recursos humanos." },
                    new { role = "user", content = prompt }
                }
            };

            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://openrouter.ai/api/v1/chat/completions"),
                Content = JsonContent.Create(request)
            };

            var apiKey = _configuration["OpenRouter:ApiKey"];
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpRequest.Headers.Referrer = new Uri("http://localhost");

            var response = await _httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenRouterResponse>();
            return result?.Choices?[0]?.Message?.Content ?? throw new Exception("Mistral no devolvió feedback.");
        }


    }
}
