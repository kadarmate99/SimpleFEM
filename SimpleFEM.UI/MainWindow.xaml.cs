using SimpleFEM.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SimpleFEM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            var element = (IInputElement)sender;
            var position = e.GetPosition(element);

            if (DataContext is MainViewModel vm)
            {
                vm.CanvasClickCommand.Execute(position);
            }
        }
    }
}