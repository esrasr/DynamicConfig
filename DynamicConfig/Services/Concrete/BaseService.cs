using DynamicConfig.Repositories.Abstract;
using DynamicConfig.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Services.Concrete
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        public BaseService(IBaseRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual IEnumerable<T> GetAll() => _repository.GetAll();

        public virtual T? GetById(int id) => _repository.GetById(id);

        public virtual T Create(T entity)
        {
            _repository.Add(entity);
            return entity;
        }

        public virtual void Update(T entity) => _repository.Update(entity);

        public virtual void Delete(int id) => _repository.Delete(id);
    }
}
