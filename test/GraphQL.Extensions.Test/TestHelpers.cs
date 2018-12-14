using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Pagination;
using Moq;

namespace GraphQL.Extensions.Test {
    public static class TestHelpers {
        
        public static OrderByInfo<TSource> MakeOrderByInfo<TSource>(
            ParameterExpression parameterExpression,
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<TSource> thenBy = null,
            SortVisitor<TSource> sortVisitor = null,
            CursorVisitor<TSource> cursorVisitor = null)
            where TSource : class {

            Mock<OrderByInfo<TSource>> mock = new Mock<OrderByInfo<TSource>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);
            mock.Setup(m => m.Depth).Returns((thenBy?.Depth ?? 0) + 1);

            MemberInfo memberInfo = typeof(TSource).GetMember(columnName).First();
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);

            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            mock.Setup(m => m.GetMemberType()).Returns(memberType);

            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            mock.Setup(m => m.GetCursorPrefix(It.IsAny<string>()))
                .Returns(columnName.ToLower() + "::" +
                    (sortDirection == SortDirections.Ascending ? "a" : "d") +
                    "::");

            if (sortVisitor != null)
                mock.Setup(m => m.Accept(It.IsAny<SortVisitor<TSource>>()))
                    .Returns(() => sortVisitor.Visit(mock.Object));

            if (cursorVisitor != null)
                mock.Setup(m => m.Accept(It.IsAny<CursorVisitor<TSource>>()))
                    .Returns(() => cursorVisitor.Visit(mock.Object));

            return mock.Object;
        }

        public static ThenByInfo<TSource> MakeThenByInfo<TSource>(
            ParameterExpression parameterExpression,
            string columnName,
            SortDirections sortDirection,
            ThenByInfo<TSource> thenBy = null,
            SortVisitor<TSource> sortVisitor = null,
            CursorVisitor<TSource> cursorVisitor = null)
            where TSource : class {
            
            Mock<ThenByInfo<TSource>> mock = new Mock<ThenByInfo<TSource>>();

            mock.Setup(m => m.ColumnName).Returns(columnName);
            mock.Setup(m => m.SortDirection).Returns(sortDirection);
            mock.Setup(m => m.ThenBy).Returns(thenBy);
            mock.Setup(m => m.Depth).Returns((thenBy?.Depth ?? 0) + 1);

            MemberInfo memberInfo = typeof(TSource).GetMember(columnName).First();
            mock.Setup(m => m.GetMemberInfo()).Returns(memberInfo);

            Type memberType = (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType;
            mock.Setup(m => m.GetMemberType()).Returns(memberType);

            MemberExpression memberExpression = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            mock.Setup(m => m.GetMemberExpression(It.IsAny<ParameterExpression>())).Returns(memberExpression);

            mock.Setup(m => m.GetCursorPrefix(It.IsAny<string>()))
                .Returns(columnName.ToLower() + "::" +
                    (sortDirection == SortDirections.Ascending ? "a" : "d") +
                    "::");

            if (sortVisitor != null)
                mock.Setup(m => m.Accept(It.IsAny<SortVisitor<TSource>>()))
                    .Returns(() => sortVisitor.Visit(mock.Object));

            if (cursorVisitor != null)
                mock.Setup(m => m.Accept(It.IsAny<CursorVisitor<TSource>>()))
                    .Returns(() => cursorVisitor.Visit(mock.Object));

            return mock.Object;
        }

        public static List<MockEntity> TestData_SingleSort = new List<MockEntity> {
            new MockEntity {
                Id = 1,
                Name = "A",
                DOB = DateTime.Parse("1981-04-07"),
            },
            new MockEntity {
                Id = 2,
                Name = "B",
                DOB = DateTime.Parse("1982-04-07"),
            },
            new MockEntity {
                Id = 3,
                Name = "C",
                DOB = DateTime.Parse("1983-04-07"),
            },
        };

        public static List<MockEntity> TestData_DoubleSort = new List<MockEntity> {
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

        public static List<MockEntity> TestData_TripleSort = new List<MockEntity> {
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