using System.Collections.Generic;

namespace System.Linq.Expressions {
    public class ExpressionEqualityComparer : IEqualityComparer<Expression> {

        public static ExpressionEqualityComparer Instance = new ExpressionEqualityComparer();

        public bool Equals(Expression left, Expression right) => new ExpressionComparisonVisitor(left, right).AreEqual;

        public int GetHashCode(Expression expression) => new HashCodeHelper(expression).HashCode;
    }
}