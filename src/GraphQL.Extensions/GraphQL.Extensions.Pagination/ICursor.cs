using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GraphQL.Extensions.Pagination {
    public interface ICursor {
         StringBuilder CursorFormatString { get; set; }
         List<Expression> CursorExpressions { get; set; }

         MemberBinding GetMemberBinding<TSource>() where TSource : class;
    }
}