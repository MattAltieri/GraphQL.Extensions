using LinqKit;
using SuccincT.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class Cursor : ICursor {
        
        public virtual StringBuilder CursorFormatString { get; set; }
        public virtual List<Expression> CursorExpressions { get; set; }

        public Cursor() {
            CursorFormatString = new StringBuilder();
            CursorExpressions = new List<Expression>();
        }

        public virtual MemberBinding GetMemberBinding<TSource>()
            where TSource : class {

            MethodCallExpression methodCall = Expression.Call(
                CachedReflection.StringFormat(),
                Expression.Constant(CursorFormatString.ToString()),
                Expression.NewArrayInit(typeof(object), CursorExpressions.Select(e => e))
            );

            return Expression.Bind(typeof(TSource).GetMember("Cursor")[0], methodCall);
        }        
    }
}