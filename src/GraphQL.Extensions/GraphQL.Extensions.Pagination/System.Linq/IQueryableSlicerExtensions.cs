using System.Linq;
using GraphQL.Extensions.Pagination;

namespace System.Linq{
    public static class IQueryableSlicerExtensions {

        public static IQueryable<TEntity> Slice<TEntity>(this IQueryable<TEntity> query, Slicer slicer)
            where TEntity : class, new() {
            

        }
    }
}