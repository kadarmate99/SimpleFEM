using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace SimpleFEM.UI.ViewModels
{
    /// <summary>
    /// Base class for ViewModels that can be used as navigation targets by <see cref="INavigationService"/>.
    /// </summary>
    public class NavigableViewModelBase : ObservableObject
    {
        /// <summary>
        /// Called by the <see cref="INavigationService"/> after this ViewModel
        /// becomes the active view. 
        /// </summary>
        /// <remarks>
        /// Override to load data, subscribe to events, etc...
        /// </remarks>
        public virtual Task OnNavigatedToAsync() => Task.CompletedTask;

        // Called by the NavigationService just before this ViewModel
        // is replaced. Override to cancel operations, unsubscribe from
        // events, or save transient state.
        /// <summary>
        /// Called by the <see cref="INavigationService"/> before this ViewModel
        /// is replaced from the active view. 
        /// </summary>
        /// <remarks>
        /// Override to cancel operations, unsubscribe from events, etc...
        /// </remarks>
        public virtual Task OnNavigatedFromAsync() => Task.CompletedTask;
    }
}
