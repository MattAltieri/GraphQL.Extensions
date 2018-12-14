namespace GraphQL.Extensions.Pagination {
    public class BeforeSlicer<TSource> : SlicerBase<TSource>
        where TSource : class {
        
        public virtual string Before { get; set; }
    }
}