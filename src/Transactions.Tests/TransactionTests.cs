using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Entities;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using CSharpExamples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CSharpExamples {
    public class TransactionTests {
        private DatabaseContext db;

        public TransactionTests() {
            db = new DatabaseContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetFirstDatabaseIdentityForSecondSave(bool throwException) {
            // Arrange
            var customer = new Customer("Elmer", "Fudd", "elmer@fudd.org");

            // Act
            // strategy is needed because of sql option to retry on failure.  if that is not enabled, a strategy for a user initiated transaction is not needed
            var strategy = db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => {
                await using (var dbContextTransaction =
                             await db.Database.BeginTransactionAsync().ConfigureAwait(false)) {
                    try {
                        db.Customers.Add(customer);
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        if (customer.CustomerId <= 0) {
                            throw new Exception("Customer not saved");
                        }

                        if (throwException) {
                            throw new ApplicationException("should rollback customer");
                        }

                        //... use db id from entity
                        var settings = new DomainEventPublisherSettings() {
                            Service = "shoppingcart",
                            Protocol = "amqps",
                            Namespace = "acme.servicebus.windows.net",
                            Policy = "SendListen",
                            Key = "secret",
                            Topic = "shoppingcart.",
                            Durable = 1,
                            Credits = 5

                        };
                        var publisher = new DomainEventOutboxPublisher<DatabaseContext>(settings, db,
                            NullLogger<DomainEventOutboxPublisher<DatabaseContext>>.Instance);
                        await publisher.PublishAsync(new CustomerCreatedEvent(customer.CustomerId))
                            .ConfigureAwait(false);
                        await db.SaveChangesAsync().ConfigureAwait(false);

                        await dbContextTransaction.CommitAsync().ConfigureAwait(false);
                    } catch (Exception ex) {
                        dbContextTransaction?.RollbackAsync().ConfigureAwait(false);
                        Console.Out.WriteLine(ex.Message);
                    }
                }
            });

            // Assert
            // get new instance to make sure that the entity is not cached
            var db2 = new DatabaseContext();

            var expected = throwException ? 0 : 1;

            Assert.Equal(expected, db2.Customers.Count());
            Assert.Equal(expected, db2.Set<Outbox>().Count());

            if (!throwException) {
                Assert.Equal("{\"CustomerId\":1}", db2.Set<Outbox>().First().Body);
            }
        }
    }
}
