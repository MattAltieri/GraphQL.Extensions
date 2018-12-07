using LinqExpressionComparison.Test;
using System.Collections.Generic;
using Xunit;
using Xunit.Should;

namespace System.Linq.Expressions {
    public class FlattenedExpressionTreeTest {

        FlattenedExpressionTree systemUnderTest;

        #region Tests
        [Theory]
        [MemberData(nameof(GetTestData))]
        public void Visit_Should_CorrectlyParseExpressionTree(Expression tree, IEnumerable<Expression> expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new FlattenedExpressionTree());
            exception.ShouldBeNull();

            exception = Record.Exception(() => systemUnderTest.Visit(tree));
            exception.ShouldBeNull();

            // Confirms that all object instances in the two enumerables are the same instances
            systemUnderTest.SequenceEqual(expectedResult).ShouldBeTrue();
        }
        #endregion

        #region Generate Test Data
        public static List<object[]> GetTestData() {

            List<object[]> testData = new List<object[]>();

            Type mockTypeA = typeof(MockObjectA);
            ParameterExpression parameterA = Expression.Parameter(mockTypeA);
            Type mockTypeB = typeof(MockObjectB);
            ParameterExpression parameterB = Expression.Parameter(mockTypeB);

            // Simple constant expression w/ object parameter
            ConstantExpression simpleConst = Expression.Constant(true);
            Expression<Func<MockObjectA, bool>> simpleExpression = Expression.Lambda<Func<MockObjectA, bool>>(simpleConst, parameterA);

            testData.Add(new object[] { simpleExpression, new Expression[] { simpleExpression, simpleConst, parameterA } });

            // Simple "and" expression
            ConstantExpression simpleAndConstLeft = Expression.Constant(true);
            ConstantExpression simpleAndConstRight = Expression.Constant(false);
            BinaryExpression simpleAndExpression = Expression.And(simpleAndConstLeft, simpleAndConstRight);
            Expression<Func<MockObjectA, bool>> simpleAndLambda = Expression.Lambda<Func<MockObjectA, bool>>(simpleAndExpression, parameterA);

            testData.Add(new object[] {
                simpleAndLambda,
                new Expression[] {
                    simpleAndLambda,
                    simpleAndExpression,
                    simpleAndConstLeft,
                    simpleAndConstRight,
                    parameterA,
                },
            });

            // Simple multiply expression
            ConstantExpression simpleMultiplyConstLeft = Expression.Constant(2);
            ConstantExpression simpleMultiplyConstRight = Expression.Constant(3);
            BinaryExpression simpleMultiplyExpression = Expression.Multiply(simpleMultiplyConstLeft, simpleMultiplyConstRight);
            Expression<Func<MockObjectA, int>> simpleMultiplyLambda = Expression.Lambda<Func<MockObjectA, int>>(simpleMultiplyExpression, parameterA);

            testData.Add(new object[] {
                simpleMultiplyLambda,
                new Expression[] {
                    simpleMultiplyLambda,
                    simpleMultiplyExpression,
                    simpleMultiplyConstLeft,
                    simpleMultiplyConstRight,
                    parameterA,
                },
            });

            // Simple equality expression
            ConstantExpression simpleEqConstLeft = Expression.Constant(5);
            ConstantExpression simpleEqConstRight = Expression.Constant(5);
            BinaryExpression simpleEqExpression = Expression.Equal(simpleEqConstLeft, simpleEqConstRight);
            Expression<Func<MockObjectA, bool>> simpleEqLambda = Expression.Lambda<Func<MockObjectA, bool>>(simpleEqExpression, parameterA);

            testData.Add(new object[] {
                simpleEqLambda,
                new Expression[] {
                    simpleEqLambda,
                    simpleEqExpression,
                    simpleEqConstLeft,
                    simpleEqConstRight,
                    parameterA,
                },
            });


            // Complex tree
            /*
             * x => true && (2 + 3 == 5 || true != false);
             * 
             *     AND
             *    /   \
             *   /     \
             *  T       \
             *          OR
             *         /  \
             *        /    \
             *       /      \
             *      ==      !=
             *     /  \     / \
             *    +    5   T   F
             *   / \
             *  2   3
             * 
             * Should flatten to: `AND` lambda expr, `And` binary expr, `true` constant expr, `OR` binary expr, `==` binary expr, `+` binary expr,
             * `2` constant expr, `3` constant expr, `5` constant expr, `!=` binary expr, `true` constant expr, `false` constant expr, object type parameter expr
             * 
             * */

            ConstantExpression complexNeqConstLeft = Expression.Constant(true);
            ConstantExpression complexNeqConstRight = Expression.Constant(false);
            BinaryExpression complexNeqExpression = Expression.NotEqual(complexNeqConstLeft, complexNeqConstRight);

            ConstantExpression complexAddConstLeft = Expression.Constant(2);
            ConstantExpression complexAddConstRight = Expression.Constant(3);
            BinaryExpression complexAddExpression = Expression.Add(complexAddConstLeft, complexAddConstRight);

            // Left side of this expression is `complexAddExpression`
            ConstantExpression complexEqConstRight = Expression.Constant(5);
            BinaryExpression complexEqExpression = Expression.Equal(complexAddExpression, complexEqConstRight);

            // Left side of this expression is `complexEqExpression`
            // Right side of this expression is `complexNeqExpression`
            BinaryExpression complexOrExpression = Expression.Or(complexEqExpression, complexNeqExpression);

            ConstantExpression complexAndConstLeft = Expression.Constant(true);
            // Right side of this expression is `complexOrExpression`
            BinaryExpression complexAndExpression = Expression.And(complexAndConstLeft, complexOrExpression);

            Expression<Func<MockObjectA, bool>> complexLambda = Expression.Lambda<Func<MockObjectA, bool>>(complexAndExpression, parameterA);

            testData.Add(new object[] {
                complexLambda,
                new Expression[] {
                    complexLambda,
                    complexAndExpression,
                    complexAndConstLeft,
                    complexOrExpression,
                    complexEqExpression,
                    complexAddExpression,
                    complexAddConstLeft,
                    complexAddConstRight,
                    complexEqConstRight,
                    complexNeqExpression,
                    complexNeqConstLeft,
                    complexNeqConstRight,
                    parameterA,
                },
            });

            // MemberInitExpression (Select instances of MockObjectB from a collection of MockObjectA)
            MockObjectA mockObjectA = new MockObjectA {
                Id = 1,
                Name = "Matt",
            };
            NewExpression newExpression = Expression.New(mockTypeB);
            ConstantExpression sourceExpression = Expression.Constant(mockObjectA);

            MemberExpression idMemberExpression = Expression.PropertyOrField(sourceExpression, "Id");
            MemberBinding idMemberBinding = Expression.Bind(mockTypeB.GetMember("Id").First(), idMemberExpression);

            MemberExpression nameMemberExpression = Expression.PropertyOrField(sourceExpression, "Name");
            MemberBinding nameMemberBinding = Expression.Bind(mockTypeB.GetMember("Name").First(), nameMemberExpression);

            List<MemberBinding> memberBindings = new List<MemberBinding>();
            memberBindings.Add(idMemberBinding);
            memberBindings.Add(nameMemberBinding);

            MemberInitExpression memberInitExpression = Expression.MemberInit(newExpression, memberBindings);
            Expression<Func<MockObjectA, MockObjectB>> memberInitLambda = Expression.Lambda<Func<MockObjectA, MockObjectB>>(memberInitExpression, parameterA);

            testData.Add(new object[] {
                memberInitLambda,
                new Expression[] {
                    memberInitLambda,
                    memberInitExpression,
                    newExpression,
                    idMemberExpression,
                    sourceExpression,
                    nameMemberExpression,
                    sourceExpression,
                    parameterA,
                },
            });

            return testData;
        }
        #endregion
    }
}