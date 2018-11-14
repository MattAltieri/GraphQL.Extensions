using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    internal static class SlicerDynamicParser {

        private static MethodInfo stringFormatMethod;
        private static Dictionary<Type, MethodInfo> toStringMethods;

        static SlicerDynamicParser() {
            stringFormatMethod = typeof(string).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Format")
                .Select(m => new {
                    Method = m,
                    Parameters = m.GetParameters()
                })
                .Where(m => m.Parameters.Count() == 2
                    && m.Parameters[0].ParameterType == typeof(string)
                    && m.Parameters[1].GetCustomAttribute(typeof(ParamArrayAttribute), false) != null)
                .Select(m => m.Method)
                .First();

            toStringMethods = PrimativeTypes.Select(t => new {
                Key = t,
                Value = t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "ToString" && m.GetParameters().Count() == 0)
                    .First()
            })
            .ToDictionary(k => k.Key, v => v.Value);
        }
        
        internal static Expression<Func<TEntity>> InjectCursorIntoSelector<TEntity>(
            Expression<Func<TEntity>> selector,
            Slicer slicer)
            where TEntity : class, new() {

            
        }

        private static Expression StringFormatExpression(string formatString, params object[] args) {
            return Expression.Call(
                stringFormatMethod,
                Expression.Constant(formatString),
                Expression.NewArrayInit(typeof(object), args.Select(a => Expression.Constant(a))));
        }

        private static Expression DateTimeCursorPart(MemberExpression memberExpression) {

            MemberExpression ticks = Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetProperty("Ticks"));
            return PrimativeCursorPart(typeof(long), ticks);
        }

        private static Expression PrimativeCursorPart(Type type, MemberExpression memberExpression) {
            if (!PrimativeTypes.Contains(type))
                throw new ArgumentException($"{nameof(type)} must be a primative.");

            // MethodInfo toString = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            //     .Where(m => m.Name == "ToString" && m.GetParameters().Count() == 0)
            //     .First();
            return Expression.Call(memberExpression, toStringMethods[type]);
        }

        private static Expression NullableCursorPart(Type type, MemberExpression memberExpression) {
            if (!type.IsValueType)
                throw new ArgumentException($"{nameof(type)} must be a value type.");
            
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null)
                throw new ArgumentException($"{nameof(type)} must be Nullable<>.");

            MemberExpression propertyHasValue = Expression.Property(memberExpression, type.GetProperty("HasValue"));
            MemberExpression propertyValue = Expression.Property(memberExpression, type.GetProperty("Value"));

            MethodCallExpression methodCallExpression;
            if (underlyingType == typeof(DateTime))
                methodCallExpression = (MethodCallExpression)DateTimeCursorPart(propertyValue);
            else if (PrimativeTypes.Contains(underlyingType))
                methodCallExpression = (MethodCallExpression)PrimativeCursorPart(underlyingType, propertyValue);
            else
                throw new ArgumentException($"Underlying type of {nameof(type)} must be DateTime or primative.");

            return Expression.Condition(Expression.Not(propertyHasValue), Expression.Constant(" "), methodCallExpression);
        }

        private static IEnumerable<Type> PrimativeTypes =>
            new List<Type> {
                typeof(char),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(float),
                typeof(double),
                typeof(bool)
            };
    }
}