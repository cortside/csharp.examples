using System;
using System.Linq;
using Common.Enumerations;
using Xunit;

namespace Relationships.Tests {
    public class UnitTest1 {
        [Fact]
        public void Test1() {
            var propertyName = "foo";

            var propertyNames = propertyName.Split('.');

            Assert.Equal(1, propertyNames.Length);
        }

        [Fact]
        public void Test2() {
            var propertyName = "foo.bar";

            var propertyNames = propertyName.Split('.');

            Assert.Equal(2, propertyNames.Length);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/40202415/order-by-enum-description
        /// </summary>
        [Fact]
        public void Test3() {
            var values = (OrderStatus[])Enum.GetValues(typeof(OrderStatus));
            var list = values.Select(x => new { Value = x.ToString(), Ordinal = (int)x });

            var s = string.Join(',', list.Select(x => x.Value));
            Assert.Equal("Created,Paid,Shipped,Cancelled", s);
        }
    }
}
