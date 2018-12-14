using System.Collections.Generic;
using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using System.Text.RegularExpressions;
// using GraphQL.Extensions.Pagination.Internal;

namespace GraphQL.Extensions.Pagination {
    public abstract class SlicerBase<TSource>
        where TSource : class {
        public virtual OrderByInfo<TSource> OrderBy { get; set; }
        public virtual int? First { get; set; }
    }
}