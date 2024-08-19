using _2024FinalYearProject.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _2024FinalYearProject.Data
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly AppDbContext _context;
        public RepositoryBase(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public async Task<string> GetUsernameByIdAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user.UserName;
        }
        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task RemoveAsync(int? id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T? entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();

        }

    }
}