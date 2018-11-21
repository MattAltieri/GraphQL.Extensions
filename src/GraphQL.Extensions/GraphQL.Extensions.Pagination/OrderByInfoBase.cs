using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GraphQL.Extensions.Pagination {
    public abstract class OrderByInfoBase<TSource>
        where TSource : class {

        private MemberInfo memberInfo;
        private Type memberType;

        public ThenByInfo<TSource> ThenBy { get; set; }

        public string ColumnName { get; set; }   

        public SortDirections SortDirection { get; set; }

        // public virtual IOrderedQueryable<TSource> Accept(OrderByVisitor<TSource> visitor) {
        //     if (ThenBy == null)
        //         return (IOrderedQueryable<TSource>)visitor.Query;
        //     return visitor.Visit(this.ThenBy);
        // }

        public abstract IOrderedQueryable<TSource> Accept(OrderByVisitor<TSource> visitor);

        public virtual MemberExpression GetMemberExpression(ParameterExpression param) {
            return Expression.MakeMemberAccess(param, GetMemberInfo());
        }

        public virtual MemberInfo GetMemberInfo() {
            if (memberInfo != null)
                return memberInfo;

            Type entityType = typeof(TSource);
            MemberInfo[] members = entityType.GetMember(ColumnName);
            if (members == null || members.Count() == 0)
                throw new ArgumentException($"{entityType.Name} does not contain a property or field named {ColumnName}.");
            return memberInfo = members[0];
        }

        public virtual Type GetMemberType() {
            if (memberType != null)
                return memberType;
            MemberInfo member = GetMemberInfo();
            return memberType = (member as PropertyInfo)?.PropertyType ?? ((FieldInfo)member).FieldType;
        }
    }
}