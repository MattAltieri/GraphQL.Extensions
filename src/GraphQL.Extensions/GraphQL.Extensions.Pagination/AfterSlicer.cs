namespace GraphQL.Extensions.Pagination {
    public class AfterSlicer<TSource> : SlicerBase<TSource>
        where TSource : class {
        
        public virtual string After { get; set; }
    }
}