using study_ia_web_app.DTOs;

namespace study_ia_web_app.Services.quiz
{
    public interface IAiQuizGenerator
    {
        Task<List<PreguntaGenerada>> GenerarPreguntasAsync(string texto, int cantidad);
    }
}
