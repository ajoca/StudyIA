namespace study_ia_web_app.DTOs
{
    public class TareaPorDia
    {
        public DateTime Fecha { get; set; }
        public string TituloSeccion { get; set; }
        public List<string> Tareas { get; set; }
    }
}
