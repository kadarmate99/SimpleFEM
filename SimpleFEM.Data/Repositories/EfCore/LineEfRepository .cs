using Microsoft.EntityFrameworkCore;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Data.Repositories.EfCore
{
    public class LineEfRepository : GeneralEfRepository<Line>
    {
        public LineEfRepository(IDbContextFactory<DataContext> contextFactory) 
            : base(contextFactory) { }

        public override IEnumerable<Line> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();

            return context.Set<Line>()
                .Include(l => l.INode)
                .Include(l => l.JNode)
                .ToList();
        }

        public override Line? GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();

            return context.Set<Line>()
                .Include(l => l.INode)
                .Include(l => l.JNode)
                .FirstOrDefault(l => l.Id == id);
        }

        public override void Add(Line model)
        {
            using var context = _contextFactory.CreateDbContext();

            // Attach nodes to prevent duplicates
            context.Attach(model.INode);
            context.Attach(model.JNode);

            context.Set<Line>().Add(model);
            context.SaveChanges();
        }
    }
}
