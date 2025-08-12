namespace study_ia_web_app.Entities
{
    public class Alternativa
    {
        public int AlternativaId { get; set; }
        public int PreguntaId { get; set; }
        public string Texto { get; set; }
        public bool EsCorrecta { get; set; }
    }
}
