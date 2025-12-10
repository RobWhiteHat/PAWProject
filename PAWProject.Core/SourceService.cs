using PAWProject.Data.Models;
using PAWProject.Data.Repositories;

namespace PAWProject.Core;

public interface ISourceService
{
    Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id);
}

public class SourceService(IRepositorySource repositorySource) : ISourceService
{
    public async Task<IEnumerable<Source>> GetArticlesFromDBAsync(int? id)
    {
        return id == null
            ? await repositorySource.ReadAsync()
            : [await repositorySource.FindAsync((int)id)];
    }
}
