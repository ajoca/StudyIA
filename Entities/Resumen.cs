using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace study_ia_web_app.Entities
{
    public class Resumen
    {
        [Key]
        public Guid ResumenId { get; set; }
        [ForeignKey("Archivo")]
        public Guid ArchivoId { get; set; }
        public string Introduccion { get; set; }

        public string Conclusion { get; set; }
        public bool isResumen { get; set; }
        public DateTime FechaGenerado { get; set; }

        public Archivo Archivo { get; set; }

        public ICollection<PuntoClave> PuntosClave { get; set; }
    }
}
