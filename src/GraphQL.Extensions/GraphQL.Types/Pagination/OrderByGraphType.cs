using GraphQL.Extensions.Pagination;
using GraphQL.Types;

namespace GraphQL.Types.Pagination {
    public class OrderByGraphType<TSource> : ObjectGraphType<OrderByInfoBase<TSource>>
        where TSource : class {
        
        public OrderByGraphType() {

            Name = "OrderBy";
            Field<OrderByGraphType<TSource>>("thenBy");
            Field<NonNullGraphType<StringGraphType>>("columnName");
            Field<NonNullGraphType<SortDirectionsGraphType>>("sortDirection");
        }
    }
}