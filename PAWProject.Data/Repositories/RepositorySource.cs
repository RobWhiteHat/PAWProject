using Microsoft.EntityFrameworkCore;
using PAWProject.Data.Models;

namespace PAWProject.Data.Repositories;

public interface IRepositorySource
{
    Task<bool> UpsertAsync(Source entity, bool isUpdating);
    Task<bool> CreateAsync(Source entity);
    Task<bool> DeleteAsync(Source entity);
    Task<IEnumerable<Source>> ReadAsync();
    Task<Source> FindAsync(int id);

    Task<bool> UpdateAsync(Source entity);
    Task<bool> UpdateManyAsync(IEnumerable<Source> entities);
    Task<bool> ExistsAsync(Source entity);
    Task<Source?> GetByUrlAsync(string url);

}

public class RepositorySource : RepositoryBase<Source>, IRepositorySource
{
    public RepositorySource(Pawg3Context context) : base(context)
    {
    }
    public async Task<Source?> GetByUrlAsync(string url)
    {
        return await DbContext.Sources.FirstOrDefaultAsync(s => s.Url == url);
    }
}

