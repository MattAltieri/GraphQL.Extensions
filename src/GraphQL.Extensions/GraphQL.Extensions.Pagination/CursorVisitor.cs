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
        public virtual int Index { get; protected set; }

        public CursorVisitor() => Index = 0;

        public CursorVisitor(CursorVisitor<TSource, TResult> visitor)
            : this()
            => Parameter = visitor.Parameter;

        public CursorVisitor(ParameterExpression param)
            : this()
            => Parameter = param;

        public virtual Cursor Visit(OrderByInfo<TSource> orderBy) {

            Type type = orderBy.GetMemberType();
            MemberExpression memberExpression = orderBy.GetMemberExpression(Parameter);
            
            Cursor cursor = new Cursor();
            cursor.CursorFormatString.Append($"{{{Index.ToString()}}}:{orderBy.SortDirection.ToString()}:{type.Name}");
            cursor.CursorExpressions.Add(GetCursorPart(type, memberExpression));

            Index++;

            if (orderBy.ThenBy != null) {
                Cursor thenByCursor = orderBy.ThenBy.Accept<TResult>(this);
                cursor.CursorFormatString.Append("/");
                cursor.CursorFormatString.Append(thenByCursor.CursorFormatString);
                cursor.CursorExpressions.AddRange(thenByCursor.CursorExpressions);
            }

            return cursor;
        }

        public virtual Cursor Visit(ThenByInfo<TSource> thenBy) {
            
            Type type = thenBy.GetMemberType();
            MemberExpression memberExpression = thenBy.GetMemberExpression(Parameter);

            Cursor cursor = new Cursor();
            cursor.CursorFormatString.Append($"{{{Index.ToString()}}}:{thenBy.SortDirection.ToString()}");
            cursor.CursorExpressions.Add(GetCursorPart(type, memberExpression));

            Index++;

            if (thenBy.ThenBy != null) {
                Cursor thenByCursor = thenBy.ThenBy.Accept<TResult>(this);
                cursor.CursorFormatString.Append("/");
                cursor.CursorFormatString.Append(thenByCursor.CursorFormatString);
                cursor.CursorExpressions.AddRange(thenByCursor.CursorExpressions);
            }

            return cursor;
        }

        private Expression GetCursorPart(Type type, MemberExpression memberExpression) {

            if (!type.IsValueType)
                throw new ArgumentException($"{nameof(type)} is not a value type.");

            Type underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
                return NullableCursorPart(type, memberExpression);
            else if (type == typeof(DateTime))
                return DateTimeCursorPart(memberExpression);
            else if (PrimitiveTypes.Contains(type))
                return PrimitiveCursorPart(type, memberExpression);
            else if (type == typeof(string))
                return memberExpression;
            else
                throw new ArgumentException($"{type.Name} is not a supported type.");
        }

        private Expression DateTimeCursorPart(MemberExpression memberExpression) {
            MemberExpression ticks = Expression.MakeMemberAccess(memberExpression, CachedReflection.DateTimeTicks());
            return PrimitiveCursorPart(typeof(long), ticks);
        }

        private Expression PrimitiveCursorPart(Type type, MemberExpression memberExpression)
            => Expression.Call(memberExpression, CachedReflection.ToString(type));

        private Expression NullableCursorPart(Type type, MemberExpression memberExpression) {

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

            return Expression.Condition(Expression.Not(propertyHasValue), Expression.Constant(" "), methodCallExpression);
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