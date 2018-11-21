using System;
using System.Linq;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    public class OrderByInfo<TSource> : OrderByInfoBase<TSource>
        where TSource : class {

        public override IOrderedQueryable<TSource> Accept(OrderByInfoVisitor<TSource> visitor)
            => visitor.Visit(this);
    }
}