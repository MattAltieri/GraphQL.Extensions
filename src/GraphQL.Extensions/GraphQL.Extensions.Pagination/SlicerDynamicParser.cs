using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GraphQL.Extensions.Pagination.Internal;

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
        
        internal static Expression<Func<TSource, TResult>> InjectCursorIntoSelector<TSource, TResult>(
            Expression<Func<TSource, TResult>> selector,
            IEnumerable<string> orderByEntries)
            where TSource : class
            where TResult : class, new() {

                ParameterExpression param = selector.Parameters[0];
                List<Expression> cursorParts = new List<Expression>();
                StringBuilder sb = new StringBuilder();

                int i = 0;
                foreach (var orderByEntry in orderByEntries) {

                    OrderByColumn column = new OrderByColumn(orderByEntry, typeof(TSource), param);

                    MemberTypes memberType = column.MemberInfo.MemberType;
                    Type type = (column.MemberInfo as FieldInfo)?.FieldType ?? (column.MemberInfo as PropertyInfo)?.PropertyType;
                    if (type == null || (memberType != MemberTypes.Field && memberType != MemberTypes.Property))
                        throw new ArgumentException($"{column.Name} is not a property or field of {nameof(TSource)}.");
                    
                    cursorParts.Add(GetCursorPart(type, column.MemberExpression));
                    if (i > 0)
                        sb.Append("//");
                    string direction = column.SortDirection == SortDirections.Descending ? "desc" : "asc";
                    sb.Append($"{i.ToString()}:{direction}:{{{i.ToString()}}}");

                    i++;
                }

                MethodCallExpression stringFormatExpression = StringFormatExpression(sb.ToString(), cursorParts);

                List<MemberBinding> memberBindings = ((MemberInitExpression)selector.Body).Bindings.ToList();
                memberBindings.Add(Expression.Bind(typeof(TSource).GetMember("Cursor")[0], stringFormatExpression));
                MemberInitExpression memberInitExpression = Expression.MemberInit(((MemberInitExpression)selector.Body).NewExpression, memberBindings);

                return Expression.Lambda<Func<TSource, TResult>>(memberInitExpression, param);
        }

        private static Expression GetCursorPart(Type type, MemberExpression memberExpression) {
            if (!type.IsValueType)
                throw new ArgumentException($"{nameof(type)} is not a value type.");
                
            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return NullableCursorPart(type, memberExpression);
            else if (type == typeof(DateTime))
                return DateTimeCursorPart(memberExpression);
            else if (PrimativeTypes.Contains(type))
                return PrimativeCursorPart(type, memberExpression);
            else if (type == typeof(string))
                return memberExpression;
            else
                throw new ArgumentException($"{nameof(type)} is not supported.");
        }

        private static Expression DateTimeCursorPart(MemberExpression memberExpression) {

            MemberExpression ticks = Expression.MakeMemberAccess(memberExpression, typeof(DateTime).GetProperty("Ticks"));
            return PrimativeCursorPart(typeof(long), ticks);
        }

        private static Expression PrimativeCursorPart(Type type, MemberExpression memberExpression)
            => Expression.Call(memberExpression, toStringMethods[type]);

        private static Expression NullableCursorPart(Type type, MemberExpression memberExpression) {

            Type underlyingType = Nullable.GetUnderlyingType(type);

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

        private static MethodCallExpression StringFormatExpression(string formatString, List<Expression> args) {
            return Expression.Call(
                stringFormatMethod,
                Expression.Constant(formatString),
                Expression.NewArrayInit(typeof(object), args.Select(a => Expression.Constant(a))));
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