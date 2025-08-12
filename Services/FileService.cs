using study_ia_web_app.DTOs;
using study_ia_web_app.Repositories.archivo;
using study_ia_web_app.Repositories.resumen;
using System.Text;
using UglyToad.PdfPig;

namespace study_ia_web_app.Services
{
    public class FileService : IFileService
    {

        private readonly IFileRepository _fileRepository;
        private readonly IResumenRepository _resumenRepository;

        public FileService(IFileRepository repository, IResumenRepository resumenRepository)
        {  
            _fileRepository = repository; 
            _resumenRepository = resumenRepository;
        }

        public async Task<string> ExtraerTextoAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var text = new StringBuilder();
            using (var document = PdfDocument.Open(memoryStream))
            {
                foreach (var item in document.GetPages())
                {
                    text.Append(item.Text);
                }
            }
            return text.ToString();
        }

        public async Task<IEnumerable<FileDto>> getAllFilesAsync()
        {
            var files = await _fileRepository.ListarTodosAsync();

            return files.Select(a => new FileDto
            {
                ArchivoId = a.ArchivoId,
                NombreArchivo = a.NombreArchivo,
                Descripcion = a.Descripcion,
                FechaSubida = a.FechaSubida,
            });
        }

        public async Task<FileDto?> getFileById(Guid fileId)
        {
            var file = await _fileRepository.GetFileByIdAsync(fileId);

            if (file == null)
            {
                return null;
            }

            return new FileDto
            {
                ArchivoId = file.ArchivoId,
                NombreArchivo = file.NombreArchivo,
                Descripcion = file.Descripcion,
                FechaSubida = file.FechaSubida,
            };
        }

        public async Task<ArchivoResumenDTO?> getResumenById(Guid fileId)
        {
            return await _resumenRepository.GetResumenConArchivoAsync(fileId);
        }
    }
}
