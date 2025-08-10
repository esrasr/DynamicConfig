using DynamicConfig.Models;

namespace DynamicConfig.Services.Abstract
{
    public interface IConfigService : IBaseService<Config>
    {
        public Config? Get(string key, string appName);
        public T GetValue<T>(string key, string appName);
        public IEnumerable<Config> GetAll(string appName);
        public Config Add(ConfigDto entity, string appName);
        public Config Update(int id, ConfigDto entity, string appName);
        public int Delete(int id, string appName);

    }
}
