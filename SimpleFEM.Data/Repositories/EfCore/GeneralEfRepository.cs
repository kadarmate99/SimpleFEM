using Microsoft.EntityFrameworkCore;
using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Data.Repositories.EfCore
{
    /// <summary>
    /// Base general repo implementation using EF. Methods allow override.
    /// </summary>
    public abstract class GeneralEfRepository<TModel> : IRepository<TModel> where TModel : class, IEntity
    {
        protected readonly IDbContextFactory<DataContext> _contextFactory;

        public GeneralEfRepository(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public virtual IEnumerable<TModel> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TModel>().ToList();
        }
        
        public virtual TModel? GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Set<TModel>().Find(id);
        }

        public virtual void Add(TModel model)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TModel>().Add(model);
            context.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            var model = context.Set<TModel>().Find(id);

            if (model is null)
                return;

            context.Set<TModel>().Remove(model);
            context.SaveChanges();
        }

        public virtual void Update(TModel model)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Set<TModel>().Update(model);
            context.SaveChanges();
        }
    }
}
