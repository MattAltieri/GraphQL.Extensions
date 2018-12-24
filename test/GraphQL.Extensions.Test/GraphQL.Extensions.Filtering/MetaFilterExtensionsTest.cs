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

        [Theory]
        [MemberData(nameof(GetNullableTestData))]
        public void Should_WrapNonNullableSideOfBinaryExpression_When_OnlyOneIsNullable(Expression left, Expression right,
            Expression expectedLeft, Expression expectedRight) {
            
            Expression newLeft = null, newRight = null;
            Exception exception = Record.Exception(() =>
                (newLeft, newRight) = MetaFilterExtensions.HandleNullableComparison(left, right));
            exception.ShouldBeNull();
            newLeft.ShouldNotBeNull();
            newRight.ShouldNotBeNull();

            ExpressionTreeComparer comparer = new ExpressionTreeComparer();
            bool? result = null;
            exception = Record.Exception(() => result = comparer.Equals(newLeft, expectedLeft));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBeTrue();

            result = null;
            exception = Record.Exception(() => result = comparer.Equals(newRight, expectedRight));
            exception.ShouldBeNull();
            result.HasValue.ShouldBeTrue();
            result.Value.ShouldBeTrue();
        }
        
        public static List<object[]> TestComparisonData
            => new List<object[]> {
                new object[] {
                    new MockMetaFilter {
                        Char = 'b'
                    },
                    FilterOperators.Equal,
                    "Char",
                    typeof(MockMetaFilter).GetProperty("Char"),
                    Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                        Expression.Equal(
                            Expression.MakeMemberAccess(
                                objectParam,
                                typeof(MockEntityForCursorVisitorTest).GetProperty("Char")
                            ),
                            Expression.Constant('b', typeof(char))
                        ),
                        objectParam
                    ),
                },
                new object[] {
                    new MockMetaFilter {
                        Char_not = 'b'
                    },
                    FilterOperators.Not,
                    "Char",
                    typeof(MockMetaFilter).GetProperty("Char_not"),
                    Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                        Expression.NotEqual(
                            Expression.MakeMemberAccess(
                                objectParam,
                                typeof(MockEntityForCursorVisitorTest).GetProperty("Char")
                            ),
                            Expression.Constant('b', typeof(char))
                        ),
                        objectParam
                    ),
                },
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
                new object[] {
                    new MockMetaFilter {
                        Char_not_in = new[] { 'b', 'a' },
                    },
                    FilterOperators.NotIn,
                    "Char",
                    typeof(MockMetaFilter).GetProperty("Char_not_in"),
                    Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                        Expression.Not(
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
                            )
                        ),
                        objectParam
                    ),
                },
                new object[] {
                    new MockMetaFilter {
                        CharN = 'b'
                    },
                    FilterOperators.Equal,
                    "CharNull",
                    typeof(MockMetaFilter).GetProperty("CharN"),
                    Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                        Expression.Equal(
                            Expression.MakeMemberAccess(
                                objectParam,
                                typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                            ),
                            Expression.Constant('b', typeof(Nullable<char>))
                        ),
                        objectParam
                    ),
                },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_not = 'b',
                //     },
                //     FilterOperators.Not,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_not"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.NotEqual(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //             ),
                //             Expression.Constant('b')
                //         ),
                //         objectParam
                //     )
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_in = new[] { 'b', 'a' },
                //     },
                //     FilterOperators.In,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_in"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Call(
                //             null,
                //             CachedReflection.IEnumerableContains(typeof(char)),
                //             new Expression[] {
                //                 Expression.Constant(new[] { 'b', 'a' }),
                //                 Expression.MakeMemberAccess(
                //                     objectParam,
                //                     typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //                 )
                //             }
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_not_in = new[] { 'b', 'a' },
                //     },
                //     FilterOperators.NotIn,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_not_in"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Not(
                //             Expression.Call(
                //                 null,
                //                 CachedReflection.IEnumerableContains(typeof(char)),
                //                 new Expression[] {
                //                     Expression.Constant(new[] { 'b', 'a' }),
                //                     Expression.MakeMemberAccess(
                //                         objectParam,
                //                         typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //                     )
                //                 }
                //             )
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_null = true,
                //     },
                //     FilterOperators.Null,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_null"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Equal(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //             ),
                //             Expression.Constant(null)
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_null = false,
                //     },
                //     FilterOperators.Null,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_null"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.NotEqual(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //             ),
                //             Expression.Constant(null)
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_not_null = true,
                //     },
                //     FilterOperators.NotNull,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_not_null"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.NotEqual(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //             ),
                //             Expression.Constant(null)
                //         ),
                //         objectParam
                //     ),
                // },
                // new object[] {
                //     new MockMetaFilter {
                //         CharN_not_null = false,
                //     },
                //     FilterOperators.NotNull,
                //     "CharNull",
                //     typeof(MockMetaFilter).GetProperty("CharN_not_null"),
                //     Expression.Lambda<Func<MockEntityForCursorVisitorTest, bool>>(
                //         Expression.Equal(
                //             Expression.MakeMemberAccess(
                //                 objectParam,
                //                 typeof(MockEntityForCursorVisitorTest).GetProperty("CharNull")
                //             ),
                //             Expression.Constant(null)
                //         ),
                //         objectParam
                //     ),
                // },
            };

        public static List<object[]> GetNullableTestData
            => new List<object[]> {
                new object[] {
                    Expression.Constant(1, typeof(int)), // left
                    Expression.Constant(1, typeof(int)), // right
                    Expression.Constant(1, typeof(int)), // expectedLeft
                    Expression.Constant(1, typeof(int)), // expectedRight
                },
                new object[] {
                    Expression.Constant(1, typeof(int)), // left
                    Expression.Constant(2, typeof(int)), // right
                    Expression.Constant(1, typeof(int)), // expectedLeft
                    Expression.Constant(2, typeof(int)), // expectedRight
                },
                new object[] {
                    Expression.Constant(1, typeof(int?)), // left
                    Expression.Constant(1, typeof(int?)), // right
                    Expression.Constant(1, typeof(int?)), // expectedLeft
                    Expression.Constant(1, typeof(int?)), // expectedRight
                },
                new object[] {
                    Expression.Constant(1, typeof(int)), // left
                    Expression.Constant(1, typeof(int?)), // right
                    Expression.Convert(
                        Expression.Constant(1, typeof(int)),
                        typeof(int?)
                    ), // expectedLeft
                    Expression.Constant(1, typeof(int?)), // expectedRight
                },
                new object[] {
                    Expression.Constant(1, typeof(int?)), // left
                    Expression.Constant(1, typeof(int)), // right
                    Expression.Constant(1, typeof(int?)), // expectedLeft
                    Expression.Convert(
                        Expression.Constant(1, typeof(int)),
                        typeof(int?)
                    ), // expectedRight
                },
                new object[] {
                    Expression.Constant(1, typeof(int)), // left
                    Expression.Constant(null, typeof(int?)), // right
                    Expression.Convert(
                        Expression.Constant(1, typeof(int)),
                        typeof(int?)
                    ), // expectedLeft
                    Expression.Constant(null, typeof(int?)), // expectedRight
                },
                new object[] {
                    Expression.Constant("abc", typeof(string)), // left
                    Expression.Constant("abc", typeof(string)), // right
                    Expression.Constant("abc", typeof(string)), // expectedLeft
                    Expression.Constant("abc", typeof(string)), // expectedRight
                },
                new object[] {
                    Expression.Constant("abc", typeof(string)), // left
                    Expression.Constant(null, typeof(string)), // right
                    Expression.Constant("abc", typeof(string)), // expectedLeft
                    Expression.Constant(null, typeof(string)), // expectedRight
                },
            };
    }
}