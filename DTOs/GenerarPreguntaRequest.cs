using study_ia_web_app.Entities;

namespace study_ia_web_app.DTOs
{
    public class GenerarPreguntaRequest
    {
        public TipoEntrevista TipoEntrevista { get; set; }
        public string Carrera { get; set; }
    }
}
