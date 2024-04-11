using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Relationships.Entities {
    [Table("Address")]
    public class Address {
        protected Address() { }

        public Address(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; private set; }

        [StringLength(50)]
        public string Street { get; private set; }
        [StringLength(50)]
        public string City { get; private set; }
        [StringLength(2)]
        public string State { get; private set; }
        [StringLength(3)]
        public string Country { get; private set; }
        [StringLength(10)]
        public string ZipCode { get; private set; }

        /// <summary>
        /// Updates the specified street.
        /// </summary>
        /// <param name="street">The street.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="country">The country.</param>
        /// <param name="zipcode">The zipcode.</param>
        internal void Update(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }
    }
}
