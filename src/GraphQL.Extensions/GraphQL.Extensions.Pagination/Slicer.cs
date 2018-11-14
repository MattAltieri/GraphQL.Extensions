namespace GraphQL.Extensions.Pagination {
    public class Slicer {
        public string OrderBy { get; set; }
        public int? First { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}