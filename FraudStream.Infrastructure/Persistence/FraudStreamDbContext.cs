using FraudStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FraudStream.Infrastructure.Persistence
{
    public sealed class FraudStreamDbContext : DbContext
    {
        public FraudStreamDbContext(DbContextOptions<FraudStreamDbContext> options) : base(options)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<FraudDecision> FraudDecisions => Set<FraudDecision>();
        public DbSet<FraudRule> FraudRules => Set<FraudRule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FraudStreamDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
