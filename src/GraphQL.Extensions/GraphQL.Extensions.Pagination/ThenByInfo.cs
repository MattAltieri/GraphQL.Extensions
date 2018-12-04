using SuccincT.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GraphQL.Extensions.Pagination {
    public class ThenByInfo<TSource> : OrderByInfoBase<TSource>, IEquatable<ThenByInfo<TSource>>
        where TSource : class {

        public ThenByInfo() { }

        public ThenByInfo(string columnName, SortDirections sortDirection)
            => (ColumnName, SortDirection) = (columnName, sortDirection);

        public ThenByInfo(string columnName, SortDirections sortDirection, ThenByInfo<TSource> thenBy)
            : this(columnName, sortDirection)
            => ThenBy = thenBy;

        public ThenByInfo(IEnumerable<string> cursorSegments) {
            
            int count = cursorSegments?.Count() ?? 0;
            if (count == 0)
                throw new ArgumentNullException(nameof(cursorSegments));

            var (name, (value, (type, (direction, rest)))) = cursorSegments.ElementAt(0).Split(':');
            ColumnName = name;
            SortDirection = (SortDirections)Enum.Parse(typeof(SortDirections), direction);

            if (count > 1)
                ThenBy = new ThenByInfo<TSource>(cursorSegments.Skip(1).Take(count - 1));
        }

        public override IOrderedQueryable<TSource> Accept(SortVisitor<TSource> visitor)
            => visitor.Visit(this);

        public override Cursor Accept<TResult>(CursorVisitor<TSource, TResult> visitor)
            => visitor.Visit(this);

        public override int GetHashCode()
            => unchecked(
                (17 * 173) +
                ColumnName.GetHashCode() +
                SortDirection.GetHashCode() +
                (ThenBy?.GetHashCode() ?? 0)
            );

        public override bool Equals(object obj)
            => Equals(obj as ThenByInfo<TSource>);

        public virtual bool Equals(ThenByInfo<TSource> other) {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return ColumnName.Equals(other.ColumnName)
                && SortDirection.Equals(other.SortDirection)
                && Equals(ThenBy, other.ThenBy);
        }

        public static bool Equals(ThenByInfo<TSource> operand1, ThenByInfo<TSource> operand2)
            => operand1?.Equals(operand2) ?? ReferenceEquals(null, operand2);

        public static bool operator ==(ThenByInfo<TSource> operand1, ThenByInfo<TSource> operand2)
            // => operand1?.Equals(operand2) ?? ReferenceEquals(null, operand2);
            => Equals(operand1, operand2);

        public static bool operator !=(ThenByInfo<TSource> operand1, ThenByInfo<TSource> operand2)
            => !(operand1 == operand2);
    }
}