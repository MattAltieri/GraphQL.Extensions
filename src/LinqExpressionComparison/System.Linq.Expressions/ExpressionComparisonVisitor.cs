using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Linq.Expressions {

    internal class ExpressionComparisonVisitor : ExpressionVisitor {

        private Queue<Expression> comparisonCandidateNodes;
        private Expression currentCandidateNode;

        public ExpressionComparisonVisitor(Expression leftTree, Expression rightTree) {

            comparisonCandidateNodes = new Queue<Expression>(new FlattenedExpressionTree(rightTree));

            AreEqual = true;

            // Visit method overrides will allow nodes in the left tree to be compared againse the nodes in the queue of
            // Expressions from the right tree.
            // As matches are found those expressions are removed from the queue of candidates.
            Visit(leftTree);

            // If there are nodes from the right-hand tree left in the queue, then the two trees are not equal
            if (comparisonCandidateNodes.Count > 0)
                AreEqual = false;
        }

        public bool AreEqual { get; private set; }

        #region Visit Overrides
        public override Expression Visit(Expression node) {

            if (node == null) return node;
            if (!AreEqual) return node;

            currentCandidateNode = PeekCandidateNode();
            if (currentCandidateNode == null) {
                AreEqual = false;
                return node;
            }
            if (currentCandidateNode.NodeType != node.NodeType) {
                AreEqual = false;
                return node;
            }
            if (currentCandidateNode.Type != node.Type) {
                AreEqual = false;
                return node;
            }

            PopCandidateNode();

            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node) {

            BinaryExpression candidate = (BinaryExpression)currentCandidateNode;

            if (!CheckEqual(node.Method, candidate.Method)) return node;
            if (!CheckEqual(node.IsLifted, candidate.IsLifted)) return node;
            if (!CheckEqual(node.IsLiftedToNull, candidate.IsLiftedToNull)) return node;

            return base.VisitBinary(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
            => !CheckEqual(node.Value, ((ConstantExpression)currentCandidateNode).Value) ? node : base.VisitConstant(node);

        protected override Expression VisitMember(MemberExpression node)
            => !CheckEqual(node.Member, ((MemberExpression)currentCandidateNode).Member) ? node : base.VisitMember(node);

        protected override Expression VisitMethodCall(MethodCallExpression node) 
            => !CheckEqual(node.Method, ((MethodCallExpression)currentCandidateNode).Method) ? node : base.VisitMethodCall(node);

        protected override Expression VisitParameter(ParameterExpression node) 
            => !CheckEqual(node.Name, ((ParameterExpression)currentCandidateNode).Name) ? node : base.VisitParameter(node);

        protected override Expression VisitTypeBinary(TypeBinaryExpression node) 
            => !CheckEqual(node.TypeOperand, ((TypeBinaryExpression)currentCandidateNode).TypeOperand) ? node : base.VisitTypeBinary(node);

        protected override Expression VisitUnary(UnaryExpression node) {

            UnaryExpression candidate = (UnaryExpression)currentCandidateNode;

            if (!CheckEqual(node.Method, candidate.Method)) return node;
            if (!CheckEqual(node.IsLifted, candidate.IsLifted)) return node;
            if (!CheckEqual(node.IsLiftedToNull, candidate.IsLiftedToNull)) return node;

            return base.VisitUnary(node);
        }

        protected override Expression VisitNew(NewExpression node) {

            NewExpression candidate = (NewExpression)currentCandidateNode;

            if (!CheckEqual(node.Constructor, candidate.Constructor)) return node;
            CompareList(node.Members, candidate.Members);

            return base.VisitNew(node);
        }
        #endregion

        #region Helper Methods
        private Expression PeekCandidateNode() => comparisonCandidateNodes.Count == 0 ? null : comparisonCandidateNodes.Peek();

        private Expression PopCandidateNode() => comparisonCandidateNodes.Dequeue();

        private void Stop() => AreEqual = false;

        private void CompareList<T>(ReadOnlyCollection<T> operand1, ReadOnlyCollection<T> operand2)
            => CompareList(operand1, operand2, (item, candidate) => EqualityComparer<T>.Default.Equals(item, candidate));

        private void CompareList<T>(ReadOnlyCollection<T> operand1, ReadOnlyCollection<T> operand2, Func<T, T, bool> comparer) {

            if (!CheckSameSize(operand1, operand2)) return;

            for (int i = 0; i < (operand1?.Count ?? 0); i++) {
                if (!comparer(operand1[i], operand2[i])) {
                    Stop();
                    return;
                }
            }
        }

        private bool CheckSameType(Expression operand1, Expression operand2) {
            if (!CheckEqual(operand1.NodeType, operand2.NodeType)) return false;
            if (!CheckEqual(operand1.Type, operand2.Type)) return false;
            return true;
        }

        private bool CheckSameSize<T>(ReadOnlyCollection<T> operand1, ReadOnlyCollection<T> operand2)
            => CheckEqual(operand1?.Count ?? 0, operand2?.Count ?? 0);

        private bool CheckNotNull<T>(T t)
            where T : class {

            if (t == null) {
                Stop();
                return false;
            }
            return true;
        }

        private bool CheckEqual<T>(T operand1, T operand2) {

            if (!EqualityComparer<T>.Default.Equals(operand1, operand2)) {
                Stop();
                return false;
            }
            return true;
        }
        #endregion
    }
}