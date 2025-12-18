using Microsoft.EntityFrameworkCore;
using SimpleFEM.Core.Interfaces;

namespace SimpleFEM.Data.Repositories
{
    public class EfRepository<TModel> : IRepository<TModel> where TModel : class, IEntity
    {
        private readonly DataContext _context;
        private readonly DbSet<TModel> _dbSet;

        public EfRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Set<TModel>();
        }

        public IEnumerable<TModel> GetAll() => _dbSet.ToList();
        
        public TModel? GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Add(TModel model)
        {
            _dbSet.Add(model);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var model = GetById(id);

            if (model is null)
                return;
            
            _dbSet.Remove(model);
            _context.SaveChanges();
        }

        public void Update(TModel model)
        {
            _dbSet.Update(model);
            _context.SaveChanges();
        }
    }
}
