namespace study_ia_web_app.Entities
{
    public class Pregunta
    {
        public int PreguntaId { get; set; }
        public int QuizId { get; set; }
        public string Enunciado { get; set; }
        public List<Alternativa> Alternativas { get; set; }
    }
}
