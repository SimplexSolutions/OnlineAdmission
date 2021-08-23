using Microsoft.EntityFrameworkCore;
using OnlineAdmission.DAL.IRepository;
using OnlineAdmission.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.DAL.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly OnlineAdmissionDbContext _context;
        public Repository(OnlineAdmissionDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table {
            get { return _context.Set<T>(); } 
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            await Table.AddAsync(entity);
            bool isSaved =await _context.SaveChangesAsync() > 0;
            return isSaved;
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await Table.FindAsync(id);
        }

        public virtual async Task<bool> RemoveAsync(T entity)
        {
            Table.Remove(entity);
            bool isSaved = await _context.SaveChangesAsync() > 0;
            return isSaved;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            bool isSaved = await _context.SaveChangesAsync() > 0;
            return isSaved;
        }
    }
}
