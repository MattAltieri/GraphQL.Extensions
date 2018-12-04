using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class SortVisitor<TSource>
        where TSource : class {

        public virtual IQueryable<TSource> Query { get; protected set; }

        public virtual ParameterExpression Parameter { get; protected set; }

        public SortVisitor() { }

        public SortVisitor(SortVisitor<TSource> visitor)
            => (Query, Parameter) = (visitor.Query, visitor.Parameter);

        public SortVisitor(IQueryable<TSource> query, ParameterExpression param)
            => (Query, Parameter) = (query, param);

        public virtual IOrderedQueryable<TSource> Visit(OrderByInfo<TSource> orderBy) {
            
            MethodInfo orderByMethod;
            if (orderBy.SortDirection == SortDirections.Descending)
                orderByMethod = CachedReflection.OrderByDescending(typeof(TSource), orderBy.GetMemberType());
            else
                orderByMethod = CachedReflection.OrderBy(typeof(TSource), orderBy.GetMemberType());

            MethodInfo lambdaMethod = CachedReflection.Lambda(typeof(TSource), orderBy.GetMemberType());
            
            Query = (IOrderedQueryable<TSource>)orderByMethod.Invoke(
                null,
                new object[] { Query, lambdaMethod.Invoke(
                    null,
                    new object[] { orderBy.GetMemberExpression(Parameter), new ParameterExpression[] { Parameter } }
                )}
            );
            
            if (orderBy.ThenBy == null)
                return (IOrderedQueryable<TSource>)Query;
            return orderBy.ThenBy.Accept(this);
        }

        public virtual IOrderedQueryable<TSource> Visit(ThenByInfo<TSource> thenBy) {

            MethodInfo thenByMethod;
            if (thenBy.SortDirection == SortDirections.Descending)
                thenByMethod = CachedReflection.ThenByDescending(typeof(TSource), thenBy.GetMemberType());
            else
                thenByMethod = CachedReflection.ThenBy(typeof(TSource), thenBy.GetMemberType());

            MethodInfo lambdaMethod = CachedReflection.Lambda(typeof(TSource), thenBy.GetMemberType());

            Query = (IOrderedQueryable<TSource>)thenByMethod.Invoke(
                null,
                new object[] { Query, lambdaMethod.Invoke(
                    null,
                    new object[] { thenBy.GetMemberExpression(Parameter), new ParameterExpression[] { Parameter } }
                )}
            );
            if (thenBy.ThenBy == null)
                return (IOrderedQueryable<TSource>)Query;
            return thenBy.ThenBy.Accept(this);
        }
    }
}