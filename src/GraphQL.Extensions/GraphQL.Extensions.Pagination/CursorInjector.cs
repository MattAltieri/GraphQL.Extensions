using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GraphQL.Extensions.Pagination.Internal;

namespace GraphQL.Extensions.Pagination {
    public sealed class CursorInjector : ICursorInjector {
        
        public static readonly Lazy<ICursorInjector> instance =
            new Lazy<ICursorInjector>(() => new CursorInjector());

        private CursorInjector() { }

        public static ICursorInjector Instance => instance.Value;

        public Expression<Func<TSource, TResult>> InjectIntoSelector<TSource, TResult>(
            Expression<Func<TSource, TResult>> selector,
            ISlicer slicer)
            where TSource : class
            where TResult : class, new() {
            
            ParameterExpression param = selector.Parameters[0];
            List<Expression> cursorParts = new List<Expression>();
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (OrderByColumn column in slicer.OrderByEntries().Select(s => new OrderByColumn(s, typeof(TSource), param))) {

                MemberTypes memberType = column.MemberInfo.MemberType;
                Type type = (column.MemberInfo as FieldInfo)?.FieldType ?? (column.MemberInfo as PropertyInfo)?.PropertyType;
                if (type == null || (memberType != MemberTypes.Field && memberType != MemberTypes.Property))
                    throw new ArgumentException($"{column.Name} is not a property or field of {nameof(TSource)}.");

                // cursorParts.Add
            }
            throw new NotImplementedException();
        }

        
        // private static MethodCallExpression StringFormatExpression

        private static IEnumerable<Type> PrimitiveTypes
            => new List<Type> {
                typeof(char),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(float),
                typeof(double),
                typeof(bool),
            };
    }
}