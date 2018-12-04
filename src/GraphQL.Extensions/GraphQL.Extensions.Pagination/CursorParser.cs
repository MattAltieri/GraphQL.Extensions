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
            this.cursorSegmentDelimiter = cursorSegmentDelimiter;
            this.cursorSubsegmentDelimiter = cursorSubsegmentDelimiter;
            
            OrderBy = orderBy;

            
        }


        public string CursorValue { get; set; }
        public OrderByInfo<TSource> OrderBy { get; set; }

        public Expression<Func<TSource, bool>> GetFilterPredicate() {

            var cursorSegments = CursorValue.Split(new[] { cursorSegmentDelimiter }, StringSplitOptions.None);
            int expectedDepth = OrderBy.Depth;
            if (cursorSegments.Length != expectedDepth && cursorSegments.Length != expectedDepth + 1)
                throw new InvalidOperationException("Cursor depth does not match OrderBy clause depth.");
            
            OrderByInfoBase<TSource> orderByNode = OrderBy;

            foreach(var cursorSegment in cursorSegments) {
                

            }
            throw new NotImplementedException();
        }
    }
}