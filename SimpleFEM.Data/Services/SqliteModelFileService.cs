using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace SimpleFEM.Data.Services
{
    public class SqliteModelFileService : IModelFileService
    {
        public string? CurrentModelFilePath { get; private set; }

        public string? CurrentModelFileName =>
            CurrentModelFilePath != null
            ? Path.GetFileName(CurrentModelFilePath)
            : null;

        public bool IsModelFileOpen => CurrentModelFilePath != null;

        public event EventHandler? ModelFileChanged;

        public string? GetConnectionString()
        {
            if (CurrentModelFilePath == null)
                return null;

            return $"Data Source ={CurrentModelFilePath};Pooling=false";
        }

        public void NewModelFile(string path)
        {
            CloseCurrentModelFile();

            if (File.Exists(path))
                File.Delete(path);

            CurrentModelFilePath = path;

            // Create and initialize the new database (model file)
            using (var context = CreateDbContext())
            {
                context.Database.Migrate();
            }

            ModelFileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OpenModelFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Model File not found: {path}");

            CloseCurrentModelFile();

            CurrentModelFilePath = path;

            using (var context = CreateDbContext())
            {
                context.Database.Migrate();
            }

            ModelFileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SaveModelFileAs(string newPath)
        {
            if (CurrentModelFilePath == null)
                throw new InvalidOperationException("No Model File is currently open.");

            if (CurrentModelFilePath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("New path must be different from current path.");

            // Create new db and backup the existing into that
            using (var sourceConnection = new SqliteConnection(GetConnectionString()))
            using (var destConnection = new SqliteConnection($"Data Source={newPath};Pooling=false"))
            {
                sourceConnection.Open();
                destConnection.Open();

                sourceConnection.BackupDatabase(destConnection);
            }

            // switch to the new file
            OpenModelFile(newPath);
        }

        public void CloseCurrentModelFile()
        {
            if (CurrentModelFilePath == null)
                return;

            SqliteConnection.ClearAllPools(); // releases all file handles

            CurrentModelFilePath = null;

            ModelFileChanged?.Invoke(this, EventArgs.Empty);
        }

        private DataContext CreateDbContext()
        {
            var connectionString = GetConnectionString();
            if (connectionString == null)
                throw new InvalidOperationException("No document is open.");

            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new DataContext(optionsBuilder.Options);
        }
    }
}
