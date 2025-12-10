
using System.Windows;

namespace SimpleFEM.Core.Tools
{
    public interface IDrawingTool
    {
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the tool is currently selected and listening for input.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Activate"/> or <see cref="Deactivate"/> to change the <see cref="IsActive"/> state.
        /// </remarks>
        bool IsActive { get; }

        /// <summary>
        /// Indicates whether the tool is in the middle of a multi-step definition sequence 
        /// (e.g., waiting for the second click of a line).
        /// </summary>
        bool InProgress { get; }

        /// <summary>
        /// Activates the tool, allowing it to begin processing user input. 
        /// </summary>
        void Activate();

        /// <summary>
        /// Calls <see cref="Reset()"/> than deactivates the tool, preventing it from processing user input. 
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Clears any transient or half-finished state of the tool without deactivating it.
        /// </summary>
        void Reset();

        /// <summary>
        /// Handles a single mouse click event at the specified canvas location 
        /// if the tool is active (indicated by <see cref="IsActive"/>).
        /// </summary>
        /// <param name="location">The canvas coordinates of the click.</param>
        void HandleCanvasClick(Point location);

        /// <summary>
        /// An event fired whenever the internal state of the tool changes (e.g., IsActive, IsComplete, 
        /// or a temporary entity is created/deleted).
        /// </summary>
        event EventHandler? StateChanged;
    }
}
