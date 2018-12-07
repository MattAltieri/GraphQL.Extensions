using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Pagination {
    public class SortVisitorTest {
        
        private SortVisitor<MockEntity> systemUnderTest;
        private ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");

        [Theory]
        [MemberData(nameof(GetSingleSortTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitingOrderByInfo(
            string columnName,
            SortDirections sortDirection,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {
            
            systemUnderTest = new SortVisitor<MockEntity>(testData_SingleSort.AsQueryable(), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            OrderByInfo<MockEntity> orderBy = MakeOrderByInfo(columnName, sortDirection, null, systemUnderTest);
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.HasValue.ShouldBeTrue();

            areEqual.Value.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetSingleSortTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitingThenByInfo(
            string columnName,
            SortDirections sortDirection,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {
            
            systemUnderTest = new SortVisitor<MockEntity>(testData_SingleSort.AsQueryable().OrderBy(o => 1), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            ThenByInfo<MockEntity> thenBy = MakeThenByInfo(columnName, sortDirection, null, systemUnderTest);
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(thenBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.HasValue.ShouldBeTrue();

            areEqual.Value.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetSingleSortTestData))]
#pragma warning disable xUnit1026
        public void Should_ThrowNullReferenceException_When_VisitingThenByInfo_On_UnorderedData(
            string columnName,
            SortDirections sortDirection,
            IOrderedQueryable<MockEntity> unused1,
            bool unused2) {
#pragma warning restore xUnit1026

            systemUnderTest = new SortVisitor<MockEntity>(testData_SingleSort.AsQueryable(), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            ThenByInfo<MockEntity> thenBy = MakeThenByInfo(columnName, sortDirection, null, systemUnderTest);
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(thenBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            Assert.Throws<NullReferenceException>(() => results.ToList());
        }

        [Theory]
        [MemberData(nameof(GetDoubleSortTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitingDoubleSort(
            Dictionary<string, SortDirections> sortDetails,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {
            
            systemUnderTest = new SortVisitor<MockEntity>(testData_DoubleSort.AsQueryable(), parameterExpression);

            ThenByInfo<MockEntity> thenBy =
                MakeThenByInfo(sortDetails.ElementAt(1).Key, sortDetails.ElementAt(1).Value, null, systemUnderTest);
            OrderByInfo<MockEntity> orderBy =
                MakeOrderByInfo(sortDetails.ElementAt(0).Key, sortDetails.ElementAt(0).Value, thenBy, systemUnderTest);
            
            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.HasValue.ShouldBeTrue();

            areEqual.Value.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetTripleSortTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitingTripleSort(
            Dictionary<string, SortDirections> sortDetails,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {
            
            systemUnderTest = new SortVisitor<MockEntity>(testData_TripleSort.AsQueryable(), parameterExpression);

            ThenByInfo<MockEntity> thenBy_level2 =
                MakeThenByInfo(sortDetails.ElementAt(2).Key, sortDetails.ElementAt(2).Value, null, systemUnderTest);
            ThenByInfo<MockEntity> thenBy_level1 =
                MakeThenByInfo(sortDetails.ElementAt(1).Key, sortDetails.ElementAt(1).Value, thenBy_level2, systemUnderTest);
            OrderByInfo<MockEntity> orderBy =
                MakeOrderByInfo(sortDetails.ElementAt(0).Key, sortDetails.ElementAt(0).Value, thenBy_level1, systemUnderTest);

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.HasValue.ShouldBeTrue();

            areEqual.Value.ShouldBe(expectedResult);            
        }

        private OrderByInfo<MockEntity> MakeOrderByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            SortVisitor<MockEntity> systemUnderTest) {
            
            Mock<OrderByInfo<MockEntity>> mock = new Mock<OrderByInfo<MockEntity>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);

            MemberInfo memberInfo = typeof(MockEntity).GetMember(columnName).First();
            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);
            mock.Setup(m => m.GetMemberType()).Returns(memberType);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            mock.Setup(m => m.Accept(It.IsAny<SortVisitor<MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        private ThenByInfo<MockEntity> MakeThenByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            SortVisitor<MockEntity> systemUnderTest) {
            
            Mock<ThenByInfo<MockEntity>> mock = new Mock<ThenByInfo<MockEntity>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);

            MemberInfo memberInfo = typeof(MockEntity).GetMember(columnName).First();
            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);
            mock.Setup(m => m.GetMemberType()).Returns(memberType);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            mock.Setup(m => m.Accept(It.IsAny<SortVisitor<MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        public static List<object[]> GetSingleSortTestData()
            => new List<object[]> {
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.Id),
                    true,
                },
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    "Id",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    "Id",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.Id),
                    false,
                },
                new object[] {
                    "Name",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.Name),
                    true,
                },
                new object[] {
                    "Name",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    "Name",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    "Name",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.Name),
                    false,
                },
                new object[] {
                    "DOB",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.DOB),
                    true,
                },
                new object[] {
                    "DOB",
                    SortDirections.Ascending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    "DOB",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    "DOB",
                    SortDirections.Descending,
                    testData_SingleSort.AsQueryable().OrderBy(o => o.DOB),
                    false,
                },
            };

        public static List<object[]> GetDoubleSortTestData
            => new List<object[]> {
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
                    false,
                },
            };

        public static List<object[]> GetTripleSortTestData
            => new List<object[]> {
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Id)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenBy(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.Name)
                        .ThenByDescending(o => o.DOB)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Name)
                        .ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenBy(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderByDescending(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    testData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
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