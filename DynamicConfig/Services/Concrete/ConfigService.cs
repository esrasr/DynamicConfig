using DynamicConfig.Models;
using DynamicConfig.Repositories.Abstract;
using DynamicConfig.Repositories.Concrete;
using DynamicConfig.Services.Abstract;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Globalization;


namespace DynamicConfig.Services.Concrete
{
    public class ConfigService : BaseService<Config>, IConfigService
    {
        protected readonly IConfigRepository _configRepo;

        public ConfigService(IConfigRepository configRepo) : base(configRepo)
        {
            _configRepo = configRepo;
        }
        public Config? Get(string name, string appName)
        {
            return _configRepo.GetConfigByValue(name, appName);
        }



        public T GetValue<T>(string name, string appName)
        {
            var config = _configRepo.GetConfigByValue(name, appName);
            if (config == null || string.IsNullOrWhiteSpace(config.Value))
                return default!; // string -> null, int? -> null, int -> 0

            try
            {
                return ConvertTo<T>(config.Value, config.Type);
            }
            catch
            {
                return default!;
            }
        }

        public IEnumerable<Config> GetAll(string appName)
        {
            return _configRepo.GetAll(appName);
        }
        public Config Add(ConfigDto entity, string appName)
        {


            return _configRepo.Add(entity, appName);
        }
        public Config Update(int id, ConfigDto entity, string appName)
        {
            return _configRepo.Update(id, entity, appName);
        }
        public int Delete(int id, string appName)
        {
            return _configRepo.Delete(id, appName);
        }

        private static T ConvertTo<T>(string? raw, string? declaredType)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default!;

            object? parsed = ParseByDeclared(raw, declaredType);
            if (parsed is null) return default!;

            var target = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (target.IsInstanceOfType(parsed)) return (T)parsed;

            try
            {
                if (target == typeof(string))
                    return (T)(object)Convert.ToString(parsed, CultureInfo.InvariantCulture)!;

                if (parsed is string s)
                    return (T)Convert.ChangeType(s, target, CultureInfo.InvariantCulture);

                return (T)Convert.ChangeType(parsed, target, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default!;
            }
        }

        private static object? ParseByDeclared(string raw, string? declaredType)
        {
            switch (declaredType?.Trim().ToLowerInvariant())
            {
                case "int":
                case "integer":
                    return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : null;

                case "double":
                case "float":
                case "number":
                    return double.TryParse(raw, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var d) ? d : null;

                case "bool":
                case "boolean":
                    if (bool.TryParse(raw, out var b)) return b;
                    var s = raw.Trim().ToLowerInvariant();
                    if (s is "1" or "yes" or "y" or "on") return true;
                    if (s is "0" or "no" or "n" or "off") return false;
                    return null;

                default: // bilinmiyorsa string kabul et
                    return raw;
            }
        }
    }
}
