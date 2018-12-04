using LinqKit;
using System;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public interface ICursorParser<TSource>
        where TSource : class {
        
        string CursorValue { get; set; }
        OrderByInfo<TSource> OrderBy { get; set; }

        Expression<Func<TSource, bool>> GetFilterPredicate();
    }
}