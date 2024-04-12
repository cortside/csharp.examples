using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Relationships.Entities {
    [Table("Item")]
    public class Item {
        public Item(string sku, string description, decimal unitPrice) {
            Update(sku, description, unitPrice);
            ItemResourceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; private set; }

        public Guid ItemResourceId { get; private set; }

        [StringLength(50)]
        public string Sku { get; private set; }
        [StringLength(50)]
        public string Description { get; private set; }
        [StringLength(250)]
        public decimal UnitPrice { get; private set; }

        // expose items as a read only collection so that the collection cannot be manipulated without going through order
        private readonly List<Supplier> suppliers = [];
        public virtual IReadOnlyList<Supplier> Suppliers => suppliers;

        public void Update(string sku, string description, decimal unitPrice) {
            // validation

            Sku = sku;
            Description = description;
            UnitPrice = unitPrice;
        }

        public void AddSupplier(Supplier supplier) {
            suppliers.Add(supplier);
        }
    }
}
