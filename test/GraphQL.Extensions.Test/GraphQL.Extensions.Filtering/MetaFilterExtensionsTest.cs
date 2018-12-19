using Linq.Expressions.Compare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Test;
using Xunit;
using Xunit2.Should;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Filtering {
    public class MetaFilterExtensionsTest {

        private static ParameterExpression objectParam = Expression.Parameter(typeof(MockEntityForCursorVisitorTest), "f");
        private static ParameterExpression filterParam = Expression.Parameter(typeof(MockMetaFilter), "f1");

        [Theory]
        [MemberData(nameof(TestComparisonData))]
        public void Should_GenerateBinaryExpressionAsLambda_When_CompareCalled(
            MockMetaFilter metaFilter,
            FilterOperators op,
            string memberName,
            MemberInfo filterMemberInfo,
            Expression<Func<MockEntityForCursorVisitorTest, bool>> expectedResult) {
            
            Expression<Func<MockEntityForCursorVisitorTest, bool>> predicate = null;
            Exception exception = Record.Exception(() =>
                predicate = MetaFilterExtensions.Compare<MockEntityForCursorVisitorTest, MockMetaFilter>(metaFilter, op, memberName,
                objectParam, filterMemberInfo));
            exception.ShouldBeNull();
            predicate.ShouldNotBeNull();

            ExpressionTreeComparer comparer = new ExpressionTreeComparer();
            bool? result = null;
            exception = Record.Exception(() => result = comparer.Equals(predicate, expectedResult));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();

            result.ShouldBeTrue();
        }
        
        public static List<object[]> TestComparisonData
            => new List<object[]> {
                // new object[] {
                //     new MockMetaFilter {
                //         Char = 'b'
                //     },
                //     FilterOperators.Equal,
                //     "Char",
                //     typeof(MockMetaFilter).GetProperty("Char"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Equal(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("Char")
                //             ),
                //             Expression.Constant('b')
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         Char_not = 'b'
                //     },
                //     FilterOperators.Not,
                //     "Char",
                //     typeof(MockMetaFilter).GetProperty("Char_not"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.NotEqual(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("Char")
                //             ),
                //             Expression.Constant('b')
                //         ),
                //         objectParam
                //     ),
                // },
                new object[] {
                    new MockMetaFilter {
                        Char_in = new[] { 'b', 'a' },
                    },
                    FilterOperators.In,
                    "Char",
                    typeof(MockMetaFilter).GetProperty("Char_in"),
                    Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                        Expression.Call(
                            null,
                            CachedReflection.IEnumerableContains(typeof(char)),
                            new Expression[] {
                                Expression.Constant(new[] { 'b', 'a' }),
                                Expression.MakeMemberAccess(
                                    objectParam,
                                    typeof(MockEntityForCursorVisitorTest).GetProperty("Char")
                                )
                            }
                        ),
                        objectParam
                    ),
                },
                // new object[] {
                //     new MockMetaFilter {
                //         Int = 5,
                //     },
                //     FilterOperators.Equal,
                //     "Int",
                //     typeof(MockMetaFilter).GetProperty("Int"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Equal(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("Int")
                //             ),
                //             Expression.Constant(5)
                //         ),
                //         objectParam
                //     ),
                // },
            };
    }
}