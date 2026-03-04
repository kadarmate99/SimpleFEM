namespace SimpleFEM.Core.Commands
{
    /// <summary>
    /// Manages command execution and undo/redo history.
    /// </summary>
    public interface ICommandManager
    {
        /// <summary>
        /// Indicates whether there is a command available to undo.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Indicates whether there is a command available to redo.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// The description of the next command that would be redone.
        /// </summary>
        string? NextRedoDescription { get; }

        /// <summary>
        /// The description of the next command that would be undone.
        /// </summary>
        string? NextUndoDescription { get; }

        /// <summary>
        /// Fired whenever the command history changes.
        /// </summary>
        event EventHandler? CommandHistoryChanged;

        /// <summary>
        /// Clears all command history.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Executes a command and adds it to the undo history.
        /// Clears the redo stack (can't redo after new actions).
        /// </summary>
        void ExecuteCommand(ICommand command);

        /// <summary>
        /// Redoes the most recently undone command.
        /// </summary>
        void Redo();

        /// <summary>
        /// Undoes the most recent command.
        /// </summary>
        void Undo();
    }
}