namespace study_ia_web_app.DTOs
{
    public class QuizListDto
    {
        public int QuizId { get; set; }
        public Guid ArchivoId { get; set; }
        public string NombreArchivo { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaGenerado { get; set; }
        public int CantidadPreguntas { get; set; }
    }
}
