namespace study_ia_web_app.Entities
{
    public class QuizDetalleDTO
    {
        public int QuizId { get; set; }
        public Guid ArchivoId { get; set; }
        public string NombreArchivo { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaGenerado { get; set; }
        public List<PreguntaDto> Preguntas { get; set; }
    }
}
