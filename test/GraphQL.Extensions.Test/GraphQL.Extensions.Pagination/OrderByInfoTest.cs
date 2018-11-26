using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit.Should;

namespace GraphQL.Extensions.Pagination {
    public class OrderByInfoTest {
        
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
        Mock<OrderByInfoVisitor<MockEntity>> mockVisitor = new Mock<OrderByInfoVisitor<MockEntity>>();
        OrderByInfo<MockEntity> systemUnderTest = new OrderByInfo<MockEntity> {
            ColumnName = "Id",
            SortDirection = SortDirections.Ascending
        };

        public OrderByInfoTest() {

            mockVisitor.Setup(o => o.Query).Returns(testData);
            mockVisitor.Setup(o => o.Parameter).Returns(parameterExpression);
            mockVisitor.Setup(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>())).Verifiable();
            mockVisitor.Setup(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>())).Verifiable();
        }

        [Fact]
        public void Should_CallVisitForOrderByInfo_When_OrderByInfoAcceptCalled() {
            
            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockVisitor.Object));
            exception.ShouldBeNull();
            mockVisitor.Verify(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>()), Times.Exactly(1));
        }

        [Fact]
        public void ShouldNot_CallVisitForThenByInfo_When_OrderByInfoAcceptCalled() {

            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockVisitor.Object));
            exception.ShouldBeNull();
            mockVisitor.Verify(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>()), Times.Never);
        }
    }
}