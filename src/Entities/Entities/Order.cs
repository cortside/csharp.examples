using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace Common.Entities {
    [Index(nameof(OrderResourceId), IsUnique = true)]
    [Table("Order")]
    [Comment("Orders")]
    public class Order {
        protected Order() {
            // Required by EF as it doesn't know about Customer
        }

        public Order(Customer customer, string street, string city, string state, string country, string zipCode) {
            OrderResourceId = Guid.NewGuid();
            Customer = customer;
            Address = new Address(street, city, state, country, zipCode);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("Primary Key")]
        public int OrderId { get; private set; }

        [Comment("Public unique identifier")]
        public Guid OrderResourceId { get; private set; }

        [Column(TypeName = "nvarchar(20)")]
        [Comment("Order status (created, paid, shipped, cancelled)")]
        public OrderStatus Status { get; private set; }

        [ForeignKey("CustomerId")]
        [Comment("FK to Customer")]
        public Customer Customer { get; private set; }

        [Comment("FK to Address")]
        [ForeignKey("AddressId")]
        public Address Address { get; private set; }

        [Comment("Date customer was last notified for order")]
        public DateTime? LastNotified { get; private set; }

        [Comment("Was customer notified for order")]
        public bool Notified { get; private set; }

        [Comment("Customer categorization")]
        public string Category { get; private set; }

        // expose items as a read only collection so that the collection cannot be manipulated without going through order
        private readonly List<OrderItem> items = [];
        public virtual IReadOnlyList<OrderItem> Items => items;

        public void AddItem(Item item, int quantity) {
            // validation goes here

            items.Add(new OrderItem(item, quantity));
        }

        public void UpdateAddress(string street, string city, string state, string country, string zipCode) {
            // validation goes here

            Address.Update(street, city, state, country, zipCode);
        }
    }
}
