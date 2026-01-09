using Microsoft.EntityFrameworkCore;
using SimpleFEM.Data.Services;

namespace SimpleFEM.Data.Factories
{
    public class ModelFileDbContextFactory : IDbContextFactory<DataContext>
    {
        private readonly IModelFileService _modelFileService;

        public ModelFileDbContextFactory(IModelFileService modelFileService)
        {
            _modelFileService = modelFileService;
        }

        public DataContext CreateDbContext()
        {
            var connectionString = _modelFileService.GetConnectionString();

            if (connectionString == null)
            {
                throw new InvalidOperationException(
                    "Cannot create DbContext: ConnectionString is empty, " +
                    "probably no model file is open.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}
