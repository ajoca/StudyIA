namespace study_ia_web_app.Entities
{
    public class PreguntaDto
    {
        public int PreguntaId { get; set; }
        public string Enunciado { get; set; }
        public List<AlternativaDto> Alternativas { get; set; }
    }
}
