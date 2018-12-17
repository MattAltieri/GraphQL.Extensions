using GraphQL.Extensions.Test;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit2.Should;

namespace GraphQL.Extensions.Internal {
    public class CachedReflectionTest {

        private static IQueryable<MockEntity> baseTestData = (new List<MockEntity> {
            new MockEntity {
                Id = 2,
                Name = "Aimee",
                DOB = DateTime.Parse("1985-07-18"),
            },
            new MockEntity {
                Id = 1,
                Name = "Matt",
                DOB = DateTime.Parse("1981-04-07")
            },
        }).AsQueryable();
        private static Type TSource = typeof(MockEntity);

        private MethodInfo systemUnderTest_Method;
        private PropertyInfo systemUnderTest_Property;

        [Theory]
        [MemberData(nameof(GetOrderByTestData))]
        public void Should_GenerateOrderByMethod_When_OrderByCalled(IQueryable<MockEntity> testData, Type TKey, Expression expression,
            IOrderedQueryable<MockEntity> expectedResults) {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.OrderBy(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = ((IQueryable<MockEntity>)systemUnderTest_Method.Invoke(null, new object[] { testData, expression })));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetThenByTestData))]
        public void Should_GenerateThenByMethod_When_ThenByCalled(IOrderedQueryable<MockEntity> testData, Type TKey, Expression expression,
            IOrderedQueryable<MockEntity> expectedResults) {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ThenBy(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = (IQueryable<MockEntity>)systemUnderTest_Method.Invoke(null, new object[] { testData, expression }));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetOrderByDescendingTestData))]
        public void Should_GenerateOrderByDescendingMethod_When_OrderByDescendingCalled(IQueryable<MockEntity> testData, Type TKey,
            Expression expression, IOrderedQueryable<MockEntity> expectedResults) {
            
            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.OrderByDescending(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = ((IQueryable<MockEntity>)systemUnderTest_Method.Invoke(null, new object[] { testData, expression })));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Theory]
        [MemberData(nameof(GetThenByDescendingTestData))]
        public void Should_GenerateThenByDescendingMethod_When_ThenByDescendingCalled(IOrderedQueryable<MockEntity> testData, Type TKey,
            Expression expression, IOrderedQueryable<MockEntity> expectedResults) {
            
            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ThenByDescending(TSource, TKey));
            exception.ShouldBeNull();

            IQueryable<MockEntity> result = null;
            exception = Record.Exception(() => result = (IQueryable<MockEntity>)systemUnderTest_Method.Invoke(null, new object[] { testData, expression }));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            Enumerable.SequenceEqual(result, expectedResults).ShouldBeTrue();
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithInt() {

            ParameterExpression param = Expression.Parameter(TSource);
            
            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.Lambda(TSource, typeof(int)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("Id"));

            Expression<Func<MockEntity, int>> intExpression = null;
            exception = Record.Exception(() =>
                intExpression = (Expression<Func<MockEntity, int>>)systemUnderTest_Method.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }));
                        
            exception.ShouldBeNull();
            intExpression.ShouldNotBeNull();
            
            // expected result
            intExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            intExpression.Type.ShouldBe(typeof(Func<MockEntity, int>));
            intExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)intExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(int));
            body.Expression.NodeType.ShouldBe(ExpressionType.Parameter);
            body.Member.Name.ShouldBe("Id");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithString() {

            ParameterExpression param = Expression.Parameter(TSource);

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.Lambda(TSource, typeof(string)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("Name"));

            Expression<Func<MockEntity, string>> stringExpression = null;
            exception = Record.Exception(() =>
                stringExpression = (Expression<Func<MockEntity, string>>)systemUnderTest_Method.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }
                ));
            exception.ShouldBeNull();
            stringExpression.ShouldNotBeNull();

            // expected result
            stringExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            stringExpression.Type.ShouldBe(typeof(Func<MockEntity, string>));
            stringExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)stringExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(string));
            body.Member.Name.ShouldBe("Name");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(string));
        }

        [Fact]
        public void Should_GenerateLambdaExpression_When_LambdaCalledWithDateTime() {

            ParameterExpression param = Expression.Parameter(TSource);

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.Lambda(TSource, typeof(DateTime)));
            exception.ShouldBeNull();

            MemberExpression property = Expression.MakeMemberAccess(param, TSource.GetProperty("DOB"));

            Expression<Func<MockEntity, DateTime>> stringExpression = null;
            exception = Record.Exception(() =>
                stringExpression = (Expression<Func<MockEntity, DateTime>>)systemUnderTest_Method.Invoke(
                    null,
                    new object[] { property, new ParameterExpression[] { param } }
                ));
            exception.ShouldBeNull();
            stringExpression.ShouldNotBeNull();

            // expected result
            stringExpression.NodeType.ShouldBe(ExpressionType.Lambda);
            stringExpression.Type.ShouldBe(typeof(Func<MockEntity, DateTime>));
            stringExpression.Parameters[0].Type.ShouldBe(TSource);

            MemberExpression body = (MemberExpression)stringExpression.Body;
            body.NodeType.ShouldBe(ExpressionType.MemberAccess);
            body.Type.ShouldBe(typeof(DateTime));
            body.Member.Name.ShouldBe("DOB");
            body.Member.MemberType.ShouldBe(MemberTypes.Property);
            ((PropertyInfo)body.Member).PropertyType.ShouldBe(typeof(DateTime));
        }

        [Fact]
        public void Should_GenerateStringFormatMethod_When_StringFormatCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringFormat());
            exception.ShouldBeNull();

            string format = "{0} is a {1}.";
            object[] parameters = new object[] { 15.ToString(), "number" };
            string expectedResult = "15 is a number.";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(null, new object[] { format, parameters }));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_char() {
            
            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(char)));
            exception.ShouldBeNull();

            char testValue = 'a';
            string expectedResult = "a";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_short() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(short)));
            exception.ShouldBeNull();

            short testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_int() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(int)));
            exception.ShouldBeNull();

            int testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_long() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(long)));
            exception.ShouldBeNull();

            long testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_decimal() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(decimal)));
            exception.ShouldBeNull();

            decimal testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_float() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(float)));
            exception.ShouldBeNull();

            float testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_double() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(double)));
            exception.ShouldBeNull();

            double testValue = 15;
            string expectedResult = "15";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateToStringMethod_When_ToStringCalled_bool() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.ToString(typeof(bool)));
            exception.ShouldBeNull();

            bool testValue = true;
            string expectedResult = "True";

            string result = null;
            exception = Record.Exception(() => result = (string)systemUnderTest_Method.Invoke(testValue, null));
            exception.ShouldBeNull();
            result.ShouldNotBeNull();

            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_char() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(char?)));
            exception.ShouldBeNull();

            char? testValue = 'a';
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_short() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(short?)));
            exception.ShouldBeNull();

            short? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_int() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(int?)));
            exception.ShouldBeNull();

            int? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_long() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(long?)));
            exception.ShouldBeNull();

            long? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_decimal() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(decimal?)));
            exception.ShouldBeNull();

            decimal? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_float() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(float?)));
            exception.ShouldBeNull();

            float? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_double() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(double?)));
            exception.ShouldBeNull();

            double? testValue = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_bool() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(bool?)));
            exception.ShouldBeNull();

            bool? testValue = true;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue, null));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateHasValueProperty_When_NullableHasValueCalled_DateTime() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableHasValue(typeof(DateTime?)));
            exception.ShouldBeNull();

            DateTime? testValue = DateTime.Today;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_char() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(char?)));
            exception.ShouldBeNull();

            char? testValue = 'a';
            char expectedResult = 'a';

            char? result = null;
            exception = Record.Exception(() => result = (char?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_short() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(short?)));
            exception.ShouldBeNull();

            short? testValue = 15;
            short expectedResult = 15;

            short? result = null;
            exception = Record.Exception(() => result = (short?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_int() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(int?)));
            exception.ShouldBeNull();

            int? testValue = 15;
            int expectedResult = 15;

            int? result = null;
            exception = Record.Exception(() => result = (int?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_long() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(long?)));
            exception.ShouldBeNull();

            long? testValue = 15;
            long expectedResult = 15;

            long? result = null;
            exception = Record.Exception(() => result = (long?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_decimal() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(decimal?)));
            exception.ShouldBeNull();

            decimal? testValue = 15;
            decimal expectedResult = 15;

            decimal? result = null;
            exception = Record.Exception(() => result = (decimal?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_float() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(float?)));
            exception.ShouldBeNull();

            float? testValue = 15;
            float expectedResult = 15;

            float? result = null;
            exception = Record.Exception(() => result = (float?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_double() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(double?)));
            exception.ShouldBeNull();

            double? testValue = 15;
            double expectedResult = 15;

            double? result = null;
            exception = Record.Exception(() => result = (double?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_bool() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(bool?)));
            exception.ShouldBeNull();

            bool? testValue = true;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateValueProperty_When_NullableValueCalled_DateTime() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.NullableValue(typeof(DateTime?)));
            exception.ShouldBeNull();

            DateTime? testValue = DateTime.Today;
            DateTime expectedResult = testValue.Value;

            DateTime? result = null;
            exception = Record.Exception(() => result = (DateTime?)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateDateTimeTicksProperty_When_DateTimeTicksCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Property = CachedReflection.DateTimeTicks());
            exception.ShouldBeNull();

            DateTime testValue = DateTime.Today;
            long expectedResult = testValue.Ticks;

            long? result = null;
            exception = Record.Exception(() => result = (long)systemUnderTest_Property.GetValue(testValue));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateStringCompareToMethod_When_StringCompareToCalled() {

            string first = "AAAAA", second = "BBBBB", third = "CCCCC";

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringCompareTo());
            exception.ShouldBeNull();
            
            int? result = null;
            exception = Record.Exception(() => result = (int)systemUnderTest_Method.Invoke(second, new object[] { first }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBeGreaterThan(0);

            result = null;
            exception = Record.Exception(() => result = (int)systemUnderTest_Method.Invoke(second, new object[] { third }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBeLessThan(0);
        }
        
        [Fact]
        public void Should_GenerateCharCompareToMethod_When_StringCompareToCalled() {

            char first = 'A', second = 'B', third = 'C';

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.CharCompareTo());
            exception.ShouldBeNull();

            int? result = null;
            exception = Record.Exception(() => result = (int)systemUnderTest_Method.Invoke(second, new object[] { first }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBeGreaterThan(0);

            result = null;
            exception = Record.Exception(() => result = (int)systemUnderTest_Method.Invoke(second, new object[] { third }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBeLessThan(0);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithChar() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(char)));
            exception.ShouldBeNull();

            IEnumerable<char> testData = new[] { 'a', 'b', 'c', 'A', 'B', 'C' };
            char testItem = 'b';
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 'd';
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableChar() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(char?)));
            exception.ShouldBeNull();

            IEnumerable<char?> testData = new[] { 'a', 'b', 'c', 'A', 'B', 'C', (char?)null };
            char? testItem = 'b';
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 'd';
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            char testItemNonNull = 'b';
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithShort() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(short)));
            exception.ShouldBeNull();

            IEnumerable<short> testData = new short[] { 5, 10, 15, 20, 25, 30 };
            short testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableShort() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(short?)));
            exception.ShouldBeNull();

            IEnumerable<short?> testData = new short?[] { 5, 10, 15, 20, 25, 30, null };
            short? testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            short testItemNonNull = 15;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithInt() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(int)));
            exception.ShouldBeNull();

            IEnumerable<int> testData = new[] { 5, 10, 15, 20, 25, 30 };
            int testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableInt() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(int?)));
            exception.ShouldBeNull();

            IEnumerable<int?> testData = new[] { 5, 10, 15, 20, 25, 30, (int?)null };
            int? testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
            
            int testItemNonNull = 15;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithLong() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(long)));
            exception.ShouldBeNull();

            IEnumerable<long> testData = new long[] { 5, 10, 15, 20, 25, 30 };
            long testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableLong() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(long?)));
            exception.ShouldBeNull();

            IEnumerable<long?> testData = new long?[] { 5, 10, 15, 20, 25, 30, null };
            long? testItem = 15;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
            
            long testItemNonNull = 15;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithDecimal() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(decimal)));
            exception.ShouldBeNull();

            IEnumerable<decimal> testData = new[] { 5M, 10M, 15M, 20M, 25M, 30M };
            decimal testItem = 15M;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16M;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableDecimal() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(decimal?)));
            exception.ShouldBeNull();

            IEnumerable<decimal?> testData = new decimal?[] { 5M, 10M, 15M, 20M, 25M, 30M, null };
            decimal? testItem = 15M;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16M;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
            
            decimal testItemNonNull = 15M;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithFloat() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(float)));
            exception.ShouldBeNull();

            IEnumerable<float> testData = new[] { 5F, 10F, 15F, 20F, 25F, 30F };
            float testItem = 15F;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16F;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableFloat() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(float?)));
            exception.ShouldBeNull();

            IEnumerable<float?> testData = new float?[] { 5F, 10F, 15F, 20F, 25F, 30F };
            float? testItem = 15F;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16F;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
            
            float testItemNonNull = 15F;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithDouble() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(double)));
            exception.ShouldBeNull();

            IEnumerable<double> testData = new[] { 5.0, 10.0, 15.0, 20.0, 25.0, 30.0 };
            double testItem = 15.0;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16.0;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableDouble() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(double?)));
            exception.ShouldBeNull();

            IEnumerable<double?> testData = new double?[] { 5.0, 10.0, 15.0, 20.0, 25.0, 30.0, null };
            double? testItem = 15.0;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = 16.0;
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
            
            double testItemNonNull = 15.0;
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithString() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(string)));
            exception.ShouldBeNull();

            IEnumerable<string> testData = new[] { "aa", "bb", "cc", "aAa", "bBb", "cCc" };
            string testItem = "bb";
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = "bbbb";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();

            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithDateTime() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(DateTime)));
            exception.ShouldBeNull();

            IEnumerable<DateTime> testData = new[] { DateTime.Parse("2015-12-17"), DateTime.Parse("2016-12-17"), DateTime.Parse("2017-12-17") };
            DateTime testItem = DateTime.Parse("2016-12-17");
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = DateTime.Parse("2016-01-24");
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();

            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateIEnumerableContainsMethod_When_IEnumerableContainsCalledWithNullableDateTime() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.IEnumerableContains(typeof(DateTime?)));
            exception.ShouldBeNull();

            IEnumerable<DateTime?> testData = new DateTime?[] { DateTime.Parse("2015-12-17"), DateTime.Parse("2016-12-17"), DateTime.Parse("2017-12-17"), null };
            DateTime? testItem = DateTime.Parse("2016-12-17");
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testItem = DateTime.Parse("2016-01-24");
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItem }));
            exception.ShouldBeNull();

            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBe(expectedResult);

            DateTime testItemNonNull = DateTime.Parse("2016-12-17");
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData, testItemNonNull }));
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateStringContainsMethod_When_StringContainsCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringContains());
            exception.ShouldBeNull();

            string testData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            string testPattern = "ipsum dol";
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testPattern = "abcdefg";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateStringContainsMethod_When_StringStartsWithCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringStartsWith());
            exception.ShouldBeNull();

            string testData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            string testPattern = "Lorem ipsum dol";
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testPattern = "abcdefg";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateStringContainsMethod_When_StringEndsWithCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringEndsWith());
            exception.ShouldBeNull();

            string testData = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            string testPattern = "ore magna aliqua.";
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testPattern = "abcdefg";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(testData, new object[] { testPattern }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        [Fact]
        public void Should_GenerateStringIsNullOrEmpty_When_StringIsNullOrEmptyCalled() {

            Exception exception = Record.Exception(() => systemUnderTest_Method = CachedReflection.StringIsNullOrEmpty());
            exception.ShouldBeNull();

            string testData = null;
            bool expectedResult = true;

            bool? result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testData = "";
            expectedResult = true;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testData = " ";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);

            testData = "abc";
            expectedResult = false;

            result = null;
            exception = Record.Exception(() => result = (bool)systemUnderTest_Method.Invoke(null, new object[] { testData }));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.Value.ShouldBe(expectedResult);
        }

        public static List<object[]> GetOrderByTestData
            => new List<object[]> {
                new object[] {
                    baseTestData,
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => o.Id)
                },
                new object[] {
                    baseTestData,
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => o.Name)
                },
                new object[] {
                    baseTestData,
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => o.DOB)
                },
            };

        public static List<object[]> GetThenByTestData
            => new List<object[]> {
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.Id),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.Name),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => 1).ThenBy(o => o.DOB),
                }
            };

        public static List<object[]> GetOrderByDescendingTestData
            => new List<object[]> {
                new object[] {
                    baseTestData,
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderByDescending(o => o.Id),
                },
                new object[] {
                    baseTestData,
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderByDescending(o => o.Name),
                },
                new object[] {
                    baseTestData,
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderByDescending(o => o.DOB),
                },
            };

        public static List<object[]> GetThenByDescendingTestData
            => new List<object[]> {
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(int),
                    (Expression<Func<MockEntity, int>>)(o => o.Id),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.Id),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(string),
                    (Expression<Func<MockEntity, string>>)(o => o.Name),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.Name),
                },
                new object[] {
                    baseTestData.OrderBy(o => 1),
                    typeof(DateTime),
                    (Expression<Func<MockEntity, DateTime>>)(o => o.DOB),
                    baseTestData.OrderBy(o => 1).ThenByDescending(o => o.DOB),
                }
            };
    }
}