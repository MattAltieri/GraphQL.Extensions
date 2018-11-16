using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using GraphQL.Extensions.Pagination.Internal;

namespace GraphQL.Extensions.Pagination {
    public class Slicer : ISlicer {
        public string OrderBy { get; set; }
        public int? First { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }
}