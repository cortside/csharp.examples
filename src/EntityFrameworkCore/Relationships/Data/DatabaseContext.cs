using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Relationships.Entities;

namespace Relationships.Data {
    public class DatabaseContext : DbContext {
        private readonly ILoggerFactory loggerFactory;

        public DatabaseContext(ILoggerFactory loggerFactory = null) {
            this.loggerFactory = loggerFactory ?? new NullLoggerFactory();
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Item> Items { get; set; }
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

            // unidirectional many to many without suppliers having an items property
            modelBuilder.Entity<Item>()
                .HasMany(e => e.Suppliers)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "ItemSupplier",
                    j => j
                        .HasOne<Supplier>()
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("FK_ItemSupplier_SupplierId")
                        .OnDelete(DeleteBehavior.NoAction),
                    j => j
                        .HasOne<Item>()
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .HasConstraintName("FK_ItemSupplier_ItemId")
                        .OnDelete(DeleteBehavior.NoAction)
                    );
        }

        protected static void DisableCascadeDelete(ModelBuilder modelBuilder) {
            var fks = modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetDeclaredForeignKeys());
            foreach (var fk in fks) {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
