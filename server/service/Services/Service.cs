// csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Repositories;
using dataaccess;

namespace service.Services;

public class Service<T> : IService<T> where T : class
{
    protected readonly IRepository<T> _repository;

    public Service(IRepository<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await _repository.DeleteAsync(existing);
        await _repository.SaveChangesAsync();
        return true;
    }
}