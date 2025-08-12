namespace study_ia_web_app.Entities
{
    public class Quiz
    {
        public int QuizId { get; set; }
        public Guid ArchivoId { get; set; }
        public Archivo Archivo { get; set; }
        public string Titulo { get; set; }
        public bool isQuiz {  get; set; }
        public DateTime FechaGenerado { get; set; }
        public List<Pregunta> Preguntas { get; set; }
    }
}
