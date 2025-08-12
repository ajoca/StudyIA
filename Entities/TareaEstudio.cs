namespace study_ia_web_app.Entities
{
    public class TareaEstudio
    {
        public Guid TareaEstudioId { get; set; }
        public Guid PlanId { get; set; }
        public string Descripcion { get; set; }

        public string? TituloSeccion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
