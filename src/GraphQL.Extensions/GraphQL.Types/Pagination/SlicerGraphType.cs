using GraphQL.Extensions.Pagination;
using GraphQL.Types;

namespace GraphQL.Types.Pagination {
    public class SlicerGraphType : ObjectGraphType<Slicer> {
        
        public SlicerGraphType() {

            Name = "Slicer";
            Field<NonNullGraphType<BooleanGraphType>>("hasNextPage");
            Field<NonNullGraphType<BooleanGraphType>>("hasPreviousPage");
            Field<StringGraphType>("startCursor");
            Field<StringGraphType>("endCursor");
        }
    }
}