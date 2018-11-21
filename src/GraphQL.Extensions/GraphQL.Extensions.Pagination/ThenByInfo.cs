using System;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class ThenByInfo<TSource> : OrderByInfoBase<TSource>
        where TSource : class {
    }
}