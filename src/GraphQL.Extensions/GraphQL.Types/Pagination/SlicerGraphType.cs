using GraphQL.Extensions.Pagination;
using GraphQL.Types;

namespace GraphQL.Types.Pagination {
    public class SlicerGraphType : ObjectGraphType<Slicer> {
        
        public SlicerGraphType() {

            Name = "Slicer";
            Field<NonNullGraphType<StringGraphType>>("orderBy");
            Field<IntGraphType>("first");
            Field<StringGraphType>("before");
            Field<StringGraphType>("after");
        }
    }
}