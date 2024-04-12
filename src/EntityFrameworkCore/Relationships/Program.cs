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
            Example1();
            return Task.FromResult(0);
        }

        private static void Example1() {
            using (var db = new DatabaseContext()) {
                SeedSuppliers(db);
                SeedItems(db);

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
                var item = db.Items.First(x => x.Sku == "ABC123");
                order.AddItem(item, 1);
                db.SaveChanges();

                // Add another item
                Console.WriteLine("Adding another item");
                item = db.Items.First(x => x.Sku == "DEF456");
                order.AddItem(item, 2);
                db.SaveChanges();

                Console.WriteLine("there are " + db.Orders.Count().ToString() + " orders in the db");
                Console.WriteLine("there are " + db.Customers.Count().ToString() + " customers in the db");

                // Delete -- explicitly deleting the children first
                Console.WriteLine("Delete the order");
                db.RemoveRange(order.Items);
                db.Remove(order);
                db.SaveChanges();

                Console.WriteLine("there are " + db.Orders.Count().ToString() + " orders in the db");
                Console.WriteLine("there are " + db.Customers.Count().ToString() + " customers in the db");
            }
        }

        private static void SeedItems(DatabaseContext db) {
            // add items
            if (db.Items.Any()) {
                return;
            }

            var item = db.Add(new Item("abc123", "item #1", 1.99M));
            item.Entity.AddSupplier(db.Suppliers.OrderBy(x => x.SupplierId).First());

            item = db.Add(new Item("def456", "item #2", 2.97M));
            item.Entity.AddSupplier(db.Suppliers.OrderBy(x => x.SupplierId).First());
            item.Entity.AddSupplier(db.Suppliers.OrderBy(x => x.SupplierId).Last());

            item = db.Add(new Item("xyz789", "item #3", 3.98M));
            item.Entity.AddSupplier(db.Suppliers.OrderBy(x => x.SupplierId).Last());

            db.SaveChanges();
        }

        private static void SeedSuppliers(DatabaseContext db) {
            // add suppliers
            if (db.Suppliers.Any()) {
                return;
            }

            db.Add(new Supplier("Acme"));
            db.Add(new Supplier("Morley"));
            db.SaveChanges();
        }
    }
}
