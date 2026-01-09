using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Data.Repositories.InMemory
{
    public class InMemoryRepository<TModel> : IRepository<TModel> where TModel : IEntity
    {
        private readonly List<TModel> _items = new();

        public void Add(TModel entity)
        {
            // Only generate ID if not already set (support undo)
            if (entity.Id == 0)
            {
                // Id not set, generate new ID
                int newId = _items.Any() ? _items.Max(i => i.Id) + 1 : 1;
                entity.Id = newId;
            }
            else
            {
                // Restoring an entity with existing ID
                // verify it's unique
                if (_items.Any(i => i.Id == entity.Id))
                    throw new InvalidOperationException($"Entity with ID {entity.Id} already exists.");
            }


            _items.Add(entity);
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public IEnumerable<TModel> GetAll() => _items.ToList();

        public TModel GetById(int id)
        {
            var model = _items.FirstOrDefault(x => x.Id == id);
            if (model is null)
                throw new InvalidOperationException(
                    $"The specified object with ID {id}) does not exist in the Database.");

            return model;
        }

        public void Update(TModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
