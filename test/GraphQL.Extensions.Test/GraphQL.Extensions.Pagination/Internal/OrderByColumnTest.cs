using GraphQL.Extensions.Test;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Pagination.Internal {
    public class OrderByColumnTest {

        private static ParameterExpression param = Expression.Parameter(typeof(MockEntity));
        
        private OrderByColumn systemUnderTest;

        [Theory]
        [MemberData(nameof(GetValidConstructorArgs))]
        public void Should_Succeed_When_ConstructorCalledWithValidArgs(string columnName, Type entityType, ParameterExpression entityParameter,
            object expectedResult) {

            Exception exception = Record.Exception(() => systemUnderTest = new OrderByColumn(columnName, entityType, entityParameter));
            exception.ShouldBeNull();

            OrderByColumn _expectedResult = (OrderByColumn)expectedResult;
            systemUnderTest.Name.ShouldBe(_expectedResult.Name);
            systemUnderTest.Type.ShouldBe(_expectedResult.Type);
            systemUnderTest.MemberInfo.ShouldBe(_expectedResult.MemberInfo);
            systemUnderTest.SortDirection.ShouldBe(_expectedResult.SortDirection);

            systemUnderTest.MemberExpression.Member.MemberType.ShouldBe(_expectedResult.MemberExpression.Member.MemberType);
            systemUnderTest.MemberExpression.Member.DeclaringType.ShouldBe(_expectedResult.MemberExpression.Member.DeclaringType);
            systemUnderTest.MemberExpression.Member.Name.ShouldBe(_expectedResult.MemberExpression.Member.Name);
            systemUnderTest.MemberExpression.Expression.NodeType.ShouldBe(_expectedResult.MemberExpression.Expression.NodeType);
            systemUnderTest.MemberExpression.Expression.Type.ShouldBe(_expectedResult.MemberExpression.Expression.Type);
            ((ParameterExpression)systemUnderTest.MemberExpression.Expression).Name.ShouldBe(((ParameterExpression)_expectedResult.MemberExpression.Expression).Name);
        }

        [Theory]
        [MemberData(nameof(GetInvalidConstructorArgs))]
        public void Should_Fail_When_ConstructorCalledWithInvalidArgs(string columnName, Type entityType, ParameterExpression entityParameter,
            Type exceptionType) {

            Assert.Throws(exceptionType, () => systemUnderTest = new OrderByColumn(columnName, entityType, entityParameter));
        }

        public static List<object[]> GetValidConstructorArgs
            => new List<object[]> {
                new object[] {
                    "Id",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Id",
                        Type = typeof(int),
                        MemberInfo = typeof(MockEntity).GetMember("Id")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Id")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Id asc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Id",
                        Type = typeof(int),
                        MemberInfo = typeof(MockEntity).GetMember("Id")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Id")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Id ascending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Id",
                        Type = typeof(int),
                        MemberInfo = typeof(MockEntity).GetMember("Id")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Id")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Id desc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Id",
                        Type = typeof(int),
                        MemberInfo = typeof(MockEntity).GetMember("Id")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Id")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
                new object[] {
                    "Id descending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Id",
                        Type = typeof(int),
                        MemberInfo = typeof(MockEntity).GetMember("Id")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Id")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
                new object[] {
                    "Name",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Name",
                        Type = typeof(string),
                        MemberInfo = typeof(MockEntity).GetMember("Name")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Name")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Name asc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Name",
                        Type = typeof(string),
                        MemberInfo = typeof(MockEntity).GetMember("Name")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Name")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Name ascending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Name",
                        Type = typeof(string),
                        MemberInfo = typeof(MockEntity).GetMember("Name")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Name")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "Name desc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Name",
                        Type = typeof(string),
                        MemberInfo = typeof(MockEntity).GetMember("Name")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Name")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
                new object[] {
                    "Name descending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "Name",
                        Type = typeof(string),
                        MemberInfo = typeof(MockEntity).GetMember("Name")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("Name")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
                new object[] {
                    "DOB",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "DOB",
                        Type = typeof(DateTime),
                        MemberInfo = typeof(MockEntity).GetMember("DOB")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("DOB")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "DOB asc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "DOB",
                        Type = typeof(DateTime),
                        MemberInfo = typeof(MockEntity).GetMember("DOB")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("DOB")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "DOB ascending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "DOB",
                        Type = typeof(DateTime),
                        MemberInfo = typeof(MockEntity).GetMember("DOB")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("DOB")[0]),
                        SortDirection = SortDirections.Ascending
                    }
                },
                new object[] {
                    "DOB desc",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "DOB",
                        Type = typeof(DateTime),
                        MemberInfo = typeof(MockEntity).GetMember("DOB")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("DOB")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
                new object[] {
                    "DOB descending",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    new OrderByColumn {
                        Name = "DOB",
                        Type = typeof(DateTime),
                        MemberInfo = typeof(MockEntity).GetMember("DOB")[0],
                        MemberExpression = Expression.MakeMemberAccess(param, typeof(MockEntity).GetMember("DOB")[0]),
                        SortDirection = SortDirections.Descending
                    }
                },
            };

        public static List<object[]> GetInvalidConstructorArgs
            => new List<object[]> {
                new object[] {
                    "Idd",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "Id acs",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "Id ascneding",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "Id decs",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "Id descneding",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "Namme",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                },
                new object[] {
                    "DBO",
                    typeof(MockEntity),
                    Expression.Parameter(typeof(MockEntity)),
                    typeof(ArgumentException)
                }
            };
    }
}