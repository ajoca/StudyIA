using study_ia_web_app.Data;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.planstudio
{
    public class PlanEstudioRepository : IPlanEstudioRepository
    {
        private readonly ApplicationDbContext _context;

        public PlanEstudioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GuardarAsync(PlanEstudio planEstudio)
        {
            _context.PlanEstudios.Add(planEstudio);
            await _context.SaveChangesAsync();
        }
    }
}
