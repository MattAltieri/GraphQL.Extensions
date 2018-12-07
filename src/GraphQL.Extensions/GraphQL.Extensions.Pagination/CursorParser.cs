using LinqKit;
using System;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class CursorParser<TSource> : ICursorParser<TSource>
        where TSource : class {
        
        private readonly string cursorSegmentDelimiter;
        private readonly string cursorSubsegmentDelimiter;

        public CursorParser(
            string cursorValue,
            CursorFilterTypes cursorFilterType,
            string cursorSegmentDelimiter,
            string cursorSubsegmentDelimiter,
            OrderByInfo<TSource> orderBy) {
            
            if (string.IsNullOrWhiteSpace(cursorValue))
                throw new ArgumentNullException(nameof(cursorValue));
            if (string.IsNullOrEmpty(cursorSegmentDelimiter))
                throw new ArgumentNullException(nameof(cursorSegmentDelimiter));
            if (string.IsNullOrEmpty(cursorSubsegmentDelimiter))
                throw new ArgumentNullException(nameof(cursorSubsegmentDelimiter));
            if (orderBy == null)
                throw new ArgumentNullException(nameof(orderBy));
            
            CursorValue = cursorValue;
            CursorFilterType = cursorFilterType;
            this.cursorSegmentDelimiter = cursorSegmentDelimiter;
            this.cursorSubsegmentDelimiter = cursorSubsegmentDelimiter;
            
            OrderBy = orderBy;            
        }


        public string CursorValue { get; set; }
        public CursorFilterTypes CursorFilterType { get; set; }
        public OrderByInfo<TSource> OrderBy { get; set; }

        public Expression<Func<TSource, bool>> GetFilterPredicate() {

            var cursorSegments = CursorValue.Split(new[] { cursorSegmentDelimiter }, StringSplitOptions.None);

            if (cursorSegments.Length != OrderBy.Depth)
                throw new InvalidOperationException("Cursor depth does not match OrderBy clause depth.");
            
            Expression<Func<TSource, bool>> predicate = PredicateBuilder.New<TSource>(true);
            OrderByInfoBase<TSource> orderByNode = OrderBy;
            foreach(var cursorSegment in cursorSegments) {

                if (cursorSegment.ToLower() != orderByNode.GetCursorPrefix(cursorSubsegmentDelimiter).ToLower())
                    return Expression.Lambda<Func<TSource, bool>>(Expression.Constant(true), Expression.Parameter(typeof(TSource)));

                string cursorFieldValue = cursorSegment.Split(new[] { cursorSubsegmentDelimiter }, StringSplitOptions.None)[2];
                
                predicate.And(
                    Expression.Lambda<Func<TSource, bool>>(
                        GetBinaryExpression(
                            orderByNode.SortDirection,
                            orderByNode.GetMemberExpression(predicate.Parameters[0]),
                            GetConstantExpression(cursorFieldValue, orderByNode.GetMemberType())
                        ),
                        predicate.Parameters[0]
                    )
                );

                orderByNode = orderByNode.ThenBy;
            }
            
            return predicate;
        }
        
        private BinaryExpression GetBinaryExpression(SortDirections direction, Expression left, Expression right) {

            if (CursorFilterType == CursorFilterTypes.After)
                if (direction == SortDirections.Ascending)
                    return Expression.GreaterThan(left, right);
                else
                    return Expression.LessThan(left, right);
            else
                if (direction == SortDirections.Ascending)
                    return Expression.LessThan(left, right);
                else
                    return Expression.GreaterThan(left, right);
        }
        
        private ConstantExpression GetConstantExpression(string value, Type type) {

            if (type == typeof(string))
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException($"{nameof(value)} of type string cannot be null or empty.");
                else
                    return Expression.Constant(value);
            else {            

                if (type == typeof(char))
                    return Expression.Constant(char.Parse(value));
                else if (type == typeof(short))
                    return Expression.Constant(short.Parse(value));
                else if (type == typeof(int))
                    return Expression.Constant(int.Parse(value));
                else if (type == typeof(long))
                    return Expression.Constant(long.Parse(value));
                else if (type == typeof(decimal))
                    return Expression.Constant(decimal.Parse(value));
                else if (type == typeof(float))
                    return Expression.Constant(float.Parse(value));
                else if (type == typeof(double))
                    return Expression.Constant(double.Parse(value));
                else if (type == typeof(bool))
                    return Expression.Constant(bool.Parse(value));
                else if (type == typeof(DateTime))
                    return Expression.Constant(new DateTime(long.Parse(value)));
                else
                    throw new ArgumentException($"{type} is not a supported type.");
            }
        }
    }
}