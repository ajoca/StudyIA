using study_ia_web_app.DTOs;

namespace study_ia_web_app.Services
{
    public interface IFileService
    {
        Task<string> ExtraerTextoAsync(IFormFile file);

        Task<IEnumerable<FileDto>> getAllFilesAsync();

        Task<FileDto?> getFileById(Guid id);

        Task<ArchivoResumenDTO?> getResumenById(Guid fileId);


    }
}
