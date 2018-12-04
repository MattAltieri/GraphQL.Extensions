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
        Mock<SortVisitor<MockEntity>> mockSortVisitor = new Mock<SortVisitor<MockEntity>>();
        Mock<CursorVisitor<MockEntity, MockEntity>> mockCursorVisitor =
            new Mock<CursorVisitor<MockEntity, MockEntity>>();
        OrderByInfo<MockEntity> systemUnderTest = new OrderByInfo<MockEntity> {
            ColumnName = "Id",
            SortDirection = SortDirections.Ascending
        };

        public OrderByInfoTest() {

            mockSortVisitor.Setup(o => o.Query).Returns(testData);
            mockSortVisitor.Setup(o => o.Parameter).Returns(parameterExpression);
            mockSortVisitor.Setup(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>())).Verifiable();
            mockSortVisitor.Setup(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>())).Verifiable();

            mockCursorVisitor.Setup(m => m.Index).Returns(0);
            mockCursorVisitor.Setup(m => m.Parameter).Returns(parameterExpression);
            mockCursorVisitor.Setup(m => m.Visit(It.IsAny<OrderByInfo<MockEntity>>())).Verifiable();
            mockCursorVisitor.Setup(m => m.Visit(It.IsAny<ThenByInfo<MockEntity>>())).Verifiable();
        }

        [Fact]
        public void Should_CallAccept_When_SortVisitorIsPassedOrderByInfo() {
            
            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockSortVisitor.Object));
            exception.ShouldBeNull();
            mockSortVisitor.Verify(o => o.Visit(It.IsAny<OrderByInfo<MockEntity>>()), Times.Exactly(1));
        }

        [Fact]
        public void Should_NotCallAccept_When_SortVisitorIsPassedThenByInfo() {

            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockSortVisitor.Object));
            exception.ShouldBeNull();
            mockSortVisitor.Verify(o => o.Visit(It.IsAny<ThenByInfo<MockEntity>>()), Times.Never);
        }

        [Fact]
        public void Should_CallAccept_When_CursorVisitorIsPassedOrderByInfo() {

            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockCursorVisitor.Object));
            exception.ShouldBeNull();
            mockCursorVisitor.Verify(m => m.Visit(It.IsAny<OrderByInfo<MockEntity>>()), Times.Exactly(1));
        }

        [Fact]
        public void Should_NotCallAccept_When_CursorVisitorIsPassedThenByInfo() {

            Exception exception = Record.Exception(() => systemUnderTest.Accept(mockCursorVisitor.Object));
            exception.ShouldBeNull();
            mockCursorVisitor.Verify(m => m.Visit(It.IsAny<ThenByInfo<MockEntity>>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(GetTestData_SingleSort))]
        [MemberData(nameof(GetTestData_DoubleSort))]
        [MemberData(nameof(GetTestData_TripleSort))]
        public void Should_CalculateCorrectHashCode_When_GetHashCodeCalled(
            OrderByInfo<MockEntity> operand1,
            OrderByInfo<MockEntity> operand2_unused,
            bool expectedEquality_unused,
            int expectedHashCode,
            int expectedDepth_unused) {
            
            int? result = null;
            Exception exception = Record.Exception(() => result = operand1.GetHashCode());
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedHashCode);
        }

        [Theory]
        [MemberData(nameof(GetTestData_SingleSort))]
        [MemberData(nameof(GetTestData_DoubleSort))]
        [MemberData(nameof(GetTestData_TripleSort))]
        public void Should_CalculateEqualityCorrectly_When_ComparingTwoInstances(
            OrderByInfo<MockEntity> operand1,
            OrderByInfo<MockEntity> operand2,
            bool expectedEquality,
            int expectedHashCode_unused,
            int expectedDepth_unused) {
            
            bool? result = null;
            Exception exception = Record.Exception(() => result = operand1.Equals(operand2));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedEquality);
        }

        [Theory]
        [MemberData(nameof(GetTestData_SingleSort))]
        [MemberData(nameof(GetTestData_DoubleSort))]
        [MemberData(nameof(GetTestData_TripleSort))]
        public void Should_CalculateDepthCorrectly_When_DepthCalled(
            OrderByInfo<MockEntity> operand1,
            OrderByInfo<MockEntity> operand2_unused,
            bool expectedEquality_unused,
            int expectedHashCode_unused,
            int expectedDepth) {
            
            int? result = null;
            Exception exception = Record.Exception(() => result = operand1.Depth);
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBe(expectedDepth);
        }

        public static List<object[]> GetTestData_SingleSort
            => new List<object[]> {
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
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
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Name".GetHashCode() +
                        SortDirections.Ascending.GetHashCode()),
                    // expectedDepth (of operand1)
                    1
                },
            };

        public static List<object[]> GetTestData_DoubleSort
            => new List<object[]> {
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
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
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "DOB",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null,
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = null,
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked((179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        ((17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode())),
                    // expectedDepth
                    2,
                },
            };

        public static List<object[]> GetTestData_TripleSort
            => new List<object[]> {
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    true,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Descending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Descending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Descending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Descending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Name",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Id",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
                new object[] {
                    // operand1
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = new ThenByInfo<MockEntity> {
                                ColumnName = "DOB",
                                SortDirection = SortDirections.Ascending,
                                ThenBy = null,
                            },
                        },
                    },
                    // operand2
                    new OrderByInfo<MockEntity> {
                        ColumnName = "Id",
                        SortDirection = SortDirections.Ascending,
                        ThenBy = new ThenByInfo<MockEntity> {
                            ColumnName = "Name",
                            SortDirection = SortDirections.Ascending,
                            ThenBy = null
                        },
                    },
                    // expectedEquality
                    false,
                    // expectedHashCode (of operand1)
                    unchecked(
                        (179 * 113) +
                        "Id".GetHashCode() +
                        SortDirections.Ascending.GetHashCode() +
                        (
                            (17 * 173) +
                            "Name".GetHashCode() +
                            SortDirections.Ascending.GetHashCode() +
                            (
                                (17 * 173) +
                                "DOB".GetHashCode() +
                                SortDirections.Ascending.GetHashCode()
                            )
                        )
                    ),
                    // expectedDepth (of operand1)
                    3,
                },
            };
    }
}