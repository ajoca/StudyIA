using Microsoft.EntityFrameworkCore;
using study_ia_web_app.Data;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.archivo
{
    public class FileRepository : IFileRepository
    {
        private readonly ApplicationDbContext _context;
        public FileRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task GuardarAsync(Archivo archivo)
        {
            _context.Archivos.Add(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Archivo>> ListarTodosAsync()
        {
            return await _context.Archivos.OrderByDescending(fil => fil.FechaSubida).ToListAsync();
        }

        public async Task<Archivo?> GetFileByIdAsync(Guid id)
        {
            return await _context.Archivos.FirstOrDefaultAsync(fil => fil.ArchivoId == id);
        }
    }
}
