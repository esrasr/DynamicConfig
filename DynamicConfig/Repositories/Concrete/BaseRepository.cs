using DynamicConfig.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DynamicConfig.Repositories.Concrete
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _ctx;
        protected readonly DbSet<T> _set;

        public BaseRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            _set = _ctx.Set<T>();
        }

        public virtual IQueryable<T> Query() => _set.AsNoTracking();

        public virtual IEnumerable<T> GetAll() => Query().ToList();

        public virtual T? GetById(int id) =>
            _set.AsNoTracking().FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

        public virtual void Add(T entity)
        {
            _set.Add(entity);
            _ctx.SaveChanges();
        }

        public virtual void Update(T entity)
        {
            _set.Attach(entity);
            _ctx.Entry(entity).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity is null) return;
            _set.Remove(entity);
            _ctx.SaveChanges();
        }
    }
}
