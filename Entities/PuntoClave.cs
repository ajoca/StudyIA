using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace study_ia_web_app.Entities
{
    public class PuntoClave
    {
        [Key]
        public Guid PuntoClaveId { get; set; }

        [ForeignKey("Resumen")]
        public Guid ResumenId { get; set; }

        public string Texto { get; set; }

        public Resumen Resumen { get; set; }
    }
}
