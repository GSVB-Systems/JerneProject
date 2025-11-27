// csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Repositories;
using dataaccess;
using service.Repositories.Interfaces;
using service.Services.Interfaces;

namespace service.Services;

public class Service<T> : IService<T> where T : class
{
    protected readonly IRepository<T> RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository;

    public Service(IRepository<T> repositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository)
    {
        RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository = repositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository;
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        return await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.GetAllAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.AddAsync(entity);
        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        var existing = await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.GetByIdAsync(id);
        if (existing == null)
            return null;

        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.UpdateAsync(entity);
        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var existing = await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.DeleteAsync(existing);
        await RepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepositoryRepository.SaveChangesAsync();
        return true;
    }
}