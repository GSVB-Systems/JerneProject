using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Repositories;
using dataaccess;
using service.Repositories.Interfaces;
using service.Services.Interfaces;

namespace service.Services;

public class Service<T> : IService<T> where T : class
{
    protected readonly IRepository<T> Repository;

    public Service(IRepository<T> repository)
    {
        Repository = repository;
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await Repository.AddAsync(entity);
        await Repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        var existing = await Repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        await Repository.UpdateAsync(entity);
        await Repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var existing = await Repository.GetByIdAsync(id);
        if (existing == null)
            return false;

        await Repository.DeleteAsync(existing);
        await Repository.SaveChangesAsync();
        return true;
    }
}