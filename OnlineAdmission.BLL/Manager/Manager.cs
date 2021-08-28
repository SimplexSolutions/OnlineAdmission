using OnlineAdmission.BLL.IManager;
using OnlineAdmission.DAL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineAdmission.BLL.Manager
{
    public class Manager<T> : IManager<T> where T : class
    {
        public readonly IRepository<T> _repository;
        public Manager(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<bool> AddAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public IQueryable<T> GetIQueryableData()
        {
            return _repository.GetIQueryableData();
        }

        public virtual async Task<bool> RemoveAsync(T entity)
        {
            return await _repository.RemoveAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await _repository.UpdateAsync(entity);
        }
    }
}
