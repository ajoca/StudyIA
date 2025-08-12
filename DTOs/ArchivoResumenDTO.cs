namespace study_ia_web_app.DTOs
{
    public class ArchivoResumenDTO
    {
        public string NombreArchivo { get; set; }
        public DateTime FechaGenerado { get; set; }

        public string Introduccion { get; set; }
        public List<string> PuntosClave { get; set; }
        public string Conclusion { get; set; }
    }
}
