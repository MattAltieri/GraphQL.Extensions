using GraphQL.Extensions.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Internal {
    public class CachedReflectionTest {

        private static IQueryable<MockEntity> baseTestData = (new List<MockEntity> {
            new MockEntity {
                Id = 2,
                Name = "Aimee",
                DOB = DateTime.Parse("1985-07-18"),
            },
            new MockEntity {
                Id = 1,
                Name = "Matt",
                DOB = DateTime.Parse("1981-04-07")
            },
        }).AsQueryable();
        private static Type TSource = typeof(MockEntity);

        private MethodInfo systemUnderTest;

        [Theory]
        [MemberData(nameof(GetOrderByTestData))]
        public void Should_GenerateOrderByMethod_When_OrderByCalled(IQueryable<MockEntity> testData, Type TKey, Expression expression,
            IOrderedQueryable<MockEntity> expectedResults) {

            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.OrderBy(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = ((IQueryable<MockEntity>)systemUnderTest.Invoke(null, new object[] { testData, expression })));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetThenByTestData))]
        public void Should_GenerateThenByMethod_When_ThenByCalled(IOrderedQueryable<MockEntity> testData, Type TKey, Expression expression,
            IOrderedQueryable<MockEntity> expectedResults) {

            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.ThenBy(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = (IQueryable<MockEntity>)systemUnderTest.Invoke(null, new object[] { testData, expression }));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetOrderByDescendingTestData))]
        public void Should_GenerateOrderByDescendingMethod_When_OrderByDescendingCalled(IQueryable<MockEntity> testData, Type TKey,
            Expression expression, IOrderedQueryable<MockEntity> expectedResults) {
            
            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.OrderByDescending(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = ((IQueryable<MockEntity>)systemUnderTest.Invoke(null, new object[] { testData, expression })));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetThenByDescendingTestData))]
        public void Should_GenerateThenByDescendingMethod_When_ThenByDescendingCalled(IOrderedQueryable<MockEntity> testData, Type TKey,
            Expression expression, IOrderedQueryable<MockEntity> expectedResults) {
            
            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.ThenByDescending(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = (IQueryable<MockEntity>)systemUnderTest.Invoke(null, new object[] { testData, expression }));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithInt() {

            ParameterExpression param = Expression.Parameter(TSource);
            
            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.Lambda(TSource, typeof(int)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("Id"));

            Expression<Func<MockEntity, int>> intExpression = null;
            exception = Record.Exception(() =>
                intExpression = (Expression<Func<MockEntity, int>>)systemUnderTest.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }));
                        
            exception.ShouldBeNull();
            intExpression.ShouldNotBeNull();
            
            // expected result
            intExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            intExpression.Type.ShouldBe(typeof(Func<MockEntity, int>));
            intExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)intExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(int));
            body.Expression.NodeType.ShouldBe(ExpressionType.Parameter);
            body.Member.Name.ShouldBe("Id");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithString() {

            ParameterExpression param = Expression.Parameter(TSource);

            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.Lambda(TSource, typeof(string)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("Name"));

            Expression<Func<MockEntity, string>> stringExpression = null;
            exception = Record.Exception(() =>
                stringExpression = (Expression<Func<MockEntity, string>>)systemUnderTest.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }
                ));
            exception.ShouldBeNull();
            stringExpression.ShouldNotBeNull();

            // expected result
            stringExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            stringExpression.Type.ShouldBe(typeof(Func<MockEntity, string>));
            stringExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)stringExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(string));
            body.Member.Name.ShouldBe("Name");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(string));
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithDateTime() {

            ParameterExpression param = Expression.Parameter(TSource);

            Exception exception = Record.Exception(() => systemUnderTest = CachedReflection.Lambda(TSource, typeof(DateTime)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("DOB"));

            Expression<Func<MockEntity, DateTime>> stringExpression = null;
            exception = Record.Exception(() =>
                stringExpression = (Expression<Func<MockEntity, DateTime>>)systemUnderTest.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }
                ));
            exception.ShouldBeNull();
            stringExpression.ShouldNotBeNull();

            // expected result
            stringExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            stringExpression.Type.ShouldBe(typeof(Func<MockEntity, DateTime>));
            stringExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)stringExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(DateTime));
            body.Member.Name.ShouldBe("DOB");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(DateTime));
        }

        public static List<object[]> GetOrderByTestData
            => new List<object[]> {
                new object[] {
                    baseTestData,
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => o.Id)
                },
                new object[] {
                    baseTestData,
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => o.Name)
                },
                new object[] {
                    baseTestData,
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => o.DOB)
                },
            };

        public static List<object[]> GetThenByTestData
            => new List<object[]> {
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.Id),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.Name),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.DOB),
                }
            };

        public static List<object[]> GetOrderByDescendingTestData
            => new List<object[]> {
                new object[] {
                    baseTestData,
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderByDescending(o => o.Id),
                },
                new object[] {
                    baseTestData,
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderByDescending(o => o.Name),
                },
                new object[] {
                    baseTestData,
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderByDescending(o => o.DOB),
                },
            };

        public static List<object[]> GetThenByDescendingTestData
            => new List<object[]> {
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.Id),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.Name),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.DOB),
                }
            };
    }
}