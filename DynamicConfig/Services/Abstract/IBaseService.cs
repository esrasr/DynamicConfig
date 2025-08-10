using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Services.Abstract
{
    public interface IBaseService<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(int id);
        T Create(T entity);
        void Update(T entity);
        void Delete(int id);
    }
}
