using Microsoft.Win32;
using System.Windows;

namespace SimpleFEM.UI.Services
{
    /// <summary>
    /// Implementation of <see cref="IDialogService"/> using Win32 dialogs and MessageBox.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <inheritdoc/>
        public string? ShowSaveFileDialog(string title, string filter, string defaultExtension)
        {
            var dialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                DefaultExt = defaultExtension,
                AddExtension = true,
            };

            if (dialog.ShowDialog() == false)
                return null;

            return dialog.FileName;
        }

        /// <inheritdoc/>
        public string? ShowOpenFileDialog(string title, string filter, string defaultExtension)
        {
            var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                DefaultExt = defaultExtension,
                AddExtension = true,
            };

            if (dialog.ShowDialog() == false)
                return null;

            return dialog.FileName;
        }

        /// <inheritdoc/>
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc/>
        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
