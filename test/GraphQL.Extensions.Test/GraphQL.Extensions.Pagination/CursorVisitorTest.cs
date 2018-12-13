using Linq.Expressions.Compare;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GraphQL.Extensions.Test;
using Xunit;
using Xunit2.Should;
using GraphQL.Extensions.Internal;
using System.Text;

namespace GraphQL.Extensions.Pagination {
    public class CursorVisitorTest {
        
        private static ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntityForCursorVisitorTest), "f");
        private static string cursorSegmentDelimiter = "//";
        private static string cursorSubsegmentDelimiter = "::";

        private static MockEntityForCursorVisitorTest testObject1 = new MockEntityForCursorVisitorTest {
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
            DateTime = DateTime.Now,
            DateTimeNull = null,
            String = null,
        };

        private static MockEntityForCursorVisitorTest testObject2 = new MockEntityForCursorVisitorTest {
            Char = 'A',
            CharNull = 'A',
            Short = 5,
            ShortNull = 5,
            Int = 10,
            IntNull = 10,
            Long = 15,
            LongNull = 15,
            Decimal = 20.01M,
            DecimalNull = 20.01M,
            Float = 25.01F,
            FloatNull = 25.01F,
            Double = 30.01,
            DoubleNull = 30.01,
            DateTime = DateTime.Now,
            DateTimeNull = DateTime.Now,
            String = "abc",
        };

        private static TestCursorVisitor systemUnderTest = new TestCursorVisitor(parameterExpression, cursorSegmentDelimiter, cursorSubsegmentDelimiter);

        [Theory]
        [MemberData(nameof(GetPrimitiveCursorPartTestData))]
        public void Should_GenerateToStringMethodCallExpression_When_PrimitiveCursorPartCalled(Type type, string memberName,
            MockEntityForCursorVisitorTest testData, string expectedResult) {
            
            Expression toStringMethodCallExpression = null;
            Exception exception = Record.Exception(() =>
                toStringMethodCallExpression = systemUnderTest.PrimitiveCursorPart(
                    type,
                    Expression.MakeMemberAccess(
                        parameterExpression,
                        typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0])
                ));
            exception.ShouldBeNull();
            toStringMethodCallExpression.ShouldNotBeNull();
            
            string result = null;
            exception = Record.Exception(() =>
                result = (string)Expression.Lambda(toStringMethodCallExpression, parameterExpression).Compile().DynamicInvoke(testData));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetDateTimeCursorPartTestData))]
        public void Should_GenerateTicksProperty_When_DateTimeCursorPartCalled(Type type, string memberName,
            MockEntityForCursorVisitorTest testData, string expectedResult) {

            expectedResult = testData.DateTime.Ticks.ToString();

            Expression ticksToStringMethodCallExpression = null;
            Exception exception = Record.Exception(() =>
                ticksToStringMethodCallExpression = systemUnderTest.DateTimeCursorPart(
                    Expression.MakeMemberAccess(
                        parameterExpression,
                        typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0]
                    )
                ));
            exception.ShouldBeNull();
            ticksToStringMethodCallExpression.ShouldNotBeNull();

            string result = null;
            exception = Record.Exception(() =>
                result = (string)Expression.Lambda(ticksToStringMethodCallExpression, parameterExpression).Compile().DynamicInvoke(testData));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [InlineData(typeof(char), "Char")]
        [InlineData(typeof(short), "Short")]
        [InlineData(typeof(int), "Int")]
        [InlineData(typeof(long), "Long")]
        [InlineData(typeof(decimal), "Decimal")]
        [InlineData(typeof(float), "Float")]
        [InlineData(typeof(double), "Double")]
        [InlineData(typeof(DateTime), "DateTime")]
        public void Should_ThrowException_When_NonNullablePassedToNullableCursorPart(Type type, string memberName) {

            Assert.Throws<ArgumentNullException>(() => systemUnderTest.NullableCursorPart(
                type,
                Expression.MakeMemberAccess(
                    parameterExpression,
                    typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0]
                )
            ));
        }

        [Theory]
        [MemberData(nameof(GetNullableCursorPartTestData))]
        public void Should_GenerateCorrectOutput_When_NullableCursorPartCalled(Type type, string memberName,
            MockEntityForCursorVisitorTest testData, string expectedResult) {
            
            Expression toStringMethodCallExpression = null;
            Exception exception = Record.Exception(() =>
                toStringMethodCallExpression = systemUnderTest.NullableCursorPart(
                    type,
                    Expression.MakeMemberAccess(
                        parameterExpression,
                        typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0]
                    )
                ));
            exception.ShouldBeNull();
            toStringMethodCallExpression.ShouldNotBeNull();

            string result = null;
            exception = Record.Exception(() =>
                result = (string)Expression.Lambda(toStringMethodCallExpression, parameterExpression).Compile().DynamicInvoke(testData));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetNullableCursorPartTestData_DateTime))]
        public void Should_GenerateCorrectOutput_When_NullableCursorPartCalled_DateTime(Type type, string memberName,
            MockEntityForCursorVisitorTest testData, string expectedResult) {

            expectedResult = testData.DateTimeNull.HasValue
                ? testData.DateTimeNull.Value.Ticks.ToString()
                : "\0";

            Expression toStringMethodCallExpression = null;
            Exception exception = Record.Exception(() =>
                toStringMethodCallExpression = systemUnderTest.NullableCursorPart(
                    type,
                    Expression.MakeMemberAccess(
                        parameterExpression,
                        typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0]
                    )
                ));
            exception.ShouldBeNull();
            toStringMethodCallExpression.ShouldNotBeNull();

            string result = null;
            exception = Record.Exception(() =>
                result = (string)Expression.Lambda(toStringMethodCallExpression, parameterExpression).Compile().DynamicInvoke(testData));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetDateTimeCursorPartTestData))]
        [MemberData(nameof(GetNullableCursorPartTestData))]
        [MemberData(nameof(GetNullableCursorPartTestData_DateTime))]
        [MemberData(nameof(GetPrimitiveCursorPartTestData))]
        [MemberData(nameof(GetStringCursorPartTestData))]
        public void Should_GenerateCorrectOutput_When_GetCursorPartCalled(Type type, string memberName,
            MockEntityForCursorVisitorTest testData, string expectedResult) {
            
            if (type == typeof(DateTime))
                expectedResult = testData.DateTime.Ticks.ToString();
            else if (type == typeof(DateTime?))
                expectedResult = testData.DateTimeNull.HasValue
                    ? testData.DateTimeNull.Value.Ticks.ToString()
                    : "\0";

            Expression cursorMethodCallExpression = null;
            Exception exception = Record.Exception(() =>
                cursorMethodCallExpression = systemUnderTest.GetCursorPart(
                    type,
                    Expression.MakeMemberAccess(
                        parameterExpression,
                        typeof(MockEntityForCursorVisitorTest).GetMember(memberName)[0]
                    )
                ));
            exception.ShouldBeNull();
            cursorMethodCallExpression.ShouldNotBeNull();

            string result = null;
            exception = Record.Exception(() =>
                result = (string)Expression.Lambda(cursorMethodCallExpression, parameterExpression).Compile().DynamicInvoke(testData));
            exception.ShouldBeNull();
            if (type != typeof(string))
                result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Theory]
        [MemberData(nameof(GetVisitorTestData_Single))]
        [MemberData(nameof(GetVisitorTestData_Double))]
        [MemberData(nameof(GetVisitorTestData_Triple))]
        public void Should_GenerateCorrectCursor_When_VisitCalled(OrderByInfo<MockEntityForCursorVisitorTest> orderBy,
            MockEntityForCursorVisitorTest testData, Cursor expectedResult) {
            
            // This step is 100% dependent on the above tests passing. Any failures above mean that this step will yield incorrect results.
            // There's not a good way to generate the list of expected expressions without relying on the methods w/i the visitor class
            // unless we duplicate them in their entirity here. So this test assumes that they are working as intended, and then relies on them.
            // Therefore, this test predominately is to ensure that the list itself is constructed correctly for a given orderBy clause.
            expectedResult.CursorExpressions = new List<Expression>();
            OrderByInfoBase<MockEntityForCursorVisitorTest> orderBySegment = orderBy;
            do {
                expectedResult.CursorExpressions.Add(systemUnderTest.GetCursorPart(
                    orderBySegment.GetMemberType(),
                    orderBySegment.GetMemberExpression(parameterExpression)
                ));
                orderBySegment = orderBySegment.ThenBy;
            } while (orderBySegment != null);
            
            Cursor result = null;
            Exception exception = Record.Exception(() => result = systemUnderTest.Visit(orderBy));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.CursorFormatString.ToString().ShouldBe(expectedResult.CursorFormatString.ToString());
            
            ExpressionTreeComparer comparer = new ExpressionTreeComparer();
            int i = 0;
            foreach (var item in result.CursorExpressions)
            {
                comparer.Equals(item, expectedResult.CursorExpressions[i]).ShouldBeTrue();
                i++;
            }
        }

        public static List<object[]> GetPrimitiveCursorPartTestData
            => new List<object[]> {
                new object[] {
                    typeof(char),
                    "Char",
                    new MockEntityForCursorVisitorTest {
                        Char = 'A',
                    },
                    "A",
                },
                new object[] {
                    typeof(short),
                    "Short",
                    new MockEntityForCursorVisitorTest {
                        Short = 5,
                    },
                    "5",
                },
                new object[] {
                    typeof(int),
                    "Int",
                    new MockEntityForCursorVisitorTest {
                        Int = 10,
                    },
                    "10",
                },
                new object[] {
                    typeof(long),
                    "Long",
                    new MockEntityForCursorVisitorTest {
                        Long = 15,
                    },
                    "15",
                },
                new object[] {
                    typeof(decimal),
                    "Decimal",
                    new MockEntityForCursorVisitorTest {
                        Decimal = 20.01M,
                    },
                    "20.01",
                },
                new object[] {
                    typeof(float),
                    "Float",
                    new MockEntityForCursorVisitorTest {
                        Float = 25.01F,
                    },
                    "25.01",
                },
                new object[] {
                    typeof(double),
                    "Double",
                    new MockEntityForCursorVisitorTest {
                        Double = 30.01,
                    },
                    "30.01",
                },
            };

        public static List<object[]> GetNullableCursorPartTestData
            => new List<object[]> {
                new object[] {
                    typeof(char?),
                    "CharNull",
                    new MockEntityForCursorVisitorTest {
                        CharNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(char?),
                    "CharNull",
                    new MockEntityForCursorVisitorTest {
                        CharNull = 'A',
                    },
                    "A",
                },
                new object[] {
                    typeof(short?),
                    "ShortNull",
                    new MockEntityForCursorVisitorTest {
                        ShortNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(short?),
                    "ShortNull",
                    new MockEntityForCursorVisitorTest {
                        ShortNull = 5,
                    },
                    "5",
                },
                new object[] {
                    typeof(int?),
                    "IntNull",
                    new MockEntityForCursorVisitorTest {
                        IntNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(int?),
                    "IntNull",
                    new MockEntityForCursorVisitorTest {
                        IntNull = 10,
                    },
                    "10",
                },
                new object[] {
                    typeof(long?),
                    "LongNull",
                    new MockEntityForCursorVisitorTest {
                        LongNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(long?),
                    "LongNull",
                    new MockEntityForCursorVisitorTest {
                        LongNull = 15,
                    },
                    "15",
                },
                new object[] {
                    typeof(decimal?),
                    "DecimalNull",
                    new MockEntityForCursorVisitorTest {
                        DecimalNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(decimal?),
                    "DecimalNull",
                    new MockEntityForCursorVisitorTest {
                        DecimalNull = 20.01M,
                    },
                    "20.01",
                },
                new object[] {
                    typeof(float?),
                    "FloatNull",
                    new MockEntityForCursorVisitorTest {
                        FloatNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(float?),
                    "FloatNull",
                    new MockEntityForCursorVisitorTest {
                        FloatNull = 25.01F,
                    },
                    "25.01",
                },
                new object[] {
                    typeof(double?),
                    "DoubleNull",
                    new MockEntityForCursorVisitorTest {
                        DoubleNull = null,
                    },
                    "\0",
                },
                new object[] {
                    typeof(double?),
                    "DoubleNull",
                    new MockEntityForCursorVisitorTest {
                        DoubleNull = 30.01,
                    },
                    "30.01",
                },
            };

        public static List<object[]> GetNullableCursorPartTestData_DateTime
            => new List<object[]> {
                new object[] {
                    typeof(DateTime?),
                    "DateTimeNull",
                    new MockEntityForCursorVisitorTest {
                        DateTimeNull = null,
                    },
                    null,
                },
                new object[] {
                    typeof(DateTime?),
                    "DateTimeNull",
                    new MockEntityForCursorVisitorTest {
                        DateTimeNull = DateTime.Now,
                    },
                    null,
                },
            };

        public static List<object[]> GetDateTimeCursorPartTestData
            => new List<object[]> {
                new object[] {
                    typeof(DateTime),
                    "DateTime",
                    new MockEntityForCursorVisitorTest {
                        DateTime = DateTime.Now,
                    },
                    null,
                },
            };

        public static List<object[]> GetStringCursorPartTestData
            => new List<object[]> {
                new object[] {
                    typeof(string),
                    "String",
                    new MockEntityForCursorVisitorTest {
                        String = null,
                    },
                    null,
                },
                new object[] {
                    typeof(string),
                    "String",
                    new MockEntityForCursorVisitorTest {
                        String = "",
                    },
                    "",
                },
                new object[] {
                    typeof(string),
                    "String",
                    new MockEntityForCursorVisitorTest {
                        String = " ",
                    },
                    " ",
                },
                new object[] {
                    typeof(string),
                    "String",
                    new MockEntityForCursorVisitorTest {
                        String = "  ",
                    },
                    "  ",
                },
                new object[] {
                    typeof(string),
                    "String",
                    new MockEntityForCursorVisitorTest {
                        String = "abc",
                    },
                    "abc",
                },
            };

        public static List<object[]> GetVisitorTestData_Single
            => new List<object[]> {
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Char", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("char::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Char", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("char::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("CharNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("charnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("CharNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("charnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Short", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("short::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Short", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("short::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("int::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("int::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("IntNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("intnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("IntNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("intnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Long", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("long::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Long", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("long::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("LongNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("longnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("LongNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("longnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Decimal", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("decimal::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Decimal", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("decimal::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DecimalNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("decimalnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DecimalNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("decimalnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Float", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("float::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Float", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("float::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("FloatNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("floatnull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("FloatNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("floatnull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Double", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("double::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("Double", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("double::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DoubleNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("doublenull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DoubleNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("doublenull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("datetime::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("datetime::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DateTimeNull", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("datetimenull::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("DateTimeNull", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("datetimenull::d::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Ascending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("string::a::{0}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Descending),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("string::d::{0}") },
                },
            };

            public static List<object[]> GetVisitorTestData_Double
                => new List<object[]> {
                    new object[] {
                        new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending,
                            new ThenByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Ascending)),
                        testObject1,
                        new Cursor { CursorFormatString = new StringBuilder("int::a::{0}//string::a::{1}") },
                    },
                    new object[] {
                        new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Descending,
                            new ThenByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Ascending)),
                        testObject1,
                        new Cursor { CursorFormatString = new StringBuilder("int::d::{0}//string::a::{1}") },
                    },
                    new object[] {
                        new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending,
                            new ThenByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Descending)),
                        testObject1,
                        new Cursor { CursorFormatString = new StringBuilder("int::a::{0}//string::d::{1}") },
                    },
                    new object[] {
                        new OrderByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Descending,
                            new ThenByInfo<MockEntityForCursorVisitorTest>("String", SortDirections.Descending)),
                        testObject1,
                        new Cursor { CursorFormatString = new StringBuilder("int::d::{0}//string::d::{1}") },
                    },
                    new object[] {
                        new OrderByInfo<MockEntityForCursorVisitorTest>("Char", SortDirections.Ascending,
                            new ThenByInfo<MockEntityForCursorVisitorTest>("DateTimeNull", SortDirections.Descending)),
                        testObject1,
                        new Cursor { CursorFormatString = new StringBuilder("char::a::{0}//datetimenull::d::{1}") },
                    },
                };

        public static List<object[]> GetVisitorTestData_Triple
            => new List<object[]> {
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Ascending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Ascending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending))),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::a::{0}//datetime::a::{1}//int::a::{2}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Descending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Ascending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending))),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::d::{0}//datetime::a::{1}//int::a::{2}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Ascending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Descending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Ascending))),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::a::{0}//datetime::d::{1}//int::a::{2}") },
                },
                new object[] {
                    new OrderByInfo<MockEntityForCursorVisitorTest>("ShortNull", SortDirections.Descending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("DateTime", SortDirections.Ascending,
                        new ThenByInfo<MockEntityForCursorVisitorTest>("Int", SortDirections.Descending))),
                    testObject1,
                    new Cursor { CursorFormatString = new StringBuilder("shortnull::d::{0}//datetime::a::{1}//int::d::{2}") },
                },
            };

        private class TestCursorVisitor : CursorVisitor<MockEntityForCursorVisitorTest, MockEntityForCursorVisitorTest> {

            public TestCursorVisitor(ParameterExpression parameterExpression, string cursorSegmentDelimiter, string cursorSubsegmentDelimiter)
                : base(parameterExpression, cursorSegmentDelimiter, cursorSubsegmentDelimiter) { }

            public new Expression NullableCursorPart(Type type, MemberExpression memberExpression)
                => base.NullableCursorPart(type, memberExpression);

            public new Expression PrimitiveCursorPart(Type type, MemberExpression memberExpression)
                => base.PrimitiveCursorPart(type, memberExpression);
            
            public new Expression DateTimeCursorPart(MemberExpression memberExpression)
                => base.DateTimeCursorPart(memberExpression);

            public new Expression GetCursorPart(Type type, MemberExpression memberExpression)
                => base.GetCursorPart(type, memberExpression);
        }
    }
}