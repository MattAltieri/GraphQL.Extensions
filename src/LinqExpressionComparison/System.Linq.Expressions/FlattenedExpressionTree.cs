using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Expressions {
    internal class FlattenedExpressionTree : ExpressionVisitor, IEnumerable<Expression> {

        private List<Expression> expressions = new List<Expression>();

        public FlattenedExpressionTree() { }

        public FlattenedExpressionTree(Expression expression) => Visit(expression);

        public IEnumerator<Expression> GetEnumerator() => expressions.GetEnumerator();

        public override Expression Visit(Expression node) {

            if (node == null) return node;

            expressions.Add(node);
            return base.Visit(node);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}