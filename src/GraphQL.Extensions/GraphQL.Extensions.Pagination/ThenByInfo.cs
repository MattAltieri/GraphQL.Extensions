using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class ThenByInfo<TSource> : OrderByInfoBase<TSource>
        where TSource : class {

        public IOrderedQueryable<TSource> Accept(OrderByInfoVisitor<TSource> visitor)
            => visitor.VisitThenBy(this);
    }
}