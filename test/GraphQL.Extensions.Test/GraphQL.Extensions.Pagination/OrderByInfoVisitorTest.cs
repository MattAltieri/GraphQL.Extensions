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
    public class OrderByInfoVisitorTest {
        
        private OrderByInfoVisitor<MockEntity> systemUnderTest;
        private ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");

        [Theory]
        [MemberData(nameof(GetSingleSortTestData))]
        public void Should_ReturnOrderedQueryable_When_VisitingSingleSort(
            string columnName,
            SortDirections sortDirection,
            IOrderedQueryable<MockEntity> compareTo,
            bool expectedResult) {
            
            systemUnderTest = new OrderByInfoVisitor<MockEntity>(testData_SingleSort.AsQueryable(), parameterExpression);

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

        private OrderByInfo<MockEntity> MakeOrderByInfo(
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

        private ThenByInfo<MockEntity> MakeThenByInfo(
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

        private List<MockEntity> testData_DoubleSort = new List<MockEntity> {
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

        private List<MockEntity> testData_TripleSort = new List<MockEntity> {
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