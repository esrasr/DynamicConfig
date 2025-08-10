using System.Globalization;
using Microsoft.EntityFrameworkCore;
using DynamicConfig.Models;
using DynamicConfig.Repositories.Abstract;

namespace DynamicConfig.Repositories.Concrete
{
    public class ConfigRepository : BaseRepository<Config>, IConfigRepository
    {
        public ConfigRepository(ApplicationDbContext ctx) : base(ctx) { }

        public Config? GetConfigByValue(string name, string appName)
        {
            return _ctx.Configs.AsNoTracking()
                           .FirstOrDefault(p => p.Name == name &&
                                                p.ApplicationName == appName &&
                                                p.IsActive);
        }

        public IEnumerable<Config> GetAll(string appName)
        {
            return _ctx.Configs
                           .AsNoTracking()
                           .Where(p => p.ApplicationName == appName && p.IsActive)
                           .ToList();
        }

        public T GetValue<T>(string name, string appName)
        {
            var cfg = GetConfigByValue(name, appName);
            return ConvertTo<T>(cfg?.Value);
        }

        // ---- Async eşdeğerleri (opsiyonel) ----
        public Task<Config?> GetConfigByValueAsync(string name, string appName, CancellationToken ct = default)
        {
            return _ctx.Configs
                           .AsNoTracking()
                           .FirstOrDefaultAsync(p => p.Name == name &&
                                                     p.ApplicationName == appName &&
                                                     p.IsActive, ct);
        }

        public Task<List<Config>> GetAllAsync(string appName, CancellationToken ct = default)
        {
            return _ctx.Configs
                           .AsNoTracking()
                           .Where(p => p.ApplicationName == appName && p.IsActive)
                           .ToListAsync(ct);
        }

        public async Task<T> GetValueAsync<T>(string name, string appName, CancellationToken ct = default)
        {
            var cfg = await GetConfigByValueAsync(name, appName, ct);
            return ConvertTo<T>(cfg?.Value);
        }

        private static T ConvertTo<T>(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return default!;

            var t = typeof(T);
            var target = Nullable.GetUnderlyingType(t) ?? t;

            try
            {
                if (target == typeof(int))
                    return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)
                        ? (T)(object)i : default!;

                if (target == typeof(double))
                    return double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)
                        ? (T)(object)d : default!;

                if (target == typeof(bool))
                {
                    if (bool.TryParse(raw, out var b)) return (T)(object)b;
                    if (raw == "1") return (T)(object)true;
                    if (raw == "0") return (T)(object)false;
                    return default!;
                }

                if (target == typeof(string))
                    return (T)(object)raw;

                return (T)Convert.ChangeType(raw, target, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default!;
            }
        }

        public Config Add(ConfigDto entity, string appName)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Config? existing = GetConfigByValue(entity.Name, appName);
            if (existing != null)
            {
                throw new InvalidOperationException($"Config with name '{entity.Name}' already exists for application '{appName}'.");
            }
            var newEntity = new Config
            {
                Name = entity.Name,
                Type = entity.Type,
                Value = entity.Value,
                IsActive = entity.IsActive,
                ApplicationName = appName
            };
            _set.Add(newEntity);
            _ctx.SaveChanges();
            return newEntity;
        }
        public Config Update(int id, ConfigDto dto, string appName)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var existing = _ctx.Configs
                .AsTracking() // global NoTracking açıksa gerekli
                .FirstOrDefault(x => x.Id == id && x.ApplicationName == appName);

            if (existing is null)
                throw new KeyNotFoundException($"Config '{id}' not found for '{appName}'.");

            // alanları ata
            existing.Name = dto.Name?.Trim() ?? existing.Name;
            existing.Type = dto.Type;
            existing.Value = dto.Value.Trim();
            existing.IsActive = dto.IsActive;

            _ctx.SaveChanges();     // artık gerçekten update gönderir
            return existing;
        }
        public int Delete(int id, string appName)
        {
            var existing = GetById(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Config with ID '{id}' not found.");
            }
            if (existing.ApplicationName != appName)
            {
                throw new InvalidOperationException($"Config with ID '{id}' does not belong to application '{appName}'.");
            }
            _set.Remove(existing);
            _ctx.SaveChanges();
            return id;
        }
    }
}
