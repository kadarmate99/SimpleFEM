using SimpleFEM.Core.Results;
using SimpleFEM.Domain.Entities;

namespace SimpleFEM.Core
{
    public interface ISolver
    {
        AnalysisResult Solve(Structure structure, LoadCase loadCase);
    }
}
