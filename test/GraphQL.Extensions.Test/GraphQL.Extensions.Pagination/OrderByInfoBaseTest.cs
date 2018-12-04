using GraphQL.Extensions.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Pagination {
    
    public class OrderByInfoBaseTest {

        private static Type TSource = typeof(MockEntity);
        private static ParameterExpression Parameter = Expression.Parameter(TSource);

        private MockOrderByInfo systemUnderTest;

        [Theory]
        [MemberData(nameof(GetMemberInfoTestData))]
        public void Should_ReturnMemberInfo_When_GetMemberInfoCalled(string propertyName, PropertyInfo expectedResult) {

            systemUnderTest = new MockOrderByInfo {
                ColumnName = propertyName
            };

            MemberInfo member = null;
            Exception exception = Record.Exception(() => member = systemUnderTest.GetMemberInfo());
            exception.ShouldBeNull();
            member.ShouldNotBeNull();

            member.Name.ShouldBe(expectedResult.Name);
            member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)member).PropertyType.ShouldBe(expectedResult.PropertyType);
        }
        
        [Theory]
        [MemberData(nameof(GetMemberExpressionTestData))]
        public void Should_ReturnMemberExpression_When_GetMemberExpressionCalled(ParameterExpression param, string propertyName, MemberExpression expectedResult) {

            systemUnderTest = new MockOrderByInfo {
                ColumnName = propertyName
            };

            MemberExpression memberExpression = null;
            Exception exception = Record.Exception(() => memberExpression = systemUnderTest.GetMemberExpression(param));
            exception.ShouldBeNull();
            memberExpression.ShouldNotBeNull();

            memberExpression.Type.ShouldBe(expectedResult.Type);
            ((ParameterExpression)memberExpression.Expression).Type.ShouldBe(((ParameterExpression)expectedResult.Expression).Type);
            memberExpression.Member.Name.ShouldBe(expectedResult.Member.Name);
            memberExpression.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)memberExpression.Member).PropertyType.ShouldBe(((PropertyInfo)memberExpression.Member).PropertyType);
        }

        [Theory]
        [MemberData(nameof(GetCursorPrefixTestData))]
        public void Should_ReturnCursorPrefix_When_GetCursorPrefixCalled(OrderByInfo<MockEntity> orderBy, string cursorDelim, string expectedResult) {

            string result = null;
            Exception exception = Record.Exception(() => result = orderBy.GetCursorPrefix(cursorDelim));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        public static List<object[]> GetMemberInfoTestData()
            => new List<object[]> {
                new object[] {
                    "Id",
                    TSource.GetProperty("Id")
                },
                new object[] {
                    "Name",
                    typeof(MockEntity).GetProperty("Name")
                },
                new object[] {
                    "DOB",
                    typeof(MockEntity).GetProperty("DOB")
                }
            };
        
        public static List<object[]> GetMemberExpressionTestData()
            => new List<object[]> {
                new object[] {
                    Parameter,
                    "Id",
                    Expression.MakeMemberAccess(Parameter, TSource.GetProperty("Id")),
                },
                new object[] {
                    Parameter,
                    "Name",
                    Expression.MakeMemberAccess(Parameter, TSource.GetProperty("Name")),
                },
                new object[] {
                    Parameter,
                    "DOB",
                    Expression.MakeMemberAccess(Parameter, TSource.GetProperty("DOB")),
                },
            };

        public static List<object[]> GetCursorPrefixTestData
            => new List<object[]> {
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    "::",
                    "id::a::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    "::",
                    "id::d::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    "::",
                    "name::a::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    "::",
                    "name::d::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    "::",
                    "dob::a::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "DOB",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    "::",
                    "dob::d::"
                },
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    ",.?$/",
                    "id,.?$/a,.?$/"
                },
            };

        public static List<object[]> GetTestDataForAddKeyColumnTest
            => new List<object[]> {
                new object[] {
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    
                },
            };

        private class MockOrderByInfo : OrderByInfoBase<MockEntity> {
            public override IOrderedQueryable<MockEntity> Accept(SortVisitor<MockEntity> visitor)
                => throw new NotImplementedException();

            public override Cursor Accept<TResult>(CursorVisitor<MockEntity, TResult> visitor)
                => throw new NotImplementedException();
        }
    }
}