using System;
using System.Linq;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    public class OrderByInfo<TSource> : OrderByInfoBase<TSource>
        where TSource : class {

        public IOrderedQueryable<TSource> Accept(OrderByInfoVisitor<TSource> visitor)
            => visitor.VisitOrderBy(this);
    }
}