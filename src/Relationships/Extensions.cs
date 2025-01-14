using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CSharpExamples;

namespace CSharpExamples {
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
            var parameters = SortField.Parse(sortParameters);

            var expression = query.Expression;
            for (var index = 0; index < parameters.Length; index++) {
                var sortParameter = parameters[index];
                //var method = sortParameter.GetOrderMethod(index);

                var propertyLambda = GetPropertyLambda<T>(sortParameter.Property, sortParameter.Arguments);

                var exp = propertyLambda.Item1;
                var t = propertyLambda.Item2;

                var method = sortParameter.GetOrderMethod(index);
                if (t.Name.Contains("nullable", StringComparison.CurrentCultureIgnoreCase)) {
                    // first add an expression where we order by the property.HasValue
                    var nullCheckProperty = GetPropertyLambda<T>($"{sortParameter.Property}.HasValue", sortParameter.Arguments);
                    expression = Expression.Call(typeof(Queryable), method, new Type[] { query.ElementType, nullCheckProperty.Item2 },
                        expression,
                        Expression.Quote(nullCheckProperty.Item1));

                    // since we may have taken the first order position by inserting this new one
                    // update the order method
                    //method = sortParameter.GetOrderMethod(index + 1);
                }

                expression = Expression.Call(typeof(Queryable), method,
                    new Type[] { query.ElementType, t },
                    expression, Expression.Quote(exp));
            }
            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(expression);
        }

        private static (Expression, Type) GetPropertyLambda<T>(string propertyName, string arguments) {
            var parameterExpression = Expression.Parameter(typeof(T), "x");

            Expression body = parameterExpression;
            var propertyNames = propertyName.Split('.');

            Type type = typeof(T);
            var pname = propertyName;
            foreach (var name in propertyNames) {
                body = Expression.PropertyOrField(body, name);

                var propertyInfo = type.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) {
                    throw new ArgumentException("Sort property does not exist!");
                }

                type = propertyInfo.PropertyType;
                pname = name;
            }

            var propertyExpression = body;
            if (type.IsEnum && string.Equals("ordinal", arguments, StringComparison.InvariantCultureIgnoreCase)) {
                body = GetExpressionForEnumOrdering<T>(type, propertyExpression);
                type = typeof(int);
            } else if (type == typeof(bool) && string.Equals("truefirst", arguments, StringComparison.InvariantCultureIgnoreCase)) {
                body = GetExpressionForBooleanOrdering(propertyExpression);
                type = typeof(int);
            } else if (type == typeof(string) && !string.IsNullOrWhiteSpace(arguments)) {
                body = GetExpressionForStringOrdering(propertyExpression, arguments);
                type = typeof(int);
            }

            var conv = Expression.Convert(body, type);
            var exp = Expression.Lambda(conv, parameterExpression);
            return (exp, type);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/40202415/order-by-enum-description
        /// </summary>
        /// <param name="parameterExpression"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private static Expression GetExpressionForStringOrdering(Expression parameterExpression, string arguments) {
            var orderedValues = arguments.Split(';');
            var body = orderedValues.Select((value, ordinal) => new { value, ordinal })
                .Reverse()
                .Aggregate((Expression)Expression.Constant(orderedValues.Length), (next, item) =>
                    Expression.Condition(
                        Expression.Equal(parameterExpression,
                            Expression.Constant(item.value)),
                        Expression.Constant(item.ordinal),
                        next));

            return body;
        }

        private static Expression GetExpressionForEnumOrdering<TSource>(Type type, Expression parameterExpression) {
            if (!type.IsEnum) {
                throw new InvalidOperationException();
            }

            var orderedValues = ((int[])Enum.GetValues(type)).OrderBy(value => value);
            // TODO: value is the int value of the enum instead of the description -- which is unimportant as sorting by the enum in the database is just the column for string
            var body = orderedValues.Select((value, ordinal) => new { value, ordinal })
                .Reverse()
                .Aggregate((Expression)null, (next, item) => next == null ?
                    Expression.Constant(item.ordinal) :
                    Expression.Condition(
                        Expression.Equal(parameterExpression, Expression.Convert(Expression.Constant(item.value), type)),
                        Expression.Constant(item.ordinal),
                        next));

            return body;
        }

        private static Expression GetExpressionForBooleanOrdering(Expression propertyExpression) {
            var body = Expression.Condition(propertyExpression,
                Expression.Constant(0),
                Expression.Constant(1));

            return body;
        }

        private static Expression GetExpressionForIndexOfOrdering(Expression parameterExpression, string arguments) {
            // TODO: string.IndexOf does not translate to sql
            var method = typeof(string).GetMethod("IndexOf", [typeof(string), typeof(StringComparison)]);

            Expression[] parms = [parameterExpression, Expression.Constant(StringComparison.OrdinalIgnoreCase)];
            var call = Expression.Call(Expression.Constant(arguments), method, parms);

            return call;
        }
    }
}
