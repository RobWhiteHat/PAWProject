using System.Collections.Generic;
using System.Threading.Tasks;
using PAWProject.Data.Models;

namespace PAWProject.Core.Interfaces
{
    public interface ISourceItemService
    {
        /// <summary>
        /// Guarda un SourceItem en la base de datos.
        /// </summary>
        Task<bool> SaveItemAsync(SourceItem item);

        /// <summary>
        /// Devuelve todos los SourceItems ordenados por fecha.
        /// </summary>
        Task<IEnumerable<SourceItem>> GetAllAsync();

        /// <summary>
        /// Devuelve un SourceItem por su Id, o null si no existe.
        /// </summary>
        Task<SourceItem?> GetByIdAsync(int id);
        Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id);
                  
        Task<Source> CreateSourceAsync(Source source);
    }
}