namespace SimpleFEM.UI.Services
{
    /// <summary>
    /// Defines the application-level workflow for loading and saving FEM model files.
    /// Handles dialog presentation, file operations, and view navigation on model load.
    /// </summary>
    public interface IModelLoadingService
    {
        /// <summary>
        /// Prompts the user to choose a save location, creates a new model file there,
        /// and navigates to the canvas view.
        /// </summary>
        Task NewModelAsync();

        /// <summary>
        /// Prompts the user to select an existing model file, opens it,
        /// and navigates to the canvas view.
        /// </summary>
        Task OpenModelAsync();

        /// <summary>
        /// Prompts the user to choose a save location and saves the current model there.
        /// Does nothing if no model file is currently open.
        /// </summary>
        Task SaveModelAsAsync();
    }
}
