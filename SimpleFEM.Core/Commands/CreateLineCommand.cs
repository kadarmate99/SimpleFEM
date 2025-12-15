using SimpleFEM.Core.Interfaces;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Core.Commands
{
    public class CreateLineCommand : ICommand
    {
        private readonly IRepository<Line> _lineRepository; // receiver

        // Context data
        private readonly Node _start;
        private readonly Node _end;

        public Line? CreatedLine { get; private set; }

        public string Description => $"Create Line from Node {_start.Id} to Node {_end.Id}";

        public CreateLineCommand(IRepository<Line> lineRepository, Node start, Node end)
        {
            _lineRepository = lineRepository;
            _start = start;
            _end = end;
        }

        public void Execute()
        {
            var line = new Line
            {
                INode = _start,
                JNode = _end
            };

            _lineRepository.Add(line);
            CreatedLine = line;
        }

        public void Undo()
        {
            if (CreatedLine == null)
                throw new InvalidOperationException("Cannot undo: Line was not created.");
            
            _lineRepository.Delete(CreatedLine.Id);
        }
    }
}
