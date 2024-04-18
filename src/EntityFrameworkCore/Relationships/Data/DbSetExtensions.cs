using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Relationships.Entities;

namespace Relationships.Data {
    public static class DbSetExtensions {
        public static IQueryable Set(this DbContext context, Type T) {
            context.Set<Address>().FirstOrDefault();
            // Get the generic type definition
            //MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);
            var method = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance).First(m => m.Name == nameof(DbContext.Set) && !m.GetParameters().Any());

            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(T);

            return method.Invoke(context, null) as IQueryable;
        }

        public static IQueryable<T> Set<T>(this DbContext context) {
            // Get the generic type definition 
            MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

            // Build a method with the specific type argument you're interested in 
            method = method.MakeGenericMethod(typeof(T));

            return method.Invoke(context, null) as IQueryable<T>;
        }
    }
}
