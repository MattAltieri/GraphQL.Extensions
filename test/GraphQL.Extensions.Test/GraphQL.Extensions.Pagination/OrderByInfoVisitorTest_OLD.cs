using Moq;
using Xunit;
using Xunit.Should;
using GraphQL.Extensions.Test;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    public class OrderByInfoVisitorTest_OLD {
        
        private static ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");

        [Theory]
        [MemberData(nameof(GetOrderByTestData_OneLevelDeep))]
        public void Should_ReturnOrderedQueryable_When_VisitOrderByInfoCalled(
            OrderByInfoVisitor<MockEntity> systemUnderTest,
            OrderByInfo<MockEntity> orderBy,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.ShouldNotBeNull();
            areEqual.ShouldBe(expectedResult);
        }


        [Theory]
        [MemberData(nameof(GetThenByTestData_OneLevelDeep))]
        public void Should_ReturnOrderedQueryable_When_VisitThenByInfoCalled(
            OrderByInfoVisitor<MockEntity> systemUnderTest,
            ThenByInfo<MockEntity> thenBy,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {

            // systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData.OrderBy(o => 1), parameterExpression);

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(thenBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.ShouldNotBeNull();
            areEqual.ShouldBe(expectedResult);
        }
/*
        [Theory]
        [MemberData(nameof(GetThenByTestData_OneLevelDeep))]
        public void Should_ThrowException_When_VisitThenByInfoCalledOnNonOrderedData(ThenByInfo<MockEntity> thenBy,
            IOrderedQueryable<MockEntity> ignore1, bool ignore2) {

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(thenBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            Assert.Throws<NullReferenceException>(() => results.ToList());
        }

        [Theory]
        [MemberData(nameof(GetTestData_TwoLevelsDeep))]
        public void Should_ReturnOrderedQueryable_When_VisitingMultiLevelOrderByInfo(OrderByInfo<MockEntity> orderBy,
            IOrderedQueryable<MockEntity> compareTo, bool expectedResult) {

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            bool? areEqual = null;

            
            exception = Record.Exception(() => areEqual = results.SequenceEqual(compareTo));
            exception.ShouldBeNull();
            areEqual.ShouldNotBeNull();
            areEqual.ShouldBe(expectedResult);
        }
*/
        public static List<object[]> GetOrderByTestData_OneLevelDeep() {
            
            IQueryable<MockEntity> testData = new List<MockEntity> {
                new MockEntity {
                    Id = 1,
                    Name = "Matt",
                    DOB = DateTime.Parse("1981-04-07"),
                },
                new MockEntity {
                    Id = 2,
                    Name = "Aimee",
                    DOB = DateTime.Parse("1985-07-18"),
                },
            }.AsQueryable();

            OrderByInfoVisitor<MockEntity> systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData, parameterExpression);

            return new List<object[]> {
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Id", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.Id),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Id", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Id", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Id", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.Id),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Name", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.Name),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Name", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Name", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("Name", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.Name),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("DOB", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.DOB),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("DOB", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("DOB", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeOrderByInfo("DOB", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.DOB),
                    false,
                },
            };
        }


        public static List<object[]> GetThenByTestData_OneLevelDeep() {
            
            IQueryable<MockEntity> testData = new List<MockEntity> {
                new MockEntity {
                    Id = 1,
                    Name = "Matt",
                    DOB = DateTime.Parse("1981-04-07"),
                },
                new MockEntity {
                    Id = 2,
                    Name = "Aimee",
                    DOB = DateTime.Parse("1985-07-18"),
                },
            }.AsQueryable();

            OrderByInfoVisitor<MockEntity> systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData.OrderBy(o => 1), parameterExpression);

            return new List<object[]> {
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Id", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.Id),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Id", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Id),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Id", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Id),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Id", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.Id),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Name", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.Name),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Name", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Name),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Name", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.Name),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("Name", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.Name),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("DOB", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderBy(o => o.DOB),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("DOB", SortDirections.Ascending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.DOB),
                    false,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("DOB", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderByDescending(o => o.DOB),
                    true,
                },
                new object[] {
                    systemUnderTest,
                    MakeThenByInfo("DOB", SortDirections.Descending, null, systemUnderTest),
                    testData.OrderBy(o => o.DOB),
                    false,
                },
            };
        }

/*
            => new List<object[]> {
                new object[] {
                    MakeThenByInfo("Id", SortDirections.Ascending, null),
                    testData.OrderBy(o => o.Id),
                    true
                },
                new object[] {
                    MakeThenByInfo("Id", SortDirections.Ascending, null),
                    testData.OrderByDescending(o => o.Id),
                    false
                },
                new object[] {
                    MakeThenByInfo("Id", SortDirections.Descending, null),
                    testData.OrderByDescending(o => o.Id),
                    true
                },
                new object[] {
                    MakeThenByInfo("Id", SortDirections.Descending, null),
                    testData.OrderBy(o => o.Id),
                    false
                },
                new object[] {
                    MakeThenByInfo("Name", SortDirections.Ascending, null),
                    testData.OrderBy(o => o.Name),
                    true
                },
                new object[] {
                    MakeThenByInfo("Name", SortDirections.Ascending, null),
                    testData.OrderByDescending(o => o.Name),
                    false
                },
                new object[] {
                    MakeThenByInfo("Name", SortDirections.Descending, null),
                    testData.OrderByDescending(o => o.Name),
                    true
                },
                new object[] {
                    MakeThenByInfo("Name", SortDirections.Descending, null),
                    testData.OrderBy(o => o.Name),
                    false
                },
                new object[] {
                    MakeThenByInfo("DOB", SortDirections.Ascending, null),
                    testData.OrderBy(o => o.DOB),
                    true
                },
                new object[] {
                    MakeThenByInfo("DOB", SortDirections.Ascending, null),
                    testData.OrderByDescending(o => o.DOB),
                    false
                },
                new object[] {
                    MakeThenByInfo("DOB", SortDirections.Descending, null),
                    testData.OrderByDescending(o => o.DOB),
                    true
                },
                new object[] {
                    MakeThenByInfo("DOB", SortDirections.Descending, null),
                    testData.OrderBy(o => o.DOB),
                    false
                }
            };

        public static List<object[]> GetTestData_TwoLevelsDeep
            => new List<object[]> {
                new object[] {
                    MakeOrderByInfo(
                        "Id",
                        SortDirections.Ascending,
                        MakeThenByInfo("Name", SortDirections.Ascending, null)
                    ),
                    testData.OrderBy(o => o.Id).ThenBy(o => o.Name),
                    true,
                },
                new object[] {
                    MakeOrderByInfo(
                        "Id",
                        SortDirections.Ascending,
                        MakeThenByInfo("Name", SortDirections.Ascending, null)
                    ),
                    testData.OrderBy(o => o.Id).ThenByDescending(o => o.Name),
                    false,
                },
            };

        */

        private static OrderByInfo<MockEntity> MakeOrderByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            OrderByInfoVisitor<MockEntity> systemUnderTest) {
            
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

            mock.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        private static ThenByInfo<MockEntity> MakeThenByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            OrderByInfoVisitor<MockEntity> systemUnderTest) {
            
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

            mock.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }
    }
}