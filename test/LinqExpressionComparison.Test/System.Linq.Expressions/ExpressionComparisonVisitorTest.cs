using LinqExpressionComparison.Test;
using System.Collections.Generic;
using Xunit;
using Xunit.Should;

namespace System.Linq.Expressions {

    public class ExpressionComparisonVisitorTest {

        #region Tests
        [Fact]
        public void VisitDifferentNodeTypes_Should_NotBeEqual() {

            Expression<Func<int>> left = () => 5;
            Expression<Func<MockObjectA, int>> right = x => x.Id;

            CompareExpressions(left, right).ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(GetBinaryExpressionTestData))]
        public void VisitBinary_Should_DetermineEqualityCorrectly(BinaryExpression left, BinaryExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetConstantExpressionTestData))]
        public void VisitConstant_Should_DetermineEqualityCorrectly(ConstantExpression left, ConstantExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetPropertyMemberExpressionTestData))]
        [MemberData(nameof(GetFieldMemberExpressionTestData))]
        public void VisitMember_Should_DetermineEqualityCorrectly(MemberExpression left, MemberExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetMemberInitExpressionTestData))]
        [MemberData(nameof(GetMemberInitExpressionWithRowNumberTestData))]
        public void VisitMemberInit_Should_DetermineEqualityCorrectly(MemberInitExpression left, MemberInitExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetMethodCallExpressionTestData))]
        public void VisitMethodCall_Should_DetermineEqualityCorrectly(MethodCallExpression left, MethodCallExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetNewExpressionTestData))]
        [MemberData(nameof(GetNewExpressionWithRowNumberTestData))]
        public void VisitNew_Should_DetermineEqualityCorrectly(NewExpression left, NewExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetParameterExpressionTestData))]
        public void VisitParameter_Should_DetermineEqualityCorrectly(ParameterExpression left, ParameterExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetTypeBinaryExpressionTestData))]
        public void VisitTypeBinary_Should_DetermineEqualityCorrectly(TypeBinaryExpression left, TypeBinaryExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetUnaryExpressionTestData))]
        public void VisitUnary_Should_DetermineEqualityCorrectly(UnaryExpression left, UnaryExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);

        [Theory]
        [MemberData(nameof(GetComplexExpressionTestData))]
        public void VisitComplexTrees_Should_DetermineEqualityCorrectly(LambdaExpression left, LambdaExpression right, bool expectedResult)
            => CompareExpressions(left, right).ShouldBe(expectedResult);
        #endregion

        #region Generate Test Data
        public static List<object[]> GetBinaryExpressionTestData
            => new List<object[]> {
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x + y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x + y)).Body,
                    true,
                },
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x + y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x - y)).Body,
                    false,
                },
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x + y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, z) => x + z)).Body,
                    false,
                },
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, int>>)((x, y) => x + y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, bool>>)((x, y) => x == y)).Body,
                    false,
                },
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, bool>>)((x, y) => x == y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, bool>>)((x, y) => x == y)).Body,
                    true,
                },
                new object[] {
                    (BinaryExpression)((Expression<Func<int, int, bool>>)((x, y) => x == y)).Body,
                    (BinaryExpression)((Expression<Func<int, int, bool>>)((x, y) => x != y)).Body,
                    false,
                },
            };

        public static List<object[]> GetConstantExpressionTestData
            => new List<object[]> {
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(5),
                    true,
                },
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(50),
                    false,
                },
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(2 + 3),
                    true,
                },
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(200 + 300),
                    false,
                },
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(9 - 4),
                    true,
                },
                new object[] {
                    (ConstantExpression)((Expression<Func<int>>)(() => 5)).Body,
                    Expression.Constant(900 - 400),
                    false,
                },
            };

        public static List<object[]> GetPropertyMemberExpressionTestData
            => new List<object[]> {
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    true,
                },
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    (MemberExpression)((Expression<Func<MockObjectA, string>>)(x => x.Name)).Body,
                    false,
                },
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(y => y.Id)).Body,
                    false,
                },
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    (MemberExpression)((Expression<Func<MockObjectB, int>>)(x => x.Id)).Body,
                    false,
                },
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectB, string>>)(x => x.Gender)).Body,
                    (MemberExpression)((Expression<Func<MockObjectB, string>>)(x => x.Gender)).Body,
                    true,
                },
            };

        public static List<object[]> GetFieldMemberExpressionTestData
            => new List<object[]> {
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, object>>)(x => x.TheThing)).Body,
                    (MemberExpression)((Expression<Func<MockObjectA, object>>)(x => x.TheThing)).Body,
                    true,
                },
                new object[] {
                    (MemberExpression)((Expression<Func<MockObjectA, object>>)(x => x.TheThing)).Body,
                    (MemberExpression)((Expression<Func<MockObjectA, int>>)(x => x.Id)).Body,
                    false,
                },
            };

        public static List<object[]> GetMemberInitExpressionTestData
            => new List<object[]> {
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    true,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 2,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Aimee",
                        Gender = "Male",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Female",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Gender = "Male",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                        Gender = "Male",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = "Matt",
                    })).Body,
                    false
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = x.Id,
                        Name = x.Name,
                        Gender = "Unknown",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = x.Id,
                        Name = x.Name,
                        Gender = "Unknown",
                    })).Body,
                    true,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = x.Id,
                        Name = x.Name,
                        Gender = "Unknown",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = 1,
                        Name = x.Name,
                        Gender = "Unknown",
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = x.Id,
                        Name = x.Name,
                        Gender = "Unknown",
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, MockObjectB>>)(x => new MockObjectB {
                        Id = x.Id,
                        Name = "Matt",
                        Gender = "Unknown",
                    })).Body,
                    false,
                },
            };

        public static List<object[]> GetMemberInitExpressionWithRowNumberTestData
            => new List<object[]> {
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    true,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx + 1,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Aimee",
                        Gender = "Male"
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Female"
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Gender = "Male"
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                        Gender = "Male"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = idx,
                        Name = "Matt",
                    })).Body,
                    false,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = x.Id + idx,
                        Name = x.Name,
                        Gender = "Unknown"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = x.Id + idx,
                        Name = x.Name,
                        Gender = "Unknown"
                    })).Body,
                    true,
                },
                new object[] {
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = x.Id + idx,
                        Name = x.Name,
                        Gender = "Unknown"
                    })).Body,
                    (MemberInitExpression)((Expression<Func<MockObjectA, int, MockObjectB>>)((x, idx) => new MockObjectB {
                        Id = x.Id + idx,
                        Name = "Matt",
                        Gender = "Unknown"
                    })).Body,
                    false,
                },
            };

        public static List<object[]> GetMethodCallExpressionTestData()
            => new List<object[]> {
                new object[] {
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(5))).Body,
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(5))).Body,
                    true,
                },
                new object[] {
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(5))).Body,
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(50))).Body,
                    false,
                },
                new object[] {
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(5))).Body,
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(y => y.DoubleIt(5))).Body,
                    false,
                },
                new object[] {
                    (MethodCallExpression)((Expression<Func<MockObjectA, int>>)(x => x.DoubleIt(5))).Body,
                    (MethodCallExpression)((Expression<Func<MockObjectB, int>>)(x => x.TripleIt(5))).Body,
                    false,
                },
                new object[] {
                    (MethodCallExpression)((Expression<Func<MockObjectB, int>>)(x => x.TripleIt(10))).Body,
                    (MethodCallExpression)((Expression<Func<MockObjectB, int>>)(x => x.TripleIt(10))).Body,
                    true,
                },
            };

        public static List<object[]> GetNewExpressionTestData()
            => new List<object[]> {
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    true,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 2, Name = "Matt" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Aimee" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Name = "Matt" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1 })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { x.Id, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { x.Id, x.Name })).Body,
                    true,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { x.Id, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { Id = 1, x.Name })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { x.Id, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, object>>)(x => new { x.Id, Name = "Matt" })).Body,
                    false,
                },
            };

        public static List<object[]> GetNewExpressionWithRowNumberTestData
            => new List<object[]> {
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    true,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = 2, Name = "Matt" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Aimee" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Name = "Matt" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx, Name = "Matt" })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = idx })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    true,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = 2, x.Name })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, Name = "Aimee" })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { x.Name })).Body,
                    false,
                },
                new object[] {
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx, x.Name })).Body,
                    (NewExpression)((Expression<Func<MockObjectA, int, object>>)((x, idx) => new { Id = x.Id + idx })).Body,
                    false,
                },
            };

        public static List<object[]> GetParameterExpressionTestData()
            => new List<object[]> {
                new object[] { Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(int), "x"), true },
                new object[] { Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(int), "y"), false },
                new object[] { Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(int)), false },
                new object[] { Expression.Parameter(typeof(int), "x"), Expression.Parameter(typeof(string), "x"), false },
            };

        public static List<object[]> GetTypeBinaryExpressionTestData
            => new List<object[]> {
                new object[] {
                    (TypeBinaryExpression)((Expression<Func<MockObjectA, bool>>)(x => x is MockObjectA)).Body,
                    (TypeBinaryExpression)((Expression<Func<MockObjectA, bool>>)(x => x is MockObjectA)).Body,
                    true,
                },
                new object[] {
                    (TypeBinaryExpression)((Expression<Func<MockObjectA, bool>>)(x => x is MockObjectA)).Body,
                    (TypeBinaryExpression)((Expression<Func<MockObjectB, bool>>)(x => x is MockObjectB)).Body,
                    false,
                },
                new object[] {
                    (TypeBinaryExpression)((Expression<Func<MockObjectA, bool>>)(x => x is MockObjectA)).Body,
                    (TypeBinaryExpression)((Expression<Func<MockObjectA, bool>>)(y => y is MockObjectA)).Body,
                    false,
                },
                new object[] {
                    (TypeBinaryExpression)((Expression<Func<MockObjectB, bool>>)(x => x is MockObjectB)).Body,
                    (TypeBinaryExpression)((Expression<Func<MockObjectB, bool>>)(x => x is MockObjectB)).Body,
                    true,
                },
            };

        public static List<object[]> GetUnaryExpressionTestData
            => new List<object[]> {
                new object[] {
                    (UnaryExpression)((Expression<Func<int, double>>)(x => x)).Body,
                    (UnaryExpression)((Expression<Func<int, double>>)(x => x)).Body,
                    true,
                },
                new object[] {
                    (UnaryExpression)((Expression<Func<int, double>>)(x => (x + x))).Body,
                    (UnaryExpression)((Expression<Func<int, double>>)(x => (x + x))).Body,
                    true,
                },
                new object[] {
                    (UnaryExpression)((Expression<Func<int, double>>)(x => (x + x))).Body,
                    (UnaryExpression)((Expression<Func<int, double>>)(x => x)).Body,
                    false,
                },
                new object[] {
                    Expression.PostIncrementAssign(Expression.Parameter(typeof(int), "x")),
                    Expression.PostIncrementAssign(Expression.Parameter(typeof(int), "x")),
                    true,
                },
                new object[] {
                    Expression.PostIncrementAssign(Expression.Parameter(typeof(int), "x")),
                    Expression.PostDecrementAssign(Expression.Parameter(typeof(int), "x")),
                    false,
                },
                new object[] {
                    Expression.PostIncrementAssign(Expression.Parameter(typeof(int), "x")),
                    (UnaryExpression)((Expression<Func<int, double>>)(x => x)).Body,
                    false,
                },
            };

        public static List<object[]> GetComplexExpressionTestData
            => new List<object[]> {
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    true,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => false && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id - 2 == 3 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 3 == 3 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 >= 3 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 4 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 && x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name == "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Eggs")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectB, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    false,
                },
                new object[] {
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Id + 2 == 3 || x.Name != "Bacon")),
                    (Expression<Func<MockObjectA, bool>>)(x => true && ( x.Name != "Bacon" || x.Id + 2 == 3 )),
                    false,
                },
            };
        #endregion

        #region Helper Methods
        private bool CompareExpressions(Expression left, Expression right) {

            bool? result = null;
            Exception exception = Record.Exception(() => result = new ExpressionComparisonVisitor(left, right).AreEqual);
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();
            return result.Value;
        }
        #endregion
    }
}