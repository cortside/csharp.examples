using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Relationships.Entities {
    [Table("OrderItem")]
    [Comment("Items that belong to an Order")]
    public class OrderItem {
        protected OrderItem() {
            // Required by EF as it doesn't know about CatalogItem
        }

        public OrderItem(string sku, int quantity, decimal unitPrice) {
            // do validation here

            Sku = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("Primary Key")]
        public int OrderItemId { get; private set; }

        /// <summary>
        /// OrderId added explicitly here so that it does not become nullable when inferred by relationships
        /// </summary>
        [ForeignKey("OrderId")]
        public int OrderId { get; private set; }

        [StringLength(10)]
        [Comment("Item Sku")]
        public string Sku { get; private set; }

        [Comment("Quantity of Sku")]
        public int Quantity { get; private set; }

        [Column(TypeName = "money")]
        [Comment("Per quantity price")]
        public decimal UnitPrice { get; private set; }
    }
}
