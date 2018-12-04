using LinqKit;
using SuccincT.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GraphQL.Extensions.Internal;

namespace GraphQL.Extensions.Pagination {
    public class Cursor {
        
        public virtual StringBuilder CursorFormatString { get; set; }
        public virtual List<Expression> CursorExpressions { get; set; }

        public Cursor() {
            CursorFormatString = new StringBuilder();
            CursorExpressions = new List<Expression>();
        }

        public virtual MemberBinding GetMemberBinding<TSource>()
            where TSource : class {

            MethodCallExpression methodCall = Expression.Call(
                CachedReflection.StringFormat(),
                Expression.Constant(CursorFormatString.ToString()),
                Expression.NewArrayInit(typeof(object), CursorExpressions.Select(e => Expression.Constant(e)))
            );

            return Expression.Bind(typeof(TSource).GetMember("Cursor")[0], methodCall);
        }

        // public virtual Expression<Func<TSource, bool>> GetCursorFilter<TSource>(CursorFilterTypes filterType)
        //     where TSource : class {

        //     Expression<Func<TSource, bool>> predicate = PredicateBuilder.New<TSource>(true);
        //     OrderByInfo<TSource> orderBy

        //     string[] cursorSegments = CursorFormatString.ToString().Split('/');
        //     if (cursorSegments == null || cursorSegments.Count() == 0)
        //         throw new InvalidOperationException($"{nameof(CursorFormatString)} is empty.");
            
        //     if (orderBy == null || cursorSegments.Count() != orderBy.Depth)
        //         throw new InvalidOperationException($"{nameof(CursorFormatString)")

        //     foreach (string cursorPart in CursorFormatString.ToString().Split('/')) {
        //         // (string value, string direction, string name) = cursorPart.Split(':');
        //         var (value, (direction, (type, rest))) = cursorPart.Split(':');

        //         MemberInfo 
        //     }
        // }

        private BinaryExpression GetBinaryExpression(CursorFilterTypes filterType, SortDirections direction, Expression left, Expression right) {

            if (filterType == CursorFilterTypes.After)
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

        private ConstantExpression GetConstantExpression(string value, string type) {

            if (type == "string")
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentException($"{nameof(value)} of type string cannot be null or empty.");
                else
                    return Expression.Constant(value);
            else {            

                if (type == "char")
                    return Expression.Constant(char.Parse(value));
                else if (type == "short")
                    return Expression.Constant(short.Parse(value));
                else if (type == "int")
                    return Expression.Constant(int.Parse(value));
                else if (type == "long")
                    return Expression.Constant(long.Parse(value));
                else if (type == "decimal")
                    return Expression.Constant(decimal.Parse(value));
                else if (type == "float")
                    return Expression.Constant(float.Parse(value));
                else if (type == "double")
                    return Expression.Constant(double.Parse(value));
                else if (type == "bool")
                    return Expression.Constant(bool.Parse(value));
                else if (type == "DateTime")
                    return Expression.Constant(new DateTime(long.Parse(value)));
                else
                    throw new ArgumentException($"{type} is not a supported type.");
            }
        }
    }
}