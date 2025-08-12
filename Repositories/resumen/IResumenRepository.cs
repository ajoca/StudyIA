using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.resumen
{
    public interface IResumenRepository
    {
        Task AgregarResumenAsync(Resumen resumen);

        Task<Resumen> GetResumenByIdAsync(Guid fileId);

        Task<ArchivoResumenDTO?> GetResumenConArchivoAsync(Guid archivoId);
    }
}
