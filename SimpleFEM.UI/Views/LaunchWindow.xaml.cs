using SimpleFEM.UI.ViewModels;
using System.Windows;

namespace SimpleFEM.UI.Views
{
    /// <summary>
    /// Interaction logic for LaunchWindow.xaml
    /// </summary>
    public partial class LaunchWindow : Window
    {
        private readonly LaunchViewModel _viewModel;

        public LaunchWindow(LaunchViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
