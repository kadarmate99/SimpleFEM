using Microsoft.EntityFrameworkCore;
using SimpleFEM.Core.Models;

namespace SimpleFEM.Data.Repositories.EfCore
{
    public class NodeEfRepository : GeneralEfRepository<Node>
    {
        public NodeEfRepository(IDbContextFactory<DataContext> contextFactory) : base(contextFactory)
        {
        }
    }
}
