using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Relationships.Entities {
    [Table("Supplier")]
    public class Supplier {
        public Supplier(string name) {
            Update(name);
            SupplierResourceId = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; private set; }

        public Guid SupplierResourceId { get; private set; }

        [StringLength(50)]
        public string Name { get; private set; }

        public void Update(string name) {
            // validation

            Name = name;
        }
    }
}
