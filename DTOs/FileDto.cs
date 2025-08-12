namespace study_ia_web_app.DTOs
{
    public class FileDto
    {
        public Guid ArchivoId { get; set; }
        public string NombreArchivo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaSubida { get; set; }
    }
}
