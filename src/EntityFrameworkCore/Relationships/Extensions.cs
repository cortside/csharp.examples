using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Relationships {
    public enum SortDirection {
        Ascending,
        Descending
    }
    public class SortField {
        public string FieldName { get; internal set; }
        public SortDirection SortDirection { get; internal set; }
    }

    public static class Extensions {

        public static IQueryable<T> ToPagedQuery<T>(this IQueryable<T> query, int page, int pageSize, int skipAdditionalRowsCount = 0) {
            return query.Skip(pageSize * (page - 1) + skipAdditionalRowsCount).Take(pageSize);
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool orderByDescending) {
            return orderByDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
        }

        /// <summary>
        /// the sortParameters must have the next format:
        /// if want to sort by descending use '-', for example: -ColumnName, if want to sort ascending use just the name of the column, for example: ColumnName
        /// if want to sot by multiple parameters separate them with a comma, for example ColumnName1,ColumnName2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">the IQueryable send from the persistence class</param>
        /// <param name="sortParameters">the name of the sort parameters</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ToSortedQuery<T>(this IQueryable<T> query, string sortParameters) {
            sortParameters = sortParameters.Replace("+", "");
            string[] parameters = sortParameters.Split(',');

            var expression = query.Expression;
            for (var index = 0; index < parameters.Length; index++) {
                var sortParameter = parameters[index];
                if (string.IsNullOrEmpty(sortParameter)) {
                    continue;
                }
                string orderType = sortParameter[..1];

                var method = GetOrderMethod(index, orderType);

                string propertyName = string.Equals(orderType, "-", StringComparison.OrdinalIgnoreCase)
                    ? sortParameter[1..]
                    : sortParameter;

                var propertyLambda = GetPropertyLambda<T>(propertyName);

                var exp = propertyLambda.Item1;
                var t = propertyLambda.Item2;

                //if (t.IsEnum) {
                //    t = typeof(int);
                //    exp = GetExpressionForEnumOrdering<T>(exp);
                //} else if (t == typeof(bool)) {
                //    var queryParameterExpression = Expression.Parameter(typeof(T), "x");
                //    t = typeof(string);
                //    exp = GetExpressionForBoolOrdering(exp, queryParameterExpression);
                //} else

                if (t.Name.Contains("nullable", StringComparison.CurrentCultureIgnoreCase)) {
                    // first add an expression where we order by the property.HasValue
                    var nullCheckProperty = GetPropertyLambda<T>($"{propertyName}.HasValue");
                    expression = Expression.Call(typeof(Queryable), method, new Type[] { query.ElementType, nullCheckProperty.Item2 },
                        expression,
                        Expression.Quote(nullCheckProperty.Item1));

                    // since we may have taken the first order position by inserting this new one
                    // update the order method
                    method = GetOrderMethod(index + 1, orderType);
                }

                expression = Expression.Call(typeof(Queryable), method,
                    new Type[] { query.ElementType, t },
                    expression, Expression.Quote(exp));
            }
            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(expression);
        }

        private static string GetOrderMethod(int index, string orderType) {
            if (string.Equals(orderType, "-", StringComparison.OrdinalIgnoreCase)) {
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

        private static (Expression, Type) GetPropertyLambda<T>(string propertyName) {
            var arg = Expression.Parameter(typeof(T), "x");
            if (propertyName.Contains('.')) {
                Expression body = arg;
                var members = propertyName.Split('.');

                Type propertyType = typeof(T);
                foreach (var subMember in members) {
                    body = Expression.PropertyOrField(body, subMember);

                    var propertyInfo = propertyType.GetProperty(subMember);
                    if (propertyInfo == null) {
                        throw new ArgumentException("Sort property does not exist!");
                    }

                    propertyType = propertyInfo.PropertyType;
                }

                var conv = Expression.Convert(body, propertyType);
                var exp = Expression.Lambda(conv, arg);
                return (exp, propertyType);
            } else {
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) {
                    throw new ArgumentException("Sort property does not exist!");
                }

                var propertyType = propertyInfo.PropertyType;
                var property = Expression.Property(arg, propertyName);

                //return the property as object
                var conv = Expression.Convert(property, propertyType);
                var exp = Expression.Lambda(conv, arg);
                return (exp, propertyType);
            }
        }

        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, SortField sortField) {
            var queryParameterExpression = Expression.Parameter(typeof(T), "x");
            var orderByPropertyExpression = GetPropertyExpression(sortField.FieldName, queryParameterExpression);

            Type orderByPropertyType = orderByPropertyExpression.Type;
            LambdaExpression lambdaExpression = Expression.Lambda(orderByPropertyExpression, queryParameterExpression);

            if (orderByPropertyType.IsEnum) {
                orderByPropertyType = typeof(int);
                lambdaExpression = GetExpressionForEnumOrdering<T>(lambdaExpression);
            } else if (orderByPropertyType == typeof(bool)) {
                orderByPropertyType = typeof(string);
                lambdaExpression = GetExpressionForBoolOrdering(orderByPropertyExpression, queryParameterExpression);
            }

            var orderByExpression = Expression.Call(
                typeof(Queryable),
                sortField.SortDirection == SortDirection.Ascending ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), orderByPropertyType },
                query.Expression,
                Expression.Quote(lambdaExpression));

            return query.Provider.CreateQuery<T>(orderByExpression);
        }

        private static MemberExpression GetPropertyExpression(string propertyName, ParameterExpression queryParameterExpression) {
            MemberExpression result = Expression.Property(queryParameterExpression, propertyName);
            return result;
        }

        private static Expression<Func<TSource, int>> GetExpressionForEnumOrdering<TSource>(LambdaExpression source) {
            var enumType = source.Body.Type;
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            var body = ((int[])Enum.GetValues(enumType))
                .OrderBy(value => (int)value) // GetEnumDescription(value, enumType))
                .Select((value, ordinal) => new { value, ordinal })
                .Reverse()
                .Aggregate((Expression)null, (next, item) => next == null ? (Expression)
                    Expression.Constant(item.ordinal) :
                    Expression.Condition(
                        Expression.Equal(source.Body, Expression.Convert(Expression.Constant(item.value), enumType)),
                        Expression.Constant(item.ordinal),
                        next));

            return Expression.Lambda<Func<TSource, int>>(body, source.Parameters[0]);
        }

        private static LambdaExpression GetExpressionForBoolOrdering(MemberExpression orderByPropertyExpression, ParameterExpression queryParameterExpression) {
            var firstWhenActiveExpression = Expression.Condition(orderByPropertyExpression,
                Expression.Constant("A"),
                Expression.Constant("Z"));

            return Expression.Lambda(firstWhenActiveExpression, new[] { queryParameterExpression });
        }

        private static string GetEnumDescription(int value, Type enumType) {
            if (!enumType.IsEnum)
                throw new InvalidOperationException();

            var name = Enum.GetName(enumType, value);
            var field = enumType.GetField(name, BindingFlags.Static | BindingFlags.Public);
            return field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
        }
    }
}
