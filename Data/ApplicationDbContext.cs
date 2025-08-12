using Microsoft.EntityFrameworkCore;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Archivo> Archivos { get; set; }
        public DbSet<Resumen> Resumens { get; set; }
        public DbSet<PuntoClave> Puntos { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Alternativa> Alternativas { get; set; }
        public DbSet<PlanEstudio> PlanEstudios { get; set; }
        public DbSet<TareaEstudio> TareasEstudios { get; set; }
    }
}
