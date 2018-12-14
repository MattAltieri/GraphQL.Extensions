using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Extensions.Internal;
using GraphQL.Extensions.Pagination;

namespace System.Linq{
    public static class IQueryableSlicerExtensions {

        private static void AssertSliceArguments<TSource>(
            IQueryable<TSource> query,
            SlicerBase<TSource> slicer,
            string cursorSegmentDelimiter,
            string cursorSubsegmentDelimiter)
            where TSource : class {
            
            if (query == null)
                throw new ArgumentNullException($"'{nameof(query)}; must not be null.");
            if (slicer == null)
                throw new ArgumentNullException($"'{nameof(slicer)}' must not be null.");
            if (slicer.OrderBy == null)
                throw new ArgumentNullException($"'{nameof(slicer.OrderBy)}' must not be null.");
            if (cursorSegmentDelimiter == string.Empty)
                throw new ArgumentException($"'{nameof(cursorSegmentDelimiter)}' must not be an empty string");
            if (cursorSubsegmentDelimiter == string.Empty)
                throw new ArgumentException($"'{nameof(cursorSubsegmentDelimiter)}' must not be an empty string");
        }

        public static IOrderedQueryable<TSource> Slice<TSource>(
            this IQueryable<TSource> query,
            BeforeSlicer<TSource> slicer,
            string cursorSegmentDelimiter = "//",
            string cursorSubsegmentDelimiter = "::")
            where TSource : class  {
            
            AssertSliceArguments<TSource>(query, slicer, cursorSegmentDelimiter, cursorSubsegmentDelimiter);
            
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), "f");
            
            // Apply slicer's OrderBy clause
            IOrderedQueryable<TSource> orderedQuery = (new SortVisitor<TSource>(query, parameterExpression)).Visit(slicer.OrderBy);

            // Filter for previous page
            if (!string.IsNullOrWhiteSpace(slicer.Before)) {
                CursorVisitor<TSource> cursorVisitor =
                    new CursorVisitor<TSource>(parameterExpression, cursorSegmentDelimiter, cursorSubsegmentDelimiter);

                Cursor cursor = cursorVisitor.Visit(slicer.OrderBy);
                CursorParser<TSource> cursorParser = new CursorParser<TSource>(
                    slicer.Before,
                    CursorFilterTypes.Before,
                    cursorSegmentDelimiter,
                    cursorSubsegmentDelimiter,
                    slicer.OrderBy
                );

                orderedQuery = (IOrderedQueryable<TSource>)orderedQuery.Where(cursorParser.GetFilterPredicate());
            }

            // Slice to a single page
            if (slicer.First.HasValue)
                orderedQuery = (IOrderedQueryable<TSource>)orderedQuery.Take(slicer.First.Value);

            return orderedQuery;
        }

        public static IOrderedQueryable<TSource> Slice<TSource>(
            this IQueryable<TSource> query,
            AfterSlicer<TSource> slicer,
            string cursorSegmentDelimiter = "//",
            string cursorSubsegmentDelimiter = "::")
            where TSource : class {
            
            AssertSliceArguments<TSource>(query, slicer, cursorSegmentDelimiter, cursorSubsegmentDelimiter);

            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), "f");
            
            // Apply slicer's OrderBy clause
            IOrderedQueryable<TSource> orderedQuery = (new SortVisitor<TSource>(query, parameterExpression)).Visit(slicer.OrderBy);

            // Filter for previous page
            if (!string.IsNullOrWhiteSpace(slicer.After)) {
                CursorVisitor<TSource> cursorVisitor =
                    new CursorVisitor<TSource>(parameterExpression, cursorSegmentDelimiter, cursorSubsegmentDelimiter);

                Cursor cursor = cursorVisitor.Visit(slicer.OrderBy);
                CursorParser<TSource> cursorParser = new CursorParser<TSource>(
                    slicer.After,
                    CursorFilterTypes.After,
                    cursorSegmentDelimiter,
                    cursorSubsegmentDelimiter,
                    slicer.OrderBy
                );

                orderedQuery = (IOrderedQueryable<TSource>)orderedQuery.Where(cursorParser.GetFilterPredicate());
            }

            // Slice to a single page
            if (slicer.First.HasValue)
                orderedQuery = (IOrderedQueryable<TSource>)orderedQuery.Take(slicer.First.Value);
                
            return orderedQuery;
        }

        public static IOrderedQueryable<TSource> Slice<TSource>(
            this IQueryable<TSource> query,
            Slicer<TSource> slicer,
            string cursorSegmentDelimiter = "//",
            string cursorSubsegmentDelimiter = "::")
            where TSource : class {
            
            AssertSliceArguments<TSource>(query, slicer, cursorSegmentDelimiter, cursorSubsegmentDelimiter);
            
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource), "f");
            
            // Apply slicer's OrderBy clause
            IOrderedQueryable<TSource> orderedQuery = (new SortVisitor<TSource>(query, parameterExpression)).Visit(slicer.OrderBy);

            // Slice to a single page
            if (slicer.First.HasValue)
                orderedQuery = (IOrderedQueryable<TSource>)orderedQuery.Take(slicer.First.Value);
                
            return orderedQuery;
        }

        public static IQueryable<TResult> InjectCursor<TSource, TResult>(
            this IQueryable<TSource> query,
            OrderByInfo<TSource> orderBy,
            Expression<Func<TSource, TResult>> selector,
            string cursorSegmentDelimiter = "//",
            string cursorSubsegmentDelimiter = "::")
            where TSource : class
            where TResult : class, new() {
            
            if (query == null)
                throw new ArgumentNullException($"'{nameof(query)}; must not be null.");
            if (orderBy == null)
                throw new ArgumentNullException($"'{nameof(orderBy)}' must not be null.");
            if (selector == null)
                throw new ArgumentNullException($"'{nameof(selector)}' must not be null.");
            if (cursorSegmentDelimiter == string.Empty)
                throw new ArgumentException($"'{nameof(cursorSegmentDelimiter)}' must not be an empty string");
            if (cursorSubsegmentDelimiter == string.Empty)
                throw new ArgumentException($"'{nameof(cursorSubsegmentDelimiter)}' must not be an empty string");
            
            CursorInjector<TSource, TResult> cursorInjector = new CursorInjector<TSource, TResult>(orderBy, cursorSegmentDelimiter,
                cursorSubsegmentDelimiter);
            return query.Select(cursorInjector.InjectIntoSelector(selector));
        }
    }
}