namespace GraphQL.Extensions.Pagination {
    public class BeforeSlicer<TSource> : Slicer<TSource>
        where TSource : class {
        
        public virtual string Before { get; set; }
    }
}