using study_ia_web_app.DTOs;

namespace study_ia_web_app.Services.studio
{
    public interface IAiPlanService
    {
        Task<List<TareaPorDia>> GenerarPlanEstudioJsonAsync(
            string texto, DateTime fechaInicio, DateTime fechaFin, int horasPorDia,
            string nivel, List<string>? tiposDeTarea);
    }
}
