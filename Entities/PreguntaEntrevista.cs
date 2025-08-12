namespace study_ia_web_app.Entities
{
    public class PreguntaEntrevista
    {
        public string Pregunta { get; set; } = string.Empty;
    }

    public enum TipoEntrevista
    {
        Tecnica,
        RecursosHumanos,
        Ingles
    }
}
