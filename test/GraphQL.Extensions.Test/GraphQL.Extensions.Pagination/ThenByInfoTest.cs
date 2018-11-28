using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Pagination {
    public class ThenByInfoTest {
        
        
        IQueryable<MockEntity> testData = (new List<MockEntity> {
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
        ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");
        Mock<SortVisitor<MockEntity>> mockVisitor = new Mock<SortVisitor<MockEntity>>();
        OrderByInfo<MockEntity> systemUnderTest = new OrderByInfo<MockEntity> {
            ColumnName = "Id",
            SortDirection = SortDirections.Ascending,
            ThenBy = new ThenByInfo<MockEntity> {
                ColumnName = "Name",
                SortDirection = SortDirections.Descending
            }
        };

        public ThenByInfoTest() {

            mockVisitor.Setup(o => o.Query).Returns(testData);
            mockVisitor.Setup(o => o.Parameter).Returns(parameterExpression);
            mockVisitor.Setup(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>())).Verifiable();
            mockVisitor.Setup(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>())).Verifiable();
        }

        [Fact]
        public void Should_CallVisitForThenByInfo_When_ThenByInfoAcceptCalled() {
            Exception exception = Record.Exception(() => systemUnderTest.ThenBy.Accept(mockVisitor.Object));
            exception.ShouldBeNull();
            mockVisitor.Verify(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>()), Times.Exactly(1));
        }

        [Fact]
        public void ShouldNot_CallVisitForOrderByInfo_When_ThenByInfoAcceptCalled() {
            Exception exception = Record.Exception(() => systemUnderTest.ThenBy.Accept(mockVisitor.Object));
            exception.ShouldBeNull();
            mockVisitor.Verify(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>()), Times.Never);
        }
    }
}