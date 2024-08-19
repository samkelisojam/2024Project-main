namespace _2024FinalYearProject.Data.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> AddAsync(T entity);
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(int? id);
        Task RemoveAsync(int? id);
        Task UpdateAsync(T? entity);
        Task SaveAsync();
    }
}
