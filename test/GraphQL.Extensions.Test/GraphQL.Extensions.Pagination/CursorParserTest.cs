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
using GraphQL.Extensions.Internal;

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
        [MemberData(nameof(GetCursorTestData_Single))]
        [MemberData(nameof(GetCursorTestData_Double))]
        [MemberData(nameof(GetCursorTestData_Triple))]
#pragma warning disable xUnit1026
        public void Should_CreateExpressionTree_When_ParsingCursor(
            string cursorValue,
            CursorFilterTypes cursorFilterType,
            string cursorSegmentDelimiter,
            string cursorSubsegmentDelimiter,
            OrderByInfo<MockEntity> orderBy,
            Expression<Func<MockEntity, bool>> expressionTree,
            bool expectedEquality,
            IOrderedQueryable<MockEntity> testData_unused,
            IOrderedQueryable<MockEntity> expectedData_unused) {
#pragma warning restore xUnit1026
            
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

            result.Value.ShouldBe(expectedEquality);
        }

        [Theory]
        [MemberData(nameof(GetCursorTestData_Single))]
        [MemberData(nameof(GetCursorTestData_Double))]
        [MemberData(nameof(GetCursorTestData_Triple))]
#pragma warning disable xUnit1026
        public void Should_CorrectlyFilterData_When_ApplyingCursorPredicate(
            string cursorValue,
            CursorFilterTypes cursorFilterType,
            string cursorSegmentDelimiter,
            string cursorSubsegmentDelimiter,
            OrderByInfo<MockEntity> orderBy,
            Expression<Func<MockEntity, bool>> expressionTree_unused,
            bool expectedEquality_unused,
            IOrderedQueryable<MockEntity> testData,
            IOrderedQueryable<MockEntity> expectedData) {
#pragma warning restore xUnit1026
            
            CursorParser<MockEntity> systemUnderTest = null;
            Exception exception = Record.Exception(() =>
                systemUnderTest = new CursorParser<MockEntity>(cursorValue, cursorFilterType, cursorSegmentDelimiter, cursorSubsegmentDelimiter,
                orderBy));
            exception.ShouldBeNull();
            systemUnderTest.ShouldNotBeNull();

            SortVisitor<MockEntity> sorter = null;
            exception = Record.Exception(() => sorter = new SortVisitor<MockEntity>(testData, parameterExpression));
            exception.ShouldBeNull();
            sorter.ShouldNotBeNull();

            IOrderedQueryable<MockEntity> sortedTestData = null;
            exception = Record.Exception(() => sortedTestData = sorter.Visit(orderBy));
            exception.ShouldBeNull();
            sortedTestData.ShouldNotBeNull();

            Expression<Func<MockEntity, bool>> predicate = null;
            exception = Record.Exception(() => predicate = systemUnderTest.GetFilterPredicate());
            exception.ShouldBeNull();
            predicate.ShouldNotBeNull();

            IQueryable<MockEntity> results = null;
            exception = Record.Exception(() => results = sortedTestData.Where(predicate));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            results.SequenceEqual(expectedData).ShouldBeTrue();
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

        public static List<object[]> GetCursorTestData_Single
            => new List<object[]> {
                new object[] {
                    "id::a::2",
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
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "id::a::1",
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
                                Expression.Constant(1)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .Where(o => o.Id > 1)
                },
                new object[] {
                    "id::a::2",
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
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "id::d::2",
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
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "id::d::2",
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
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "name::a::B",
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
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "name::d::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "name::d::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "name::a::B",
                    CursorFilterTypes.Before,
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
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.Constant(true),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_SingleSort.AsQueryable(),
                    testData_SingleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
            };

        public static List<object[]> GetCursorTestData_Double
            => new List<object[]> {
                new object[] {
                    "id::a::2//name::a::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "id::a::2//name::a::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "id::d::2//name::a::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "id::d::2//name::a::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "id::a::2//name::d::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "id::a::2//name::d::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "id::d::2//name::d::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                },
                new object[] {
                    "id::d::2//name::d::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                },
                new object[] {
                    "id::a::2//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "id::a::2//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "id::d::2//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "id::d::2//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "id::a::2//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "id::a::2//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "id::d::2//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "id::d::2//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("Id")[0]
                                    ),
                                    Expression.Constant(2)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                },
                new object[] {
                    "name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                },
                new object[] {
                    "name::a::B//id::a::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "name::a::B//id::a::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "name::d::B//id::a::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "name::d::B//id::a::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "name::a::B//id::d::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "name::a::B//id::d::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "name::d::B//id::d::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.Id < 2)
                },
                new object[] {
                    "name::d::B//id::d::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.Id > 2)
                },
                new object[] {
                    "dob::a::625225824000000000//id::a::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => o.Id > 2),
                },
                new object[] {
                    "dob::a::625225824000000000//id::a::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => o.Id < 2),
                },
                new object[] {
                    "dob::d::625225824000000000//id::a::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => o.Id > 2),
                },
                new object[] {
                    "dob::d::625225824000000000//id::a::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => o.Id < 2),
                },
                new object[] {
                    "dob::a::625225824000000000//id::d::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => o.Id < 2),
                },
                new object[] {
                    "dob::a::625225824000000000//id::d::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => o.Id > 2),
                },
                new object[] {
                    "dob::d::625225824000000000//id::d::2",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => o.Id < 2),
                },
                new object[] {
                    "dob::d::625225824000000000//id::d::2",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("Id")[0]
                                ),
                                Expression.Constant(2)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => o.Id > 2),
                },
                new object[] {
                    "dob::a::625225824000000000//name::a::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) > 0),
                },
                new object[] {
                    "dob::a::625225824000000000//name::a::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) < 0),
                },
                new object[] {
                    "dob::d::625225824000000000//name::a::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) > 0),
                },
                new object[] {
                    "dob::d::625225824000000000//name::a::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) < 0),
                },
                new object[] {
                    "dob::a::625225824000000000//name::d::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) < 0),
                },
                new object[] {
                    "dob::a::625225824000000000//name::d::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) > 0),
                },
                new object[] {
                    "dob::d::625225824000000000//name::d::B",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.LessThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.LessThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.DOB < new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) < 0),
                },
                new object[] {
                    "dob::d::625225824000000000//name::d::B",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.Constant(true),
                                Expression.GreaterThan(
                                    Expression.MakeMemberAccess(
                                        parameterExpression,
                                        typeof(MockEntity).GetMember("DOB")[0]
                                    ),
                                    Expression.Constant(new DateTime(625225824000000000))
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.Call(
                                    Expression.Constant("B"),
                                    CachedReflection.StringCompareTo(),
                                    new Expression[] {
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Name")[0]
                                        )
                                    }
                                ),
                                Expression.Constant(0)
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_DoubleSort.AsQueryable(),
                    testData_DoubleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .Where(o => o.DOB > new DateTime(625225824000000000))
                        .Where(o => "B".CompareTo(o.Name) > 0),
                },
            };

        public static List<object[]> GetCursorTestData_Triple
            => new List<object[]> {
                new object[] {
                    "id::a::2//name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::a::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::d::B//dob::a::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::a::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::a::2//name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.After,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.LessThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.LessThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.LessThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id < 2)
                        .Where(o => "B".CompareTo(o.Name) < 0)
                        .Where(o => o.DOB < new DateTime(625225824000000000)),
                },
                new object[] {
                    "id::d::2//name::d::B//dob::d::625225824000000000",
                    CursorFilterTypes.Before,
                    "//",
                    "::",
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            }
                        }
                    },
                    Expression.Lambda<Func<MockEntity, bool>>(
                        Expression.AndAlso(
                            Expression.AndAlso(
                                Expression.AndAlso(
                                    Expression.Constant(true),
                                    Expression.GreaterThan(
                                        Expression.MakeMemberAccess(
                                            parameterExpression,
                                            typeof(MockEntity).GetMember("Id")[0]
                                        ),
                                        Expression.Constant(2)
                                    )
                                ),
                                Expression.GreaterThan(
                                    Expression.Call(
                                        Expression.Constant("B"),
                                        CachedReflection.StringCompareTo(),
                                        new Expression[] {
                                            Expression.MakeMemberAccess(
                                                parameterExpression,
                                                typeof(MockEntity).GetMember("Name")[0]
                                            )
                                        }
                                    ),
                                    Expression.Constant(0)
                                )
                            ),
                            Expression.GreaterThan(
                                Expression.MakeMemberAccess(
                                    parameterExpression,
                                    typeof(MockEntity).GetMember("DOB")[0]
                                ),
                                Expression.Constant(new DateTime(625225824000000000))
                            )
                        ),
                        parameterExpression
                    ),
                    true,
                    testData_TripleSort.AsQueryable(),
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .Where(o => o.Id > 2)
                        .Where(o => "B".CompareTo(o.Name) > 0)
                        .Where(o => o.DOB > new DateTime(625225824000000000)),
                },
            };

        private static List<MockEntity> testData_SingleSort = new List<MockEntity> {
            new MockEntity {
                Id = 1,
                Name = "Matt",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "Aimee",
                DOB = DateTime.Parse("1985-07-18"),
            }
        };

        private static List<MockEntity> testData_DoubleSort = new List<MockEntity> {
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "B",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "C",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "D",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "A",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "C",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "D",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "A",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "B",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "D",
                DOB = DateTime.Parse("1983-04-07"),
            },
        };

        private static List<MockEntity> testData_TripleSort = new List<MockEntity> {
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "B",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "C",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "D",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "B",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "C",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "D",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "B",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "C",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "D",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "B",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "C",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 1,
                Name = "D",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "A",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "C",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "D",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "A",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "C",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "D",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "A",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "C",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "D",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "A",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "C",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "D",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "A",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "B",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "D",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "A",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "B",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "D",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "A",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "B",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "D",
                DOB = DateTime.Parse("1983-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "A",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "B",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1984-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "D",
                DOB = DateTime.Parse("1984-04-07"),
            },
        };
    }
}