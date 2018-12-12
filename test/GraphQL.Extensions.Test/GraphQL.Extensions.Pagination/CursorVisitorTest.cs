using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Test;
using Moq;
using Xunit;
using Xunit2.Should;

namespace GraphQL.Extensions.Pagination {
    public class CursorVisitorTest {
        
        private ParameterExpression parameterExpression = Expression.Parameter(typeof(MockEntity), "o");

        public static List<object[]> GetSingleSortTestData
            => new List<object[]> {
                new object[] {
                    "Id",
                    SortDirections.Ascending,
                    "id::a::15",
                    
                }
            };
    }
}