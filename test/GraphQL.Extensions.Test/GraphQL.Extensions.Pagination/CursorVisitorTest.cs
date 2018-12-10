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
    public class CursorVisitorTest {
        
        private ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");

        private OrderByInfo<MockEntity> MakeOrderByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            CursorVisitor<MockEntity, MockEntity> systemUnderTest
        ) {

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

            mock.Setup(m => m.Accept(It.IsAny<CursorVisitor<MockEntity, MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        private ThenByInfo<MockEntity> MakeThenByInfo(
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<MockEntity> thenBy,
            CursorVisitor<MockEntity, MockEntity> systemUnderTest
        ) {

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

            mock.Setup(m => m.Accept(It.IsAny<CursorVisitor<MockEntity, MockEntity>>()))
                .Returns(() => systemUnderTest.Visit(mock.Object));

            return mock.Object;
        }

        public static List<object[]> GetSingleSortTestData
            => new List<object[]> {
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    "id::a::15",
                    
                }
            };
    }
}