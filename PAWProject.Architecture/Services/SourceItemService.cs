using System.Threading.Tasks;
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

        public async Task<bool> SaveItemAsync(SourceItem item)
        {
            try
            {
                var sourceExists = await _context.Sources.AnyAsync(s => s.Id == item.SourceId);
                if (!sourceExists) return false;

                item.CreatedAt = DateTime.UtcNow;
                _context.SourceItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // test de que funcione todo
                Console.WriteLine($"Error al guardar SourceItem: {ex.Message}");
                return false;
            }
        }
    }
}