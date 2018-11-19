using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GraphQL.Extensions.Pagination.Internal;

namespace GraphQL.Extensions.Pagination {
    public class CursorInjector : ICursorInjector {
        
        public static readonly ICursorInjector instance = new CursorInjector();

        static CursorInjector() { }

        private CursorInjector() { }

        public static ICursorInjector Instance => instance;

        public Expression<Func<TSource, TResult>> InjectIntoSelector<TSource, TResult>(
            Expression<Func<TSource, TResult>> selector,
            ISlicer slicer)
            where TSource : class
            where TResult : class, new() {
            
            throw new NotImplementedException();
        }
    }
}