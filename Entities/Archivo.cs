using System.ComponentModel.DataAnnotations;

namespace study_ia_web_app.Entities
{
    public class Archivo
    {
        [Key]
        public Guid ArchivoId { get; set; }
        public Guid UserId { get; set; } // Por ahora puedes usar Guid.Empty para pruebas
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public string TextoExtraido { get; set; }

        public string Descripcion { get; set; }
        public DateTime FechaSubida { get; set; }

        public ICollection<Resumen> Resumenes { get; set; }
    }
}
