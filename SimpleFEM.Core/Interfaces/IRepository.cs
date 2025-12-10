namespace SimpleFEM.Core.Interfaces
{
    public interface IRepository<TModel> where TModel : IEntity
    {
        IEnumerable<TModel> GetAll();
        TModel GetById(int id);
        void Add(TModel entity);
        void Update(TModel entity);
        void Delete(int id);
    }
}
