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
    public class OrderByInfoVisitorTest {


        static IQueryable<MockEntity> testData = (new List<MockEntity> {
            new MockEntity {
                Id = 1,
                Name = "Matt",
                DOB = DateTime.Parse("1981-04-07")
            },
            new MockEntity {
                Id = 2,
                Name = "Aimee",
                DOB = DateTime.Parse("1985-07-18")
            }
        }).AsQueryable();
        static ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");
        // Mock<OrderByInfo<MockEntity>> mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
        // Mock<ThenByInfo<MockEntity>> mockThenBy1 = new Mock<ThenByInfo<MockEntity>>();
        // Mock<ThenByInfo<MockEntity>> mockThenBy2 = new Mock<ThenByInfo<MockEntity>>();
        static OrderByInfoVisitor<MockEntity> systemUnderTest;

        static OrderByInfoVisitorTest() {

            systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData, parameterExpression);
        }

        public OrderByInfoVisitorTest() {

            // systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData, parameterExpression);

            // mockThenBy2.Setup(o => o.ColumnName).Returns("DOB");
            // mockThenBy2.Setup(o => o.SortDirection).Returns(SortDirections.Descending);
            // mockThenBy2.Setup(o => o.ThenBy).Returns<ThenByInfo<MockEntity>>(null);

            // mockThenBy1.Setup(o => o.ColumnName).Returns("Name");
            // mockThenBy1.Setup(o => o.SortDirection).Returns(SortDirections.Ascending);
            // mockThenBy1.Setup(o => o.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // // mockThenBy1.Setup(o => o.ThenBy).Returns(mockThenBy2.Object);
            // mockThenBy1.Setup(o => o.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockThenBy1.Object));
            
            // mockOrderBy.Setup(o => o.ColumnName).Returns("Id");
            // mockOrderBy.Setup(o => o.SortDirection).Returns(SortDirections.Ascending);
            // mockOrderBy.Setup(o => o.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // // mockOrderBy.Setup(o => o.ThenBy).Returns(mockThenBy1.Object);
            // mockOrderBy.Setup(o => o.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
        }

        [Theory]
        [MemberData(nameof(GetSingleOrderByTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitOrderByInfoCalled(OrderByInfo<MockEntity> orderBy, IOrderedQueryable<MockEntity> expectedResults) {

            IOrderedQueryable<MockEntity> results = null;
            Exception exception = Record.Exception(() => results = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            results.ShouldNotBeNull();

            results.SequenceEqual(expectedResults).ShouldBe(true);
        }

        public static List<object[]> GetSingleOrderByTestData() {

            List<object[]> results = new List<object[]>();

            // Mock<OrderByInfo<MockEntity>> mockOrderBy = new Mock<Pagination.OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("Id");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Ascending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                MakeOrderByInfo("Id", SortDirections.Ascending, null),
                testData.OrderBy(o => o.Id)
            });

            // mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("Id");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Descending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                MakeOrderByInfo("Id", SortDirections.Descending, null),
                testData.OrderByDescending(o => o.Id)
            });

            // mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("Name");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Ascending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                
                MakeOrderByInfo("Name", SortDirections.Ascending, null),
                testData.OrderBy(o => o.Name)
            });

            // mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("Name");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Descending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                
                MakeOrderByInfo("Name", SortDirections.Descending, null),
                testData.OrderByDescending(o => o.Name)
            });

            // mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("DOB");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Ascending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                MakeOrderByInfo("DOB", SortDirections.Ascending, null),
                testData.OrderBy(o => o.DOB)
            });

            // mockOrderBy = new Mock<OrderByInfo<MockEntity>>();
            // mockOrderBy.Setup(m => m.ColumnName).Returns("DOB");
            // mockOrderBy.Setup(m => m.SortDirection).Returns(SortDirections.Descending);
            // mockOrderBy.Setup(m => m.ThenBy).Returns<ThenByInfo<MockEntity>>(null);
            // mockOrderBy.Setup(m => m.Accept(It.IsAny<OrderByInfoVisitor<MockEntity>>()))
            //     .Returns(systemUnderTest.Visit(mockOrderBy.Object));
            results.Add(new object[] {
                MakeOrderByInfo("DOB", SortDirections.Descending, null),
                testData.OrderByDescending(o => o.DOB)
            });

            return results;
        }

        private static OrderByInfo<MockEntity> MakeOrderByInfo(string columnName, SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy) {
            
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
                .Returns(systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        

        private static ThenByInfo<MockEntity> MakeThenByInfo(string columnName, SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy) {
            
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
                .Returns(systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }
    }
}