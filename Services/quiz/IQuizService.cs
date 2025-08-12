using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Services.quiz
{
    public interface IQuizService
    {
        Task<Quiz> GenerarQuizAsync(Guid archivoId, int cantidadPreguntas);
        Task<QuizDetalleDTO?> ObtenerQuizPorArchivoIdAsync(Guid archivoId);

        Task<List<QuizListDto>> ObtenerTodosQuizzesAsync();
    }
}
