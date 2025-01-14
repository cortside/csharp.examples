using System;
using System.Linq;

namespace CSharpExamples {
    public class SortField {
        public SortField(string sortParameter) {
            string direction = sortParameter.Trim()[..1];
            SortDirection = string.Equals(direction, "-", StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending;

            var property = string.Equals(direction, "-", StringComparison.OrdinalIgnoreCase)
                ? sortParameter[1..]
                : sortParameter;

            if (property.Contains('|')) {
                var parts = property.Split('|');
                Property = parts[0];
                Arguments = parts[1];
            } else {
                Property = property;
            }
        }

        public string Property { get; private set; }
        public SortDirection SortDirection { get; private set; }
        public string Arguments { get; private set; }

        public static SortField[] Parse(string sortParameters) {
            var s = sortParameters.Replace("+", "");
            var parameters = s.Split(',').Where(x => x.Trim().Length > 0).Select(x => new SortField(x)).ToArray();
            return parameters;
        }

        public string GetOrderMethod(int index) {
            if (SortDirection == SortDirection.Descending) {
                if (index == 0) {
                    return "OrderByDescending";
                } else {
                    return "ThenByDescending";
                }
            } else if (index == 0) {
                return "OrderBy";
            } else {
                return "ThenBy";
            }
        }
    }
}
