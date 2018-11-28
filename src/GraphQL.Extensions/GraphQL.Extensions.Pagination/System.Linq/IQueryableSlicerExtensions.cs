// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using GraphQL.Extensions.Internal;
// using GraphQL.Extensions.Pagination;
// using GraphQL.Extensions.Pagination.Internal;

// namespace System.Linq{
//     public static class IQueryableSlicerExtensions {

//         public static IQueryable<TResult> Slice<TSource, TResult>(this IQueryable<TSource> query, ISlicer slicer)
//             where TSource : class
//             where TResult : class, new()
//             => Slice<TSource, TResult>(query, slicer, CursorInjector.Instance);

//         internal static IQueryable<TResult> Slice<TSource, TResult>(IQueryable<TSource> query, ISlicer slicer, ICursorInjector cursorInjector)
//             where TSource : class
//             where TResult : class, new() {
            
//             ParameterExpression param = (ParameterExpression)((MethodCallExpression)query.Expression).Arguments[0];
//             Expression<Func<TSource, TResult>> selector =
//                 (Expression<Func<TSource, TResult>>)((MethodCallExpression)query.Expression).Arguments[1];
                
//             IQueryable<TSource> baseQuery = query.Provider.CreateQuery<TSource>(param);

//             // IQueryable<TResult> returnQuery = baseQuery.Select(SlicerDynamicParser.InjectCursorIntoSelector<TSource, TResult>(selector, slicer));
//             IQueryable<TResult> returnQuery = baseQuery.Select(cursorInjector.InjectIntoSelector(selector, slicer));

//             int i = 0;
//             foreach (string orderByEntry in slicer.OrderByEntries()) {

//                 OrderByColumn orderByColumn = new OrderByColumn(orderByEntry, typeof(TResult), param);
//                 Expression<Func<TResult>> columnExpression = Expression.Lambda<Func<TResult>>(orderByColumn.MemberExpression, param);

//                 switch (orderByColumn.SortDirection) {
//                     case SortDirections.Ascending:
//                         if (i == 0) {
//                             returnQuery = (IOrderedQueryable<TResult>)CachedReflection
//                                 .OrderBy(typeof(TResult), orderByColumn.Type)
//                                 .Invoke(null, new object[] { columnExpression });
//                         } else {
//                             returnQuery = (IOrderedQueryable<TResult>)CachedReflection
//                                 .ThenBy(typeof(TResult), orderByColumn.Type)
//                                 .Invoke(null, new object[] { columnExpression });
//                         }
//                         break;
//                     case SortDirections.Descending:
//                         if (i == 0) {
//                             returnQuery = (IOrderedQueryable<TResult>)CachedReflection
//                                 .OrderByDescending(typeof(TResult), orderByColumn.Type)
//                                 .Invoke(null, new object[] { columnExpression });
//                         } else {
//                             returnQuery = (IOrderedQueryable<TResult>)CachedReflection
//                                 .ThenByDescending(typeof(TResult), orderByColumn.Type)
//                                 .Invoke(null, new object[] { columnExpression });
//                         }
//                         break;
//                 }
//                 i++;
//             }

//             return returnQuery;
//         }
//     }
// }