using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Entities;
using Cortside.DomainEvent;
using Cortside.DomainEvent.EntityFramework;
using CSharpExamples.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

// db context from uowcontext
// with outbox
// customer entity
// publish message with customerId
// test with theory
// check that message is published with customer created
// check that no customer, no message

// should this be in shoppingcart-api???

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
            using (var dbContextTransaction = db.Database.BeginTransaction()) {
                try {
                    db.Customers.Add(customer);
                    await db.SaveChangesAsync();

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
                    var publisher = new DomainEventOutboxPublisher<DatabaseContext>(settings, db, NullLogger<DomainEventOutboxPublisher<DatabaseContext>>.Instance);
                    await publisher.PublishAsync(new CustomerCreatedEvent(customer.CustomerId));
                    await db.SaveChangesAsync();

                    dbContextTransaction.Commit();
                } catch (Exception ex) {
                    dbContextTransaction?.Rollback();
                    Console.Out.WriteLine(ex.Message);
                }
            }

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
