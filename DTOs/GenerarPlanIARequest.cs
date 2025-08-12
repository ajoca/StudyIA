namespace study_ia_web_app.DTOs
{
    public class GenerarPlanIARequest
    {
        public Guid ArchivoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int HorasPorDia { get; set; }
        public string Nivel { get; set; } 
        public List<string>? TiposDeTarea { get; set; }
    }
}
