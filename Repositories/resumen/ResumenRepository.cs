using Microsoft.EntityFrameworkCore;
using study_ia_web_app.Data;
using study_ia_web_app.DTOs;
using study_ia_web_app.Entities;

namespace study_ia_web_app.Repositories.resumen
{
    public class ResumenRepository : IResumenRepository
    {
        private readonly ApplicationDbContext _context;

        public ResumenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AgregarResumenAsync(Resumen resumen)
        {
            _context.Resumens.Add(resumen);
            await _context.SaveChangesAsync();
        }

        public async Task<Resumen> GetResumenByIdAsync(Guid fileId)
        {
            var resumes = await _context.Resumens
                .Include(r => r.PuntosClave)
                .FirstOrDefaultAsync(r => r.ResumenId == fileId);

            if (resumes == null)
            {
                throw new KeyNotFoundException($"No se encontró el resumen con ID {fileId}");
            }

            return resumes;
        }

        public async Task<ArchivoResumenDTO?> GetResumenConArchivoAsync(Guid archivoId)
        {
            var resumen = await _context.Resumens
                .Include(r => r.PuntosClave)
                .Include(r => r.Archivo)
                .FirstOrDefaultAsync(r => r.ArchivoId == archivoId);

            if (resumen == null || resumen.Archivo == null)
            {
                return null;
            }

            return new ArchivoResumenDTO
            {
                NombreArchivo = resumen.Archivo.NombreArchivo,
                FechaGenerado = resumen.FechaGenerado,
                Introduccion = resumen.Introduccion,
                Conclusion = resumen.Conclusion,
                PuntosClave = resumen.PuntosClave.Select(p => p.Texto).ToList()
            };
        }
    }
}
