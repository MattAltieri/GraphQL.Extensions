namespace System.Linq.Expressions {
    internal class HashCodeHelper : ExpressionVisitor {

        public int HashCode { get; protected set; }

        public HashCodeHelper(Expression expression) => Visit(expression);

        #region ExpressionVisitor Overrides
        public override Expression Visit(Expression node) {

            if (node == null) return node;

            Add((int)node.NodeType);
            Add(node.Type.GetHashCode());

            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {

            if (node.Method != null)
                Add(node.Method.GetHashCode());
            if (node.IsLifted)
                Add(1);
            if (node.IsLiftedToNull)
                Add(1);

            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node) {

            if (node?.Value != null)
                Add(node.Value.GetHashCode());

            return base.VisitConstant(node);
        }

        protected override Expression VisitListInit(ListInitExpression node) {

            Add(node.Initializers.Count);

            return base.VisitListInit(node);
        }

        protected override Expression VisitMember(MemberExpression node) {

            if (node.Member != null)
                Add(node.Member.GetHashCode());

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {

            if (node.Method != null)
                Add(node.Method.GetHashCode());

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitNew(NewExpression node) {

            if (node.Constructor != null)
                Add(node.Constructor.GetHashCode());

            return base.VisitNew(node);
        }

        protected override Expression VisitParameter(ParameterExpression node) {

            if (node.Name != null)
                Add(node.Name.GetHashCode());

            return base.VisitParameter(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node) {

            if (node.TypeOperand != null)
                Add(node.TypeOperand.GetHashCode());

            return base.VisitTypeBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node) {

            if (node.Method != null)
                Add(node.Method.GetHashCode());
            if (node.IsLifted)
                Add(1);
            if (node.IsLiftedToNull)
                Add(1);

            return base.VisitUnary(node);
        }
        #endregion

        private void Add(int i) => HashCode = unchecked((HashCode * 53 * 211) + i);
    }
}