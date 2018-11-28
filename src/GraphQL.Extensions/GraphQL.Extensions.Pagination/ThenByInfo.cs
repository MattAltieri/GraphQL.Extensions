using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class ThenByInfo<TSource> : OrderByInfoBase<TSource>
        where TSource : class {

        public ThenByInfo() { }

        public override IOrderedQueryable<TSource> Accept(SortVisitor<TSource> visitor)
            => visitor.Visit(this);
    }
}