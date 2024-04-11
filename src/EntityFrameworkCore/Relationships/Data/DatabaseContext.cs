using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Relationships.Entities;

namespace Relationships.Data {
    public class DatabaseContext : DbContext {
        private readonly ILoggerFactory loggerFactory;

        public DatabaseContext() {
            this.loggerFactory = new NullLoggerFactory();
        }

        //public DatabaseContext(ILoggerFactory loggerFactory) {
        //    this.loggerFactory = loggerFactory;
        //}

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            var connectionString = "Data Source=.;Initial Catalog=EFCore;Integrated Security=True;TrustServerCertificate=true;";
            optionsBuilder.UseSqlServer(connectionString, sqlOptions => {
                // instruct ef to use multiple queries instead of large joined queries
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            optionsBuilder.UseLoggerFactory(loggerFactory);

            // NOT safe for production
            optionsBuilder.EnableSensitiveDataLogging(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            DisableCascadeDelete(modelBuilder);
        }

        protected static void DisableCascadeDelete(ModelBuilder modelBuilder) {
            var fks = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetDeclaredForeignKeys());
            foreach (var fk in fks) {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
