using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Relationships.Data;
using Relationships.Entities;

namespace Relationships {
    /// <summary>
    /// Program
    /// </summary>
    public static class Program {
        public static Task<int> Main(string[] args) {
            //var loggerFactory = LoggerFactory.Create(builder => {
            //    builder
            //        .AddFilter((category, level) =>
            //            category == DbLoggerCategory.Database.Command.Name
            //            && level == LogLevel.Information)
            //        .AddConsole();
            //});
            //var logger = loggerFactory.CreateLogger<Program>();

            Example1();

            return Task.FromResult(0);
        }

        private static void Example1() {
            using (var db = new DatabaseContext()) {
                // Create
                Console.WriteLine("Inserting a new Order");
                db.Add(new Order(new Customer("Elmer", "Fudd", "elmer@fudd.org"), "123 Main", "Salt Lake City:", "UT", "USA", "84123-1000"));
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for an Order");
                var order = db.Orders
                    .Include(x => x.Address)
                    .Include(x => x.Items)
                    .OrderBy(b => b.OrderId)
                    .First();

                // Update and add an item
                Console.WriteLine("Updating the order and adding an item");
                order.UpdateAddress("234 State", "Salt Lake City:", "UT", "USA", "84123-1000");
                order.AddItem("ABC123", 1, 1.99M);
                db.SaveChanges();

                // Add another item
                Console.WriteLine("Adding another item");
                order.AddItem("DEF456", 2, 2.97M);
                db.SaveChanges();

                Console.WriteLine("there are " + db.Orders.Count().ToString() + " orders in the db");
                Console.WriteLine("there are " + db.Customers.Count().ToString() + " customers in the db");

                // Delete -- explicitly deleting the children first
                Console.WriteLine("Delete the order");
                db.RemoveRange(order.Items);
                db.Remove(order);
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    Console.WriteLine("can't delete blog without deleting all posts first, violation of FK");
                }

                Console.WriteLine("there are " + db.Orders.Count().ToString() + " orders in the db");
                Console.WriteLine("there are " + db.Customers.Count().ToString() + " customers in the db");
            }
        }
    }
}
