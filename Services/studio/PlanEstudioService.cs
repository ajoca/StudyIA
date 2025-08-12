using Microsoft.AspNetCore.Http.HttpResults;
using study_ia_web_app.Entities;
using study_ia_web_app.Repositories.archivo;
using study_ia_web_app.Repositories.planstudio;

namespace study_ia_web_app.Services.studio
{
    public class PlanEstudioService : IPlanEstudioService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IPlanEstudioRepository _planStudioRepository;
        private readonly IAiPlanService _aiPlanService;

        public PlanEstudioService(IFileRepository fileRepository, 
            IPlanEstudioRepository planStudioRepository, IAiPlanService aiPlanService)
        {
            _fileRepository = fileRepository;
            _planStudioRepository = planStudioRepository;
            _aiPlanService = aiPlanService;
        }


        public async Task<PlanEstudio> GenerarPlanIaAsync(Guid archivoId,
            DateTime fechaInicio, DateTime fechaFin, int horasPorDia, 
            string nivel, List<string>? tiposDeTarea)
        {
            var archivo = await _fileRepository.GetFileByIdAsync(archivoId);

            var texto = archivo?.TextoExtraido;

            if (texto == null)
            {
                throw new ArgumentNullException(nameof(texto));
            }

            var taskDay = await _aiPlanService
                .GenerarPlanEstudioJsonAsync(texto, fechaInicio, fechaFin,
                horasPorDia, nivel, tiposDeTarea);

            var planId = Guid.NewGuid();
            var newPlan = new PlanEstudio
            {
                PlanEstudioId = planId,
                ArchivoId = archivoId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Tareas = taskDay
                .SelectMany(day => day.Tareas.Select(t => new TareaEstudio
                {
                    TareaEstudioId = Guid.NewGuid(),
                    PlanId = planId,
                    Fecha = day.Fecha,
                    Descripcion = t,
                    TituloSeccion = day.TituloSeccion
                })).ToList()
            };

            await _planStudioRepository.GuardarAsync(newPlan);

            return newPlan;
        }
    }
}
