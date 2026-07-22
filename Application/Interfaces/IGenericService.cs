using Domain.Entities;

namespace Application.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{

    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    void Add(T entity);
    void Delete(T entity);
    void Update(T entity);
    Task<bool> SaveAllAsync();
    bool Exsits(int id);

}
