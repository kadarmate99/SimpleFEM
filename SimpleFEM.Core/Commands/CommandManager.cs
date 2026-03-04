namespace SimpleFEM.Core.Commands
{
    public class CommandManager : ICommandManager
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;


        public string? NextUndoDescription =>
            CanUndo ? _undoStack.Peek().Description : null;


        public string? NextRedoDescription =>
            CanRedo ? _redoStack.Peek().Description : null;

        public event EventHandler? CommandHistoryChanged;

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            _undoStack.Push(command);
            _redoStack.Clear();

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            if (!CanUndo) return;

            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Redo()
        {
            if (!CanRedo) return;

            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);

            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            CommandHistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
