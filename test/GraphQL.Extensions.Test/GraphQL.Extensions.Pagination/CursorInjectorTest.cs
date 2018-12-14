using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.Extensions.Test;
using Xunit;
using Xunit2.Should;

namespace GraphQL.Extensions.Pagination {
    public class CursorInjectorTest {
    
    private static CursorInjector<MockEntityForCursorVisitorTest, MockEntityForCursorVisitorTest> systemUnderTest;
    private static string cursorSegmentDelimiter = "//";
    private static string cursorSubsegmentDelimiter = "::";
    private ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntityForCursorVisitorTest), "f");

    [Theory]
    [MemberData(nameof(GetTestData_Single))]
    public void Should_InjectCorrectCursor_When_InjectIntoSelectorCalled(OrderByInfo<MockEntityForCursorVisitorTest> orderBy,
        IQueryable<MockEntityForCursorVisitorTest> testData, IQueryable<MockEntityForCursorVisitorTest> expectedResults) {
        
        Exception exception = Record.Exception(() =>
            systemUnderTest = new CursorInjector<MockEntityForCursorVisitorTest, MockEntityForCursorVisitorTest>(orderBy,
                cursorSegmentDelimiter, cursorSubsegmentDelimiter));
        exception.ShouldBeNull();
        systemUnderTest.ShouldNotBeNull();

        Expression<Func<MockEntityForCursorVisitorTest, MockEntityForCursorVisitorTest>> newSelector = null;
        exception = Record.Exception(() => newSelector = systemUnderTest.InjectIntoSelector(o => new MockEntityForCursorVisitorTest {
            Char = o.Char,
            CharNull = o.CharNull,
            Short = o.Short,
            ShortNull = o.ShortNull,
            Int = o.Int,
            IntNull = o.IntNull,
            Long = o.Long,
            LongNull = o.LongNull,
            Decimal = o.Decimal,
            DecimalNull = o.DecimalNull,
            Float = o.Float,
            FloatNull = o.FloatNull,
            Double = o.Double,
            DoubleNull = o.DoubleNull,
            DateTime = o.DateTime,
            DateTimeNull = o.DateTimeNull,
            String = o.String,
        }));
        exception.ShouldBeNull();
        newSelector.ShouldNotBeNull();

        IQueryable<MockEntityForCursorVisitorTest> results = null;
        exception = Record.Exception(() =>
            results = testData.Select<MockEntityForCursorVisitorTest, MockEntityForCursorVisitorTest>(newSelector));
        exception.ShouldBeNull();
        results.ShouldNotBeNull();

        results.SequenceEqual(expectedResults).ShouldBeTrue();
    }
    
    public static List<object[]> GetTestData_Single
        => new List<object[]> {
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Char", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "char::a::" + o.Char).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Char", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "char::d::" + o.Char).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("CharNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "charnull::a::" + (o.CharNull.HasValue ? o.CharNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("CharNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "charnull::d::" + (o.CharNull.HasValue ? o.CharNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Short", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "short::a::" + o.Short.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Short", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "short::d::" + o.Short.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "shortnull::a::" + (o.ShortNull.HasValue ? o.ShortNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "shortnull::d::" + (o.ShortNull.HasValue ? o.ShortNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "int::a::" + o.Int.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "int::d::" + o.Int.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("IntNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "intnull::a::" + (o.IntNull.HasValue ? o.IntNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("IntNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "intnull::d::" + (o.IntNull.HasValue ? o.IntNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Long", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "long::a::" + o.Long.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Long", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "long::d::" + o.Long.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("LongNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "longnull::a::" + (o.LongNull.HasValue ? o.LongNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("LongNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "longnull::d::" + (o.LongNull.HasValue ? o.LongNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Decimal", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "decimal::a::" + o.Decimal.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Decimal", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "decimal::d::" + o.Decimal.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DecimalNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "decimalnull::a::" + (o.DecimalNull.HasValue ? o.DecimalNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DecimalNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "decimalnull::d::" + (o.DecimalNull.HasValue ? o.DecimalNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Float", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "float::a::" + o.Float.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Float", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "float::d::" + o.Float.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("FloatNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "floatnull::a::" + (o.FloatNull.HasValue ? o.FloatNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("FloatNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "floatnull::d::" + (o.FloatNull.HasValue ? o.FloatNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Double", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "double::a::" + o.Double.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("Double", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "double::d::" + o.Double.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DoubleNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "doublenull::a::" + (o.DoubleNull.HasValue ? o.DoubleNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DoubleNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "doublenull::d::" + (o.DoubleNull.HasValue ? o.DoubleNull.Value.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "datetime::a::" + o.DateTime.Ticks.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "datetime::d::" + o.DateTime.Ticks.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DateTimeNull", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "doublenull::a::" + (o.DateTimeNull.HasValue ? o.DateTimeNull.Value.Ticks.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("DateTimeNull", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "doublenull::d::" + (o.DateTimeNull.HasValue ? o.DateTimeNull.Value.Ticks.ToString() : " ")).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Ascending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "string::a::" + o.String.ToString()).AsQueryable(),
            },
            new object[] {
                new OrderByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Descending),
                testData.AsQueryable(),
                testData.Select(o => o.Cursor = "string::d::" + o.String.ToString()).AsQueryable(),
            },
        };

    private static List<MockEntityForCursorVisitorTest> testData
        => new List<MockEntityForCursorVisitorTest> {
            new MockEntityForCursorVisitorTest {
                Char = 'A',
                CharNull = null,
                Short = 5,
                ShortNull = null,
                Int = 10,
                IntNull = null,
                Long = 15,
                LongNull = null,
                Decimal = 20.01M,
                DecimalNull = null,
                Float = 25.01F,
                FloatNull = null,
                Double = 30.01,
                DoubleNull = null,
                DateTime = DateTime.Today,
                DateTimeNull = null,
                String = "xyz",
            },
            new MockEntityForCursorVisitorTest {
                Char = 'B',
                CharNull = 'C',
                Short = 50,
                ShortNull = 55,
                Int = 100,
                IntNull = 105,
                Long = 150,
                LongNull = 155,
                Decimal = 200.01M,
                DecimalNull = 205.01M,
                Float = 250.01F,
                FloatNull = 255.01F,
                Double = 300.01,
                DoubleNull = 305.01,
                DateTime = DateTime.Today.AddDays(10).AddHours(15).AddMinutes(20).AddSeconds(25),
                DateTimeNull = DateTime.Today.AddDays(-10).AddHours(-15).AddMinutes(-20).AddSeconds(-25),
                String = "abc",
            },
            new MockEntityForCursorVisitorTest {
                Char = 'D',
                CharNull = 'D',
                Short = -5,
                ShortNull = -5,
                Int = -10,
                IntNull = -10,
                Long = -15,
                LongNull = -15,
                Decimal = -20.01M,
                DecimalNull = -20.01M,
                Float = -25.01F,
                FloatNull = -25.01F,
                Double = 30.01,
                DoubleNull = 30.01,
                DateTime = DateTime.Today.AddYears(1),
                DateTimeNull = DateTime.Today.AddYears(1),
                String = "abcde",
            },
        };

        private static List<MockEntityForCursorVisitorTest> testDataVariousStrings
            => new List<MockEntityForCursorVisitorTest> {
                new MockEntityForCursorVisitorTest {
                    String = null,
                },
                new MockEntityForCursorVisitorTest {
                    String = "",
                },
                new MockEntityForCursorVisitorTest {
                    String = " ",
                },
                new MockEntityForCursorVisitorTest {
                    String = "  ",
                },
                new MockEntityForCursorVisitorTest {
                    String = "   ",
                },
                new MockEntityForCursorVisitorTest {
                    String = "abcdefg",
                },
            };
    }
}