
using Sieve.Models;
using Sieve.Services;

namespace test
{
  
    public class TestSieveProcessor : ISieveProcessor
    {
        
        public IQueryable Apply(SieveModel model, IQueryable source, bool ensureOrdered = true) => source;

        public Task<IQueryable> ApplyAsync(SieveModel model, IQueryable source, CancellationToken cancellationToken = default) =>
            Task.FromResult(source);

       
        public IQueryable<T> Apply<T>(SieveModel model, IQueryable<T> source, bool ensureOrdered = true) => source;

        public Task<IQueryable<T>> ApplyAsync<T>(SieveModel model, IQueryable<T> source, CancellationToken cancellationToken = default) =>
            Task.FromResult(source);

        public IQueryable<TEntity> Apply<TEntity>(SieveModel model, IQueryable<TEntity> source, object[] dataForCustomMethods = null,
            bool applyFiltering = true, bool applySorting = true, bool applyPagination = true)
        {
            return source;
        }
    }
}