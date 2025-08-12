using Microsoft.EntityFrameworkCore;
using study_ia_web_app.Data;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.quiz
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task AddAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<Quiz?> ObtenerQuizPorArchivoIdAsync(Guid archivoId)
        {
            return await _context.Quizzes
                .Include(q => q.Preguntas)
                .ThenInclude(p => p.Alternativas)
                .Include(q => q.Archivo)
                .FirstOrDefaultAsync(q => q.ArchivoId == archivoId);
        }

        public async Task<List<Quiz>> GetAllWithArchivoAsync()
        {
            return await _context.Quizzes
                .Include(q => q.Archivo)
                .Include(q => q.Preguntas)
                .ToListAsync();
        }
    }
}
