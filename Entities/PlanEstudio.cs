using System.ComponentModel.DataAnnotations;

namespace study_ia_web_app.Entities
{
    public class PlanEstudio
    {
        [Key]
        public Guid PlanEstudioId { get; set; }
        public Guid ArchivoId { get; set; }
        public Archivo Archivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<TareaEstudio> Tareas { get; set; }
    }
}
