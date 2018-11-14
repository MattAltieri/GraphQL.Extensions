using GraphQL.Extensions.Pagination;
using GraphQL.Types;

namespace GraphQL.Types.Pagination {
    public class PageDetailGraphType : ObjectGraphType<PageDetail> {
        
        public PageDetailGraphType() {

            Name = "PageDetail";
            Field<NonNullGraphType<BooleanGraphType>>("hasNextPage");
            Field<NonNullGraphType<BooleanGraphType>>("hasPreviousPage");
            Field<StringGraphType>("startCursor");
            Field<StringGraphType>("endCursor");
        }
    }
}