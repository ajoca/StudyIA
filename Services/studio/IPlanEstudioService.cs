using study_ia_web_app.Entities;

namespace study_ia_web_app.Services.studio
{
    public interface IPlanEstudioService
    {
        Task<PlanEstudio> GenerarPlanIaAsync(Guid archivoId,
            DateTime fechaInicio, DateTime fechaFin, int horasPorDia,
            string nivel, List<string>? tiposDeTarea);
    }
}
