using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit2.Should;

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
            
            systemUnderTest = new SortVisitor<MockEntity>(TestHelpers.TestData_SingleSort.AsQueryable(), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            OrderByInfo<MockEntity> orderBy =
                TestHelpers.MakeOrderByInfo<MockEntity, MockEntity>(parameterExpression, columnName, sortDirection, null,
                sortVisitor: systemUnderTest);
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
            
            systemUnderTest = new SortVisitor<MockEntity>(TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => 1), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            ThenByInfo<MockEntity> thenBy =
                TestHelpers.MakeThenByInfo<MockEntity, MockEntity>(parameterExpression, columnName, sortDirection, null,
                sortVisitor: systemUnderTest);
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

            systemUnderTest = new SortVisitor<MockEntity>(TestHelpers.TestData_SingleSort.AsQueryable(), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            ThenByInfo<MockEntity> thenBy =
                TestHelpers.MakeThenByInfo<MockEntity, MockEntity>(parameterExpression, columnName, sortDirection, null,
                sortVisitor: systemUnderTest);
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
            
            systemUnderTest = new SortVisitor<MockEntity>(TestHelpers.TestData_DoubleSort.AsQueryable(), parameterExpression);

            ThenByInfo<MockEntity> thenBy =
                TestHelpers.MakeThenByInfo<MockEntity, MockEntity>(parameterExpression, sortDetails.ElementAt(1).Key,
                sortDetails.ElementAt(1).Value, null, sortVisitor: systemUnderTest);
            OrderByInfo<MockEntity> orderBy =
                TestHelpers.MakeOrderByInfo<MockEntity, MockEntity>(parameterExpression, sortDetails.ElementAt(0).Key,
                sortDetails.ElementAt(0).Value, thenBy, sortVisitor: systemUnderTest);
            
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
            
            systemUnderTest = new SortVisitor<MockEntity>(TestHelpers.TestData_TripleSort.AsQueryable(), parameterExpression);

            ThenByInfo<MockEntity> thenBy_level2 =
                TestHelpers.MakeThenByInfo<MockEntity, MockEntity>(parameterExpression, sortDetails.ElementAt(2).Key,
                sortDetails.ElementAt(2).Value, null, sortVisitor: systemUnderTest);
            ThenByInfo<MockEntity> thenBy_level1 =
                TestHelpers.MakeThenByInfo<MockEntity, MockEntity>(parameterExpression, sortDetails.ElementAt(1).Key,
                sortDetails.ElementAt(1).Value, thenBy_level2, sortVisitor: systemUnderTest);
            OrderByInfo<MockEntity> orderBy =
                TestHelpers.MakeOrderByInfo<MockEntity, MockEntity>(parameterExpression, sortDetails.ElementAt(0).Key,
                sortDetails.ElementAt(0).Value, thenBy_level1, sortVisitor: systemUnderTest);

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

        public static List<object[]> GetSingleSortTestData
            => new List<object[]> {
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.Id),
                    true,
                },
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    "Id",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    "Id",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.Id),
                    false,
                },
                new object[] {
                    "Name",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.Name),
                    true,
                },
                new object[] {
                    "Name",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    "Name",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    "Name",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.Name),
                    false,
                },
                new object[] {
                    "DOB",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.DOB),
                    true,
                },
                new object[] {
                    "DOB",
                    SortDirections.Ascending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    "DOB",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    "DOB",
                    SortDirections.Descending,
                    TestHelpers.TestData_SingleSort.AsQueryable().OrderBy(o => o.DOB),
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
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Id).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Id", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Id).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenBy(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Id", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Ascending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenBy(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenBy(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.Name).ThenByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "Name", SortDirections.Descending },
                        { "DOB", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.Name).ThenByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Ascending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Ascending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenBy(o => o.Name),
                    false,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderByDescending(o => o.DOB).ThenByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    new Dictionary<string, SortDirections> {
                        { "DOB", SortDirections.Descending },
                        { "Name", SortDirections.Descending },
                    },
                    TestHelpers.TestData_DoubleSort.AsQueryable().OrderBy(o => o.DOB).ThenByDescending(o => o.Name),
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
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
                    TestHelpers.TestData_TripleSort.AsQueryable()
                        .OrderBy(o => o.DOB)
                        .ThenByDescending(o => o.Id)
                        .ThenBy(o => o.Name),
                    false,
                },
            };
    }
}