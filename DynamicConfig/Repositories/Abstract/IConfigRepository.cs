using DynamicConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfig.Repositories.Abstract
{
    public interface IConfigRepository : IBaseRepository<Config>
    {
        public Config? GetConfigByValue(string name, string appName);
        public IEnumerable<Config> GetAll(string appName);
        public T GetValue<T>(string name, string appName);
        public Task<Config?> GetConfigByValueAsync(string name, string appName, CancellationToken ct = default);
        public Task<List<Config>> GetAllAsync(string appName, CancellationToken ct = default);
        public Task<T> GetValueAsync<T>(string name, string appName, CancellationToken ct = default);
        public Config Add(ConfigDto entity, string appName);
        public Config Update(int id, ConfigDto entity, string appName);
        public int Delete(int id, string appName);
    }
}
