using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.archivo
{
    public interface IFileRepository
    {
        Task GuardarAsync(Archivo archivo);

        Task<IEnumerable<Archivo>> ListarTodosAsync();

        Task<Archivo?> GetFileByIdAsync(Guid id);
    }
}
