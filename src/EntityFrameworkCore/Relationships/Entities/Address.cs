using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Relationships.Entities {
    [Table("Address")]
    public class Address : IValueObject {
        protected Address() {
        }

        public Address(string street, string city, string state, string country, string zipcode) {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        // TODO: using internal instead of private so that context can set it
        public int AddressId { get; internal set; }

        [StringLength(125)]
        public string UniqueKey { get => GetUniqueKey(this); set => _ = value; }

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

        public static string GetUniqueKey(Address address) {
            if (address == null) {
                return "null";
            }

            var sb = new StringBuilder();
            sb.Append(address.Street ?? string.Empty).Append('|');
            sb.Append(address.City ?? string.Empty).Append('|');
            sb.Append(address.State ?? string.Empty).Append('|');
            sb.Append(address.ZipCode ?? string.Empty).Append('|');
            sb.Append(address.Country ?? string.Empty).Append('|');

            return sb.ToString();
        }
    }
}
