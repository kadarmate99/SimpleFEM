namespace SimpleFEM.Core.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Executes the command's action.
        /// </summary>
        void Execute();

        /// <summary>
        /// Reverses the command's action, restoring previous state.
        /// </summary>
        void Undo();

        /// <summary>
        /// A description of what this command does.
        /// </summary>
        string Description { get; }
    }
}
