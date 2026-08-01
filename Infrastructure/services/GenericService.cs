
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class GenericService<T>(StoreContext context, ILogger<GenericService<T>> logger) : IGenericService<T> where T : BaseEntity
{
    // Entity type name is logged as a property so Seq can group by it across all closed generics.
    private static readonly string EntityName = typeof(T).Name;

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await context.Set<T>().FindAsync(id);

        if (entity is null)
        {
            logger.LogDebug("{EntityName} {EntityId} not found", EntityName, id);
        }

        return entity;
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await context.Set<T>().ToListAsync();
    }

    public async void Add(T entity)
    {
        logger.LogInformation("Adding {EntityName} to the change tracker", EntityName);
        await context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        logger.LogInformation("Marking {EntityName} {EntityId} as modified", EntityName, entity.Id);
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        logger.LogInformation("Marking {EntityName} {EntityId} for deletion", EntityName, entity.Id);
        context.Set<T>().Remove(entity);
    }

    public bool Exsits(int id)
    {
        return context.Set<T>().Any(x => x.Id == id);
    }
    public async Task<bool> SaveAllAsync()
    {
        try
        {
            var affected = await context.SaveChangesAsync();

            if (affected == 0)
            {
                logger.LogWarning("SaveChanges for {EntityName} completed without affecting any rows", EntityName);
            }

            return affected > 0;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed while saving {EntityName}", EntityName);
            throw;
        }
    }
}
