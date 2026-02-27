using SimpleFEM.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleFEM.UI.Views
{
    /// <summary>
    /// Interaction logic for CanvasView.xaml
    /// </summary>
    public partial class CanvasView : UserControl
    {
        public CanvasView()
        {
            InitializeComponent();
        }

        private void OnCanvasClick(object sender, MouseButtonEventArgs e)
        {
            var element = (IInputElement)sender;
            var position = e.GetPosition(element);

            if (DataContext is CanvasViewModel vm)
            {
                vm.CanvasClickCommand.Execute(position);
            }
        }
    }
}