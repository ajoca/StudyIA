using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Services
{
    public interface IAiService
    {
        Task<ResumenEstructurado> GenerarResumenAsync(string texto);

        Task<DescripcionFile> FileGetDescription(string texto);

        Task<DescripcionFile> ArchivoGetDescription(string texto);

        Task<PreguntaEntrevista> GenerarPreguntaAsync(TipoEntrevista tipo, string carrera);

        Task<byte[]> ConvertirTextoAVozAsync(string texto);

        Task<string> TranscribirAudioAsync(IFormFile audioFile);

        Task<string> EvaluarRespuestaAsync(string pregunta, string respuesta);

        Task<ResumenEstructurado> GenerarResumenConGeminiAsync(string texto);
    }
}
