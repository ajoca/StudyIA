using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.quiz
{
    public interface IQuizRepository
    {
        Task AddAsync(Quiz quiz);

        Task<Quiz?> ObtenerQuizPorArchivoIdAsync(Guid archivoId);

        Task<List<Quiz>> GetAllWithArchivoAsync();
    }
}
