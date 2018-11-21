using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class OrderByVisitor<TSource>
        where TSource : class {

        public virtual IQueryable<TSource> Query { get; protected set; }

        public virtual ParameterExpression Parameter { get; protected set; }

        public OrderByVisitor(IQueryable<TSource> query, ParameterExpression param)
            => (Query, Parameter) = (query, param);

        public virtual IOrderedQueryable<TSource> Visit(OrderByInfo<TSource> orderBy) {
            
            // MethodInfo orderByMethod;
            // if (orderBy.SortDirection == SortDirections.Descending)
            //     orderByMethod = CachedReflection.OrderByDescending(typeof(TSource), orderBy.GetMemberType());
            // else
            //     orderByMethod = CachedReflection.OrderBy(typeof(TSource), orderBy.GetMemberType());
            throw new NotImplementedException();
        }

        public virtual IOrderedQueryable<TSource> Visit(ThenByInfo<TSource> thenBy) {

            throw new NotImplementedException();   
        }

        public virtual Expression GetExpression(OrderByInfoBase<TSource> orderBy) {
            throw new NotImplementedException();
        }
    }
}