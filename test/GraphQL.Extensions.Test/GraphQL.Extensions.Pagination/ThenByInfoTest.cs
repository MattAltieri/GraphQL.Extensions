using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit2.Should;

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

        [Theory]
        [MemberData(nameof(GetTestData))]
#pragma warning disable xUnit1026
        public void Should_CalculateCorrectHashCode_When_GetHashCodeCalled(
            ThenByInfo< MockEntity> operand1,
            ThenByInfo< MockEntity> operand2_unused,
            bool expectedEquality_unused,
            int expectedHashCode,
            int expectedDepth_unused) {
#pragma warning restore xUnit1026
            
            int? result = null;
            Exception exception = Record.Exception(() => result = operand1.GetHashCode());
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedHashCode);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
#pragma warning disable xUnit1026        
        public void Should_CalculateEqualityCorrectly_When_ComparingTwoInstances(
            ThenByInfo< MockEntity> operand1,
            ThenByInfo< MockEntity> operand2,
            bool expectedEquality,
            int expectedHashCode_unused,
            int expectedDepth_unused) {
#pragma warning restore xUnit1026
            
            bool? result = null;
            Exception exception = Record.Exception(() => result = operand1.Equals(operand2));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedEquality);
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
#pragma warning disable xUnit1026
        public void Should_CalculateDepthCorrectly_When_DepthCalled(
            ThenByInfo< MockEntity> operand1,
            ThenByInfo< MockEntity> operand2_unused,
            bool expectedEquality_unused,
            int expectedHashCode_unused,
            int expectedDepth) {
#pragma warning restore xUnit1026
            
            int? result = null;
            Exception exception = Record.Exception(() => result = operand1.Depth);
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedDepth);
        }

        public static List<object[]> GetTestData
            => new List<object[]> {
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new ThenByInfo< MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((17 * 173) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
            };
    }
}