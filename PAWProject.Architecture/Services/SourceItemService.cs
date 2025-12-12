using Microsoft.EntityFrameworkCore;
using PAWProject.Data.Models;
using PAWProject.Core.Interfaces;

namespace PAWProject.Architecture.Services
{
    public class SourceItemService : ISourceItemService
    {
        private readonly Pawg3Context _context;

        public SourceItemService(Pawg3Context context)
        {
            _context = context;
        }

        public Task<Source> CreateSourceAsync(Source source)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SourceItem>> GetAllAsync()
        {
            // Devuelve todos los SourceItems ordenados por fecha de creación
            return await _context.SourceItems
                                 .OrderByDescending(s => s.CreatedAt)
                                 .ToListAsync();
        }

        public Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<SourceItem?> GetByIdAsync(int id)
        {
            // este Busca un SourceItem por su Id
            return await _context.SourceItems.FindAsync(id);
        }

        public async Task<bool> SaveItemAsync(SourceItem item)
        {
            try
            {
                if (item.SourceId == null) return false;

                var sourceExists = await _context.Sources.AnyAsync(s => s.Id == item.SourceId.Value);
                if (!sourceExists) return false;

                item.CreatedAt = DateTime.UtcNow;
                _context.SourceItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar SourceItem: {ex.Message}");
                return false;
            }
        }
    }
}