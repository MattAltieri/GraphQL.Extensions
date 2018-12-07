using LinqKit;
using SuccincT.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class Cursor {
        
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
                Expression.NewArrayInit(typeof(object), CursorExpressions.Select(e => Expression.Constant(e)))
            );

            return Expression.Bind(typeof(TSource).GetMember("Cursor")[0], methodCall);
        }

        // public virtual Expression<Func<TSource, bool>> GetCursorFilter<TSource>(CursorFilterTypes filterType)
        //     where TSource : class {

        //     Expression<Func<TSource, bool>> predicate = PredicateBuilder.New<TSource>(true);
        //     OrderByInfo<TSource> orderBy

        //     string[] cursorSegments = CursorFormatString.ToString().Split('/');
        //     if (cursorSegments == null || cursorSegments.Count() == 0)
        //         throw new InvalidOperationException($"{nameof(CursorFormatString)} is empty.");
            
        //     if (orderBy == null || cursorSegments.Count() != orderBy.Depth)
        //         throw new InvalidOperationException($"{nameof(CursorFormatString)")

        //     foreach (string cursorPart in CursorFormatString.ToString().Split('/')) {
        //         // (string value, string direction, string name) = cursorPart.Split(':');
        //         var (value, (direction, (type, rest))) = cursorPart.Split(':');

        //         MemberInfo 
        //     }
        // }

        

        
    }
}