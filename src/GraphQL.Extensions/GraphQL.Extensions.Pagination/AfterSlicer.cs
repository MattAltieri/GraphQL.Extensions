namespace GraphQL.Extensions.Pagination {
    public class AfterSlicer<TSource> : Slicer<TSource>
        where TSource : class {
        
        public virtual string After { get; set; }
    }
}