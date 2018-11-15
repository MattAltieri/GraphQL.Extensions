using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GraphQL.Extensions.Pagination.Internal {
    internal class OrderByColumn {

        public OrderByColumn(string orderByEntry, Type entityType, ParameterExpression entityParameter) {

            Name = Regex.Replace(orderByEntry, " (desc[ending]?|asc[ending]?)", "", RegexOptions.IgnoreCase);
            SortDirection = Regex.Matches(orderByEntry, " desc[ending]?", RegexOptions.IgnoreCase).Count > 0
                ? SortDirections.Descending
                : SortDirections.Ascending;
            MemberInfo = entityType.GetMember(Name)[0];
            Type = (MemberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)MemberInfo).FieldType;
            MemberExpression = Expression.MakeMemberAccess(entityParameter, MemberInfo);
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public MemberInfo MemberInfo { get; set; }
        public MemberExpression MemberExpression { get; set; }
        public SortDirections SortDirection { get; set; }
    }
}