using Microsoft.EntityFrameworkCore;
using SimpleFEM.Core.Models;


namespace SimpleFEM.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Line> Lines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Line>()
                .HasOne(l => l.INode)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Line>()
                .HasOne(l => l.JNode)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }

        // This way the app supports factory-created contexts
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This should not happen with proper factory setup
                throw new InvalidOperationException(
                    "DbContext must be configured with a connection string via the factory.");
            }
        }
    }
}
