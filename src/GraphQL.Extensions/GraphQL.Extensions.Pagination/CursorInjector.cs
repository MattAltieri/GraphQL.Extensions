using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class CursorInjector<TSource, TResult>
        where TSource : class
        where TResult : class, new() {

        private readonly string cursorSegmentDelimiter;
        private readonly string cursorSubsegmentDelimiter;
        
        public OrderByInfo<TSource> OrderBy { get; protected set; }

        public CursorInjector(OrderByInfo<TSource> orderBy, string cursorSegmentDelimiter, string cursorSubsegmentDelimiter) {
            OrderBy = orderBy;
            this.cursorSegmentDelimiter = cursorSegmentDelimiter;
            this.cursorSubsegmentDelimiter = cursorSubsegmentDelimiter;
        }

        public Expression<Func<TSource, TResult>> InjectIntoSelector(Expression<Func<TSource, TResult>> originalSelector) {

            ParameterExpression parameterExpression = originalSelector.Parameters[0];
            MemberInitExpression originalMemberInit = (MemberInitExpression)originalSelector.Body;
            
            CursorVisitor<TSource, TResult> cursorVisitor =
                new CursorVisitor<TSource, TResult>(parameterExpression, cursorSegmentDelimiter, cursorSubsegmentDelimiter);
            Cursor cursor = cursorVisitor.Visit(OrderBy);
            
            List<MemberBinding> newMemberBindings = originalMemberInit.Bindings.ToList();
            newMemberBindings.Add(cursor.GetMemberBinding<TSource>());

            return Expression.Lambda<Func<TSource, TResult>>(
                Expression.MemberInit(originalMemberInit.NewExpression, newMemberBindings),
                parameterExpression
            );
        }
    }
}