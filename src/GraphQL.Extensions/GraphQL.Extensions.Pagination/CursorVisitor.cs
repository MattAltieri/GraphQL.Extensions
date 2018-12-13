using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class CursorVisitor<TSource, TResult>
        where TSource : class
        where TResult : class, new() {
        
        public virtual ParameterExpression Parameter { get; protected set; }
        public virtual string CursorSegmentDelimiter { get; protected set; }
        public virtual string CursorSubsegmentDelimiter { get; protected set; }
        public virtual int Index { get; protected set; }

        public CursorVisitor() => Index = 0;

        public CursorVisitor(CursorVisitor<TSource, TResult> visitor)
            /* : this() */  {
            
            Parameter = visitor.Parameter;
            CursorSegmentDelimiter = visitor.CursorSegmentDelimiter;
            CursorSubsegmentDelimiter = visitor.CursorSubsegmentDelimiter;
        }

        public CursorVisitor(ParameterExpression param, string cursorSegmentDelimiter, string cursorSubsegmentDelimiter)
            /* : this() */ {
            
            Parameter = param;
            CursorSegmentDelimiter = cursorSegmentDelimiter;
            CursorSubsegmentDelimiter = CursorSubsegmentDelimiter;
        }

        public virtual Cursor Visit(OrderByInfo<TSource> orderBy) {

            Type type = orderBy.GetMemberType();
            MemberExpression memberExpression = orderBy.GetMemberExpression(Parameter);
            
            Cursor cursor = new Cursor();
            cursor.CursorFormatString.Append(orderBy.SortDirection == SortDirections.Ascending ? "a" : "d");
            cursor.CursorFormatString.Append(CursorSubsegmentDelimiter);
            cursor.CursorFormatString.Append(orderBy.ColumnName.ToLower().ToString());
            cursor.CursorFormatString.Append(CursorSubsegmentDelimiter);
            cursor.CursorFormatString.Append($"{{{Index.ToString()}}}");
            cursor.CursorExpressions.Add(GetCursorPart(type, memberExpression));

            Index++;

            if (orderBy.ThenBy != null) {
                Cursor thenByCursor = orderBy.ThenBy.Accept<TResult>(this);
                cursor.CursorFormatString.Append(CursorSegmentDelimiter);
                cursor.CursorFormatString.Append(thenByCursor.CursorFormatString);
                cursor.CursorExpressions.AddRange(thenByCursor.CursorExpressions);
            }

            return cursor;
        }

        public virtual Cursor Visit(ThenByInfo<TSource> thenBy) {
            
            Type type = thenBy.GetMemberType();
            MemberExpression memberExpression = thenBy.GetMemberExpression(Parameter);

            Cursor cursor = new Cursor();
            cursor.CursorFormatString.Append(thenBy.SortDirection == SortDirections.Ascending ? "a" : "d");
            cursor.CursorFormatString.Append(CursorSubsegmentDelimiter);
            cursor.CursorFormatString.Append(thenBy.ColumnName.ToLower().ToString());
            cursor.CursorFormatString.Append(CursorSubsegmentDelimiter);
            cursor.CursorFormatString.Append($"{{{Index.ToString()}}}");
            cursor.CursorExpressions.Add(GetCursorPart(type, memberExpression));

            Index++;

            if (thenBy.ThenBy != null) {
                Cursor thenByCursor = thenBy.ThenBy.Accept<TResult>(this);
                cursor.CursorFormatString.Append(CursorSegmentDelimiter);
                cursor.CursorFormatString.Append(thenByCursor.CursorFormatString);
                cursor.CursorExpressions.AddRange(thenByCursor.CursorExpressions);
            }

            return cursor;
        }

        protected virtual Expression GetCursorPart(Type type, MemberExpression memberExpression) {
            
            if (type == typeof(string))
                return memberExpression;

            if (!type.IsValueType)
                throw new ArgumentException($"{nameof(type)} is not a value type.");

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return NullableCursorPart(type, memberExpression);
            else if (type == typeof(DateTime))
                return DateTimeCursorPart(memberExpression);
            else if (PrimitiveTypes.Contains(type))
                return PrimitiveCursorPart(type, memberExpression);
            else
                throw new ArgumentException($"{type.Name} is not a supported type.");
        }

        protected virtual Expression DateTimeCursorPart(MemberExpression memberExpression) {
            MemberExpression ticks = Expression.MakeMemberAccess(memberExpression, CachedReflection.DateTimeTicks());
            return PrimitiveCursorPart(typeof(long), ticks);
        }

        protected virtual Expression PrimitiveCursorPart(Type type, MemberExpression memberExpression)
            => Expression.Call(memberExpression, CachedReflection.ToString(type));

        protected virtual Expression NullableCursorPart(Type type, MemberExpression memberExpression) {

            Type underlyingType = Nullable.GetUnderlyingType(type);

            MemberExpression propertyHasValue = Expression.Property(memberExpression, CachedReflection.NullableHasValue(type));
            MemberExpression propertyValue = Expression.Property(memberExpression, CachedReflection.NullableValue(type));

            MethodCallExpression methodCallExpression;
            if (underlyingType == typeof(DateTime))
                methodCallExpression = (MethodCallExpression)DateTimeCursorPart(propertyValue);
            else if (PrimitiveTypes.Contains(underlyingType))
                methodCallExpression = (MethodCallExpression)PrimitiveCursorPart(underlyingType, propertyValue);
            else
                throw new ArgumentException($"Underlying type of {nameof(type)} must be DateTime or primative.");

            return Expression.Condition(Expression.Not(propertyHasValue), Expression.Constant("\0"), methodCallExpression);
        }

        private static IEnumerable<Type> PrimitiveTypes
            => new List<Type> {
                typeof(char),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(float),
                typeof(double),
                typeof(bool),
            };
    }
}