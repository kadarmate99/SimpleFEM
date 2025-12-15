namespace SimpleFEM.Core.Commands
{
    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// Gets the description of the next command that would be undone.
        /// </summary>
        public string? NextUndoDescription =>
            CanUndo ? _undoStack.Peek().Description : null;

        /// <summary>
        /// Gets the description of the next command that would be redone.
        /// </summary>
        public string? NextRedoDescription =>
            CanRedo ? _redoStack.Peek().Description : null;

        /// <summary>
        /// Fired whenever the command history changes (execute/undo/redo/clear).
        /// </summary>
        public event EventHandler? CommandHistoryChanged;

        /// <summary>
        /// Executes a command and adds it to the undo history.
        /// Clears the redo stack (can't redo after new actions).
        /// </summary>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            _undoStack.Push(command);
            _redoStack.Clear();

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Undoes the most recent command.
        /// </summary>
        public void Undo()
        {
            if (!CanUndo) return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Redoes the most recently undone command.
        /// </summary>
        public void Redo()
        {
            if (!CanRedo) return;

            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clears all command history.
        /// </summary>
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
