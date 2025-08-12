using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.planstudio
{
    public interface IPlanEstudioRepository
    {
        Task GuardarAsync(PlanEstudio planEstudio);
    }
}
