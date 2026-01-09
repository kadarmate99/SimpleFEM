namespace SimpleFEM.Data.Services
{
    public interface IModelFileService
    {
        /// <summary>
        /// The currently open model file path, or null if no file is open.
        /// </summary>
        string? CurrentModelFilePath { get; }

        /// <summary>
        /// The display name of the current model file (filename without path).
        /// </summary>
        string? CurrentModelFileName { get; }

        /// <summary>
        /// Indicates whether a model file is currently open.
        /// </summary>
        bool IsModelFileOpen { get; }

        /// <summary>
        /// Fired when a model file is opened or closed.
        /// </summary>
        event EventHandler? ModelFileChanged;

        /// <summary>
        /// Gets the connection string for the currently open model file.
        /// </summary>
        string? GetConnectionString();

        /// <summary>
        /// Creates a new empty model file.
        /// </summary>
        void NewModelFile(string path);

        /// <summary>
        /// Opens an existing model file.
        /// </summary>
        void OpenModelFile(string path);

        /// <summary>
        /// Saves the current model to a specific path.
        /// </summary>
        void SaveModelFileAs(string newPath);

        /// <summary>
        /// Closes the current file.
        /// </summary>
        void CloseCurrentModelFile();
    }
}
