using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Services.quiz
{
    public interface IQuizBuilderService
    {
        Quiz ConstruirQuiz(Guid archivoId, List<PreguntaGenerada> preguntas);
    }
}
