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

        private class MockOrderByInfo : OrderByInfoBase<MockEntity> {
            public override IOrderedQueryable<MockEntity> Accept(SortVisitor<MockEntity> visitor)
                => throw new NotImplementedException();
        }
    }
}