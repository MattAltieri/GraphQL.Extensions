using GraphQL.Extensions.Internal;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace GraphQL.Extensions.Filtering {
    public static class MetaFilterExtensions {
        
        public static Expression<Func<TSource, bool>> GetPredicate<TSource>(this IMetaFilter metaFilter, Type metaFilterType) {

            if (!typeof(IMetaFilter).IsAssignableFrom(metaFilterType))
                throw new ArgumentException(nameof(metaFilterType));

            MethodInfo predicateMethod =
                (from type in Assembly.GetExecutingAssembly().GetTypes()
                 where type.IsSealed
                 && !type.IsGenericType
                 && !type.IsNested
                 from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                 where method.IsDefined(typeof(ExtensionAttribute), false)
                 && method.Name == "GetPredicate"
                 select new {
                     Method = method,
                     Parameters = method.GetParameters(),
                     GenericArguments = method.GetGenericArguments()
                 })
                 .Where(m => m.GenericArguments.Count() == 2 && m.Parameters.Count() == 1 && m.Parameters[0].ParameterType == m.GenericArguments[1])
                 .Select(m => m.Method)
                 .First();
            predicateMethod = predicateMethod.MakeGenericMethod(typeof(TSource), metaFilterType);
            return (Expression<Func<TSource, bool>>)predicateMethod.Invoke(null, new object[] { metaFilter });
        }

        public static Expression<Func<TSource, bool>> GetPredicate<TSource, TMetaFilter>(this TMetaFilter metaFilter)
            where TSource : class, new()
            where TMetaFilter : IMetaFilter<TMetaFilter> {
            

            ParameterExpression param = Expression.Parameter(typeof(TSource), "f");
            return GetPredicate<TSource, TMetaFilter>(metaFilter, param);
        }

        internal static Expression<Func<TSource, bool>> GetPredicate<TSource, TMetaFilter>(TMetaFilter metaFilter, ParameterExpression param)
            where TSource : class, new()
            where TMetaFilter : IMetaFilter<TMetaFilter> {
            
            Expression<Func<TSource, bool>> predicate = PredicateBuilder.New<TSource>(true);

            if (metaFilter.And?.Count > 0)
                metaFilter.And.ToList().ForEach(o => predicate = predicate.And(GetPredicate<TSource, TMetaFilter>(o, param)));

            foreach(var filter in metaFilter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Name != "And" && m.Name != "Or")
                .Where(m => m.GetValue(metaFilter) != null)) {
                
                (FilterOperators op, string field) = ParseOperationAndFieldFromFilter(filter);

                predicate = predicate.And(Compare<TSource, TMetaFilter>(metaFilter, op, field, param, filter));
            }

            if (metaFilter.Or?.Count > 0) {
                predicate = (PredicateBuilder.New<TSource>(false)).Or(predicate);
                metaFilter.Or.ToList().ForEach(o => predicate = predicate.Or(GetPredicate<TSource, TMetaFilter>(o, param)));
            }

            return predicate;
        }
                
        internal static (FilterOperators, string) ParseOperationAndFieldFromFilter(MemberInfo filter) {

            Type type = (filter as PropertyInfo)?.PropertyType ?? ((FieldInfo)filter).FieldType;
            string[] segments = filter.Name.Split('_');
            string opString = "";
            bool match = false;
            StringBuilder field = new StringBuilder();
            FilterOperators op = 0;

            for (int i = segments.Length - 1; i >= 0; i--) {

                opString = segments[i] + opString;
                match = Enum.TryParse(opString, true, out op);

                if (match) {
                    if (i != 0 && segments[i - 1] == "not") {
                        string tempOpString = "not" + opString;
                        bool tempMatch;
                        FilterOperators tempOp = 0;
                        tempMatch = Enum.TryParse(tempOpString, true, out tempOp);
                        if (tempMatch) {
                            op = tempOp;
                            i--;
                        }
                    }

                    for (int j = 0; j <= i - 1; j++) {
                        field.Append(segments[j]);
                        if (j < i - 1)
                            field = field.Append("_");
                    }
                    break;
                }
            }

            if (!match) {
                op = FilterOperators.Equal;
                field = new StringBuilder(filter.Name);
            }

            return (op, field.ToString());
        }

        internal static Expression<Func<TSource, bool>> Compare<TSource, TMetaFilter>(TMetaFilter metaFilter, FilterOperators op,
            string memberName, ParameterExpression param, MemberInfo filterMemberInfo)
            where TSource : class, new()
            where TMetaFilter : IMetaFilter<TMetaFilter> {
            
            MemberTypes memberType = filterMemberInfo.MemberType;
            Type filterType;

            object filterValue;
            if (memberType == MemberTypes.Property) {
                filterValue = ((PropertyInfo)filterMemberInfo).GetValue(metaFilter);
                filterType = ((PropertyInfo)filterMemberInfo).PropertyType;
            } else if (memberType == MemberTypes.Field) {
                filterValue = ((FieldInfo)filterMemberInfo).GetValue(metaFilter);
                filterType = ((FieldInfo)filterMemberInfo).FieldType;
            } else
                throw new ArgumentException($"{nameof(filterMemberInfo)} must be of type {nameof(PropertyInfo)} or {nameof(FieldInfo)}.");

            MemberInfo memberInfo = typeof(TSource).GetMember(memberName).First();
            MemberExpression property = Expression.MakeMemberAccess(param, memberInfo);

            Expression expression = null;
            ConstantExpression filterValueExpression = Expression.Constant(filterValue, filterType);

            switch (op) {
                case FilterOperators.Equal:
                    expression = Expression.Equal(property, filterValueExpression);
                    break;

                case FilterOperators.Not:
                    expression = Expression.NotEqual(property, filterValueExpression);
                    break;

                case FilterOperators.In:
                case FilterOperators.NotIn:
                    MethodInfo listContainsMethod = CachedReflection.IEnumerableContains(filterType.GetElementType());

                    expression = Expression.Call(null, listContainsMethod, filterValueExpression, property);

                    if (op == FilterOperators.NotIn)
                        expression = Expression.Not(expression);
                    break;

                case FilterOperators.lt:
                    expression = Expression.LessThan(property, filterValueExpression);
                    break;

                case FilterOperators.lte:
                    expression = Expression.LessThanOrEqual(property, filterValueExpression);
                    break;

                case FilterOperators.gt:
                    expression = Expression.GreaterThan(property, filterValueExpression);
                    break;

                case FilterOperators.gte:
                    expression = Expression.GreaterThanOrEqual(property, filterValueExpression);
                    break;

                case FilterOperators.Contains:
                case FilterOperators.NotContains:
                    MethodInfo containsMethod = CachedReflection.StringContains();
                    expression = Expression.Call(property, containsMethod, filterValueExpression);
                    if (op == FilterOperators.NotContains)
                        expression = Expression.Not(expression);
                    break;

                case FilterOperators.StartsWith:
                case FilterOperators.NotStartsWith:
                    MethodInfo startsWithMethod = CachedReflection.StringStartsWith();
                    expression = Expression.Call(property, startsWithMethod, filterValueExpression);
                    if (op == FilterOperators.NotStartsWith)
                        expression = Expression.Not(expression);
                    break;

                case FilterOperators.EndsWith:
                case FilterOperators.NotEndsWith:
                    MethodInfo endsWithMethod = CachedReflection.StringEndsWith();
                    expression = Expression.Call(property, endsWithMethod, filterValueExpression);
                    if (op == FilterOperators.NotEndsWith)
                        expression = Expression.Not(expression);
                    break;

                case FilterOperators.Null:
                case FilterOperators.NotNull:

                    bool notNull = false;
                    if ((op == FilterOperators.Null && !(bool)filterValue) ||
                        (op == FilterOperators.NotNull && (bool)filterValue)) {
                        
                        notNull = true;
                    }

                    expression = Expression.Equal(property, Expression.Constant(null));
                    if (notNull)
                        expression = Expression.Not(expression);
                    break;

                case FilterOperators.Empty:
                case FilterOperators.NotEmpty:

                    bool notEmpty = false;
                    if ((op == FilterOperators.Empty && !(bool)filterValue) ||
                        (op == FilterOperators.NotEmpty && (bool)filterValue)) {
                        
                        notEmpty = true;
                    }

                    MethodInfo isEmptyMethod = CachedReflection.StringIsNullOrEmpty();
                    expression = Expression.Call(null, isEmptyMethod, property);
                    if (notEmpty)
                        expression = Expression.Not(expression);
                    break;
            }

            return Expression.Lambda<Func<TSource, bool>>(expression, param);
        }

        internal static (Expression, Expression) HandleNullableComparison(Expression left, Expression right) {

            Type underlyingLeft = Nullable.GetUnderlyingType(left.Type);
            Type underlyingRight = Nullable.GetUnderlyingType(right.Type);
            
            /* If both types are not Nullable<T>, Expression.Equal and other binary operators works normally
             * If both types are Nullable<T>, Expression.Equal will use a lifted operator.
             *              * 
             * An issue arrises when one type is Nullable<T> and the other is T. The operator cannot be lifted, so
             * this condition must be tested for and the non-nullable expression
             */

            // If both left and right are nullable, -OR- neither is, return the expressions as-is
            if (ReferenceEquals(underlyingLeft, underlyingRight)) {
                
                return (left, right);
            } else { // otherwise, wrap whichever expression is non-nullable in a unary expression to cast it to nullable
                Expression newLeft = underlyingLeft == null
                    ? Expression.Convert(left, typeof(Nullable<>).MakeGenericType(left.Type))
                    : left;
                Expression newRight = underlyingRight == null
                    ? Expression.Convert(right, typeof(Nullable<>).MakeGenericType(right.Type))
                    : right;

                return (newLeft, newRight);
            }
        }
    }
}