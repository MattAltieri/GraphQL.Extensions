using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Linq.Expressions.Compare;
using Moq;
using GraphQL.Extensions.Test;
using Xunit;
using Xunit2.Should;
using System.Linq;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    public class CursorParserTest {

        private static ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "f");

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullCursorProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>(null, CursorFilterTypes.After,
                    "::", "//", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_EmptyCursorProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("", CursorFilterTypes.After,
                    "::", "//", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_WhiteSpaceCursorProvided() {
            
            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("  ", CursorFilterTypes.After,
                    "::", "//", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullSegmentDelimiterProvided() {
            
            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("abc", CursorFilterTypes.After,
                    null, "//", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_EmptySegmentDelimiterProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("abc", CursorFilterTypes.After,
                    "", "//", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullSubsegmentDelimiterProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("abc", CursorFilterTypes.After,
                    "::", null, new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_EmptySubsegmentDelimiterProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("abc", CursorFilterTypes.After,
                    "::", "", new OrderByInfo<MockEntity>()));
        }

        [Fact]
        public void Should_ThrowArgumentNullException_When_NullOrderByProvided() {

            CursorParser<MockEntity> systemUnderTest = null;
            Assert.Throws<ArgumentNullException>(() =>
                systemUnderTest = new CursorParser<MockEntity>("abc", CursorFilterTypes.After,
                    "::", "//", null));
        }

        [Theory]
        [MemberData(nameof(GetCursorTestData))]
        public void Should_CreateExpressionTree_When_ParsingCursor(
            string cursorValue,
            CursorFilterTypes cursorFilterType,
            string cursorSegmentDelimiter,
            string cursorSubsegmentDelimiter,
            OrderByInfo<MockEntity> orderBy,
            Expression<Func<MockEntity, bool>> expressionTree,
            bool expectedResult) {

            CursorParser<MockEntity> systemUnderTest = null;
            Exception exception = Record.Exception(() =>
                systemUnderTest = new CursorParser<MockEntity>(cursorValue, cursorFilterType, cursorSegmentDelimiter, cursorSubsegmentDelimiter,
                orderBy));
            exception.ShouldBeNull();
            systemUnderTest.ShouldNotBeNull();

            Expression<Func<MockEntity, bool>> resultPredicate = null;
            exception = Record.Exception(() => resultPredicate = systemUnderTest.GetFilterPredicate());
            exception.ShouldBeNull();
            resultPredicate.ShouldNotBeNull();
            
            ExpressionTreeComparer comparer = new ExpressionTreeComparer();
            
            bool? result = null;
            exception = Record.Exception(() =>
                result = comparer.Equals(resultPredicate, expressionTree));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }
        
        private OrderByInfo<MockEntity> MakeOrderByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy) {
            
            Mock<OrderByInfo<MockEntity>> mock = new Mock<OrderByInfo<MockEntity>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);
            mock.Setup(m => m.Depth).Returns((thenBy?.Depth ?? 0) + 1);

            mock.Setup(m => m.GetCursorPrefix(It.IsAny<string>()))
                .Returns(string.Format("{0}::{1}::", columnName.ToLower(), sortDirection == SortDirections.Ascending ? "a" : "d"));
            
            MemberInfo memberInfo = typeof(MockEntity).GetMember(columnName).First();
            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);
            mock.Setup(m => m.GetMemberType()).Returns(memberType);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            return mock.Object;
        }

        private ThenByInfo<MockEntity> MakeThenByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy) {

            Mock<ThenByInfo<MockEntity>> mock = new Mock<ThenByInfo<MockEntity>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);
            mock.Setup(m => m.Depth).Returns((thenBy?.Depth ?? 0) + 1);

            MemberInfo memberInfo = typeof(MockEntity).GetMember(columnName).First();
            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);
            mock.Setup(m => m.GetMemberType()).Returns(memberType);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            return mock.Object;
        }

        public static List<object[]> GetCursorTestData
            => new List<object[]> {
                new object[] {
                    "id::a::3000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(3000)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
                new object[] {
                    "id::a::4000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(4000)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
                new object[] {
                    "id::a::3000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(3000)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
                new object[] {
                    "id::d::3000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(3000)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
                new object[] {
                    "id::d::3000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(3000)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
                new object[] {
                    "name::a::Matt",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Name")[0]
                                ),
                                Expression.Constant("Matt")
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                },
            };
    }
}