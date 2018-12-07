using LinqExpressionComparison.Test;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Should;

namespace System.Linq.Expressions {
    public class HashCodeHelperTest {

        HashCodeHelper systemUnderTest;

        #region Tests
        [Theory]
        [MemberData(nameof(GetBinaryExpressionTestData))]
        public void VisitBinary_Should_CalcHashCodeCorrectly(BinaryExpression binaryExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(binaryExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetConstantExpressionTestData))]
        public void VisitConstant_Should_CalcHashCodeCorrectly(ConstantExpression constantExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(constantExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetListInitExpressionTestData))]
        public void VisitListInit_Should_CalcHashCodeCorrectly(ListInitExpression listInitExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(listInitExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetMemberExpressionTestData))]
        public void VisitMember_Should_CalcHashCodeCorrectly(MemberExpression memberExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(memberExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetMethodCallExpressionTestData))]
        public void VisitMethodCall_Should_CalcHashCodeCorrectly(MethodCallExpression methodCallExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(methodCallExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetNewExpressionTestData))]
        public void VisitNew_Should_CalcHashCodeCorrectly(NewExpression newExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(newExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetParameterExpressionTestData))]
        public void VisitParameter_Should_CalcHashCodeCorrectly(ParameterExpression parameterExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(parameterExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetTypeBinaryExpressionTestData))]
        public void VisitTypeBinary_Should_CalcHashCodeCorrectly(TypeBinaryExpression typeBinaryExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(typeBinaryExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetUnaryExpressionTestData))]
        public void VisitUnary_Should_CalcHashCodeCorrectly(UnaryExpression unaryExpression, int expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new HashCodeHelper(unaryExpression));
            exception.ShouldBeNull();

            systemUnderTest.HashCode.ShouldBe(expectedResult);
        }
        #endregion

        #region Generate Test Data
        public static List<object[]> GetBinaryExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            BinaryExpression binaryExpression = Expression.Multiply(Expression.Constant(5), Expression.Constant(21));

            CalculateHash(ref expectedResult, (int)binaryExpression.NodeType);
            CalculateHash(ref expectedResult, binaryExpression.Type.GetHashCode());
            if (binaryExpression.Method != null)
                CalculateHash(ref expectedResult, binaryExpression.Method.GetHashCode());
            if (binaryExpression.IsLifted)
                CalculateHash(ref expectedResult, 1);
            if (binaryExpression.IsLiftedToNull)
                CalculateHash(ref expectedResult, 1);

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 5.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 21.GetHashCode());

            testData.Add(new object[] { binaryExpression, expectedResult });

            return testData;
        }

        public static List<object[]> GetConstantExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(bool).GetHashCode());
            CalculateHash(ref expectedResult, true.GetHashCode());

            testData.Add(new object[] { Expression.Constant(true), expectedResult });

            expectedResult = 0;
            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(bool).GetHashCode());
            CalculateHash(ref expectedResult, false.GetHashCode());

            testData.Add(new object[] { Expression.Constant(false), expectedResult });

            return testData;
        }

        public static List<object[]> GetListInitExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            Type type = typeof(List<int>);
            NewExpression newExpression = Expression.New(type);
            MethodInfo addMethod = type.GetMethod("Add");
            List<ElementInit> elementInits = new List<ElementInit>();

            elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(1)));
            elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(3)));
            elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(5)));
            elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(7)));
            elementInits.Add(Expression.ElementInit(addMethod, Expression.Constant(9)));

            ListInitExpression listInitExpression = Expression.ListInit(newExpression, elementInits);

            CalculateHash(ref expectedResult, (int)listInitExpression.NodeType);
            CalculateHash(ref expectedResult, listInitExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, listInitExpression.Initializers.Count);

            CalculateHash(ref expectedResult, (int)newExpression.NodeType);
            CalculateHash(ref expectedResult, newExpression.Type.GetHashCode());
            if (newExpression.Constructor != null)
                CalculateHash(ref expectedResult, newExpression.Constructor.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 1.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 3.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 5.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 7.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 9.GetHashCode());

            testData.Add(new object[] { listInitExpression, expectedResult });

            return testData;
        }

        public static List<object[]> GetMemberExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            Type mockTypeA = typeof(MockObjectA);
            MockObjectA mockObjectA = new MockObjectA();
            MemberInfo nameProp = mockTypeA.GetProperty("Name");
            MemberExpression nameExpression = Expression.MakeMemberAccess(Expression.Constant(mockObjectA), nameProp);

            CalculateHash(ref expectedResult, (int)nameExpression.NodeType);
            CalculateHash(ref expectedResult, nameExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, nameProp.GetHashCode());
            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectA).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectA.GetHashCode());

            testData.Add(new object[] { nameExpression, expectedResult });

            MemberInfo theThingField = mockTypeA.GetField("TheThing");
            MemberExpression theThingExpression = Expression.MakeMemberAccess(Expression.Constant(mockObjectA), theThingField);

            expectedResult = 0;
            CalculateHash(ref expectedResult, (int)theThingExpression.NodeType);
            CalculateHash(ref expectedResult, theThingExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, theThingField.GetHashCode());
            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectA).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectA.GetHashCode());

            testData.Add(new object[] { theThingExpression, expectedResult });

            Type mockTypeB = typeof(MockObjectB);
            MockObjectB mockObjectB = new MockObjectB();
            MemberInfo genderProp = mockTypeB.GetProperty("Gender");
            MemberExpression genderExpression = Expression.MakeMemberAccess(Expression.Constant(mockObjectB), genderProp);

            expectedResult = 0;
            CalculateHash(ref expectedResult, (int)genderExpression.NodeType);
            CalculateHash(ref expectedResult, genderExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, genderProp.GetHashCode());
            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectB).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectB.GetHashCode());

            testData.Add(new object[] { genderExpression, expectedResult });

            MemberInfo anotherThingField = mockTypeB.GetField("AnotherThing");
            MemberExpression anotherThingExpression = Expression.MakeMemberAccess(Expression.Constant(mockObjectB), anotherThingField);

            expectedResult = 0;
            CalculateHash(ref expectedResult, (int)anotherThingExpression.NodeType);
            CalculateHash(ref expectedResult, anotherThingExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, anotherThingField.GetHashCode());
            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectB).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectB.GetHashCode());

            testData.Add(new object[] { anotherThingExpression, expectedResult });

            return testData;
        }

        public static List<object[]> GetMethodCallExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            Type mockTypeA = typeof(MockObjectA);
            MockObjectA mockObjectA = new MockObjectA();
            MethodInfo doubleItMethod = mockTypeA.GetMethod("DoubleIt");
            MethodCallExpression doubleItExpression = Expression.Call(Expression.Constant(mockObjectA), doubleItMethod, Expression.Constant(5));

            CalculateHash(ref expectedResult, (int)doubleItExpression.NodeType);
            CalculateHash(ref expectedResult, doubleItExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, doubleItMethod.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, mockTypeA.GetHashCode());
            CalculateHash(ref expectedResult, mockObjectA.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 5.GetHashCode());

            testData.Add(new object[] { doubleItExpression, expectedResult });

            expectedResult = 0;
            Type mockTypeB = typeof(MockObjectB);
            MockObjectB mockObjectB = new MockObjectB();
            MethodInfo tripleItMethod = mockTypeB.GetMethod("TripleIt");
            MethodCallExpression tripleItExpression = Expression.Call(Expression.Constant(mockObjectB), tripleItMethod, Expression.Constant(9));

            CalculateHash(ref expectedResult, (int)tripleItExpression.NodeType);
            CalculateHash(ref expectedResult, tripleItExpression.Type.GetHashCode());
            CalculateHash(ref expectedResult, tripleItMethod.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, mockTypeB.GetHashCode());
            CalculateHash(ref expectedResult, mockObjectB.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 9.GetHashCode());

            testData.Add(new object[] { tripleItExpression, expectedResult });

            return testData;
        }

        public static List<object[]> GetNewExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            Type mockTypeA = typeof(MockObjectA);
            ConstructorInfo ctorA = mockTypeA.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(int), typeof(string) }, null);
            NewExpression newExpressionA = Expression.New(ctorA, Expression.Constant(1), Expression.Constant("Matt"));

            CalculateHash(ref expectedResult, (int)newExpressionA.NodeType);
            CalculateHash(ref expectedResult, newExpressionA.Type.GetHashCode());
            CalculateHash(ref expectedResult, ctorA.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 1.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(string).GetHashCode());
            CalculateHash(ref expectedResult, "Matt".GetHashCode());

            testData.Add(new object[] { newExpressionA, expectedResult });

            expectedResult = 0;
            Type mockTypeB = typeof(MockObjectB);
            ConstructorInfo ctorB = mockTypeB.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null,
                new Type[] { typeof(int), typeof(string), typeof(string) }, null);
            NewExpression newExpressionB = Expression.New(ctorB, Expression.Constant(2), Expression.Constant("Aimee"), Expression.Constant("Female"));

            CalculateHash(ref expectedResult, (int)newExpressionB.NodeType);
            CalculateHash(ref expectedResult, newExpressionB.Type.GetHashCode());
            CalculateHash(ref expectedResult, ctorB.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, 2.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(string).GetHashCode());
            CalculateHash(ref expectedResult, "Aimee".GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(string).GetHashCode());
            CalculateHash(ref expectedResult, "Female".GetHashCode());

            testData.Add(new object[] { newExpressionB, expectedResult });

            return testData;
        }

        public static List<object[]> GetParameterExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            ParameterExpression parameterA = Expression.Parameter(typeof(MockObjectA));

            CalculateHash(ref expectedResult, (int)parameterA.NodeType);
            CalculateHash(ref expectedResult, parameterA.Type.GetHashCode());

            testData.Add(new object[] { parameterA, expectedResult });

            expectedResult = 0;
            ParameterExpression namedParameterA = Expression.Parameter(typeof(MockObjectA), "mockA");

            CalculateHash(ref expectedResult, (int)namedParameterA.NodeType);
            CalculateHash(ref expectedResult, namedParameterA.Type.GetHashCode());
            CalculateHash(ref expectedResult, "mockA".GetHashCode());

            testData.Add(new object[] { namedParameterA, expectedResult });

            return testData;
        }

        public static List<object[]> GetTypeBinaryExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            MockObjectA mockObjectA = new MockObjectA();
            TypeBinaryExpression typeBinaryExpressionAIsA = Expression.TypeIs(Expression.Constant(mockObjectA), typeof(MockObjectA));

            CalculateHash(ref expectedResult, (int)typeBinaryExpressionAIsA.NodeType);
            CalculateHash(ref expectedResult, typeBinaryExpressionAIsA.Type.GetHashCode());
            if (typeBinaryExpressionAIsA.TypeOperand != null)
                CalculateHash(ref expectedResult, typeBinaryExpressionAIsA.TypeOperand.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectA).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectA.GetHashCode());

            testData.Add(new object[] { typeBinaryExpressionAIsA, expectedResult });

            expectedResult = 0;
            TypeBinaryExpression typeBinaryExpressionAIsB = Expression.TypeIs(Expression.Constant(mockObjectA), typeof(MockObjectB));

            CalculateHash(ref expectedResult, (int)typeBinaryExpressionAIsB.NodeType);
            CalculateHash(ref expectedResult, typeBinaryExpressionAIsB.Type.GetHashCode());
            if (typeBinaryExpressionAIsB.TypeOperand != null)
                CalculateHash(ref expectedResult, typeBinaryExpressionAIsB.TypeOperand.GetHashCode());

            CalculateHash(ref expectedResult, (int)ExpressionType.Constant);
            CalculateHash(ref expectedResult, typeof(MockObjectA).GetHashCode());
            CalculateHash(ref expectedResult, mockObjectA.GetHashCode());

            testData.Add(new object[] { typeBinaryExpressionAIsB, expectedResult });

            return testData;
        }

        public static List<object[]> GetUnaryExpressionTestData() {

            int expectedResult = 0;
            List<object[]> testData = new List<object[]>();

            ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "value");
            UnaryExpression unaryExpression = Expression.PostIncrementAssign(parameterExpression);

            CalculateHash(ref expectedResult, (int)unaryExpression.NodeType);
            CalculateHash(ref expectedResult, unaryExpression.Type.GetHashCode());
            if (unaryExpression.Method != null)
                CalculateHash(ref expectedResult, unaryExpression.Method.GetHashCode());
            if (unaryExpression.IsLifted)
                CalculateHash(ref expectedResult, 1);
            if (unaryExpression.IsLiftedToNull)
                CalculateHash(ref expectedResult, 1);

            CalculateHash(ref expectedResult, (int)ExpressionType.Parameter);
            CalculateHash(ref expectedResult, typeof(int).GetHashCode());
            CalculateHash(ref expectedResult, "value".GetHashCode());

            testData.Add(new object[] { unaryExpression, expectedResult });

            return testData;
        }
        #endregion

        #region Helper Methods
        private static void CalculateHash(ref int hash, int i) => hash = unchecked((hash * 53 * 211) + i);
        #endregion
    }
}