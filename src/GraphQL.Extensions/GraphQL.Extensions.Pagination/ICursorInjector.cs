using System;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public interface ICursorInjector {
         Expression<Func<TSource, TResult>> InjectIntoSelector<TSource, TResult>(
             Expression<Func<TSource, TResult>> selector,
             ISlicer slicer)
             where TSource : class
             where TResult : class, new();
    }
}