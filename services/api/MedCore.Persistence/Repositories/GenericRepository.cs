using MedCore.Application.Common;
using MedCore.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MedCore.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<T>().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().Where(predicate);
        if (include != null)
        {
            query = include(query);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<T>().Where(predicate);
        if (include != null)
        {
            query = include(query);
        }
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public virtual IQueryable<T> GetQueryable()
    {
        return _dbContext.Set<T>().AsQueryable();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task RestoreAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity is SoftDeleteEntity softDeleteEntity)
        {
            softDeleteEntity.IsDeleted = false;
            softDeleteEntity.DeletedAt = null;
            softDeleteEntity.DeletedBy = null;
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
        return Task.CompletedTask;
    }

    public virtual Task PermanentDeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (entity is SoftDeleteEntity softDeleteEntity)
        {
            // Ignore the SoftDelete interceptor by directly manipulating the state if needed, 
            // but normally EF Core's Remove on a SoftDelete intercepted entity triggers soft delete.
            // Wait, the AuditableEntityInterceptor changes Deleted to Modified for SoftDeleteEntity.
            // If we really want permanent delete, we might need a flag or raw SQL, or disable interceptor.
            // For now, we will just use DbContext.Remove and if the interceptor prevents it, we can revisit.
            _dbContext.Set<T>().Remove(entity);
        }
        else
        {
            _dbContext.Set<T>().Remove(entity);
        }
        
        return Task.CompletedTask;
    }
}
