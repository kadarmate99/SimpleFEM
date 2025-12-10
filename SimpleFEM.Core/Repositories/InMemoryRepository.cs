using SimpleFEM.Core.Interfaces;

namespace SimpleFEM.Core.Repositories
{
    public class InMemoryRepository<TModel> : IRepository<TModel> where TModel : IEntity
    {
        private readonly List<TModel> _items = new();

        public void Add(TModel entity)
        {
            int newId = _items.Any() ? _items.Max(i => i.Id) + 1 : 1;
            entity.Id = newId;
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

        public IEnumerable<TModel> GetAll()
        {
            return _items.ToList();
        }

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
