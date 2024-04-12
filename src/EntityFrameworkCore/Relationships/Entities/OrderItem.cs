using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Relationships.Entities {
    [Table("OrderItem")]
    [Comment("Items that belong to an Order")]
    public class OrderItem {
        protected OrderItem() {
            // Required by EF as it doesn't know about Item
        }

        public OrderItem(Item item, int quantity) {
            // do validation here

            Item = item;
            Quantity = quantity;
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

        /// <summary>
        /// Use of Required attribute to make sure that the inferred ItemId is not nullable
        /// </summary>
        [Required]
        [ForeignKey("ItemId")]
        public Item Item { get; private set; }

        [Comment("Quantity of Sku")]
        public int Quantity { get; private set; }
    }
}
