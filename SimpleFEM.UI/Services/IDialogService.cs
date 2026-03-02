namespace SimpleFEM.UI.Services
{
    /// <summary>
    /// Abstracts OS-level dialog interactions
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Opens a Save File dialog and returns the chosen file path,
        /// or <see langword="null"/> if the user cancelled.
        /// </summary>
        string? ShowSaveFileDialog(string title, string filter, string defaultExtension);

        /// <summary>
        /// Opens an Open File dialog and returns the chosen file path,
        /// or <see langword="null"/> if the user cancelled.
        /// </summary>
        string? ShowOpenFileDialog(string title, string filter, string defaultExtension);

        /// <summary>
        /// Displays a modal error message dialog.
        /// </summary>
        void ShowError(string message);

        /// <summary>
        /// Displays a modal informational message dialog.
        /// </summary>
        void ShowInfo(string message);
    }
}
