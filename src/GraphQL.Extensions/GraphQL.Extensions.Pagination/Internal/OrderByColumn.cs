// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using System.Text.RegularExpressions;

// namespace GraphQL.Extensions.Pagination.Internal {
//     internal class OrderByColumn {

//         public OrderByColumn() { }

//         public OrderByColumn(string orderByEntry, Type entityType, ParameterExpression entityParameter)
//             : this() {

//             Name = Regex.Replace(orderByEntry, " (descending|ascending)", "", RegexOptions.IgnoreCase);
//             Name = Regex.Replace(Name, " (desc|asc)", "", RegexOptions.IgnoreCase);

//             MemberInfo[] members = entityType.GetMember(Name);
//             int? memberCount = members?.Count();
//             if (!memberCount.HasValue || memberCount.Value == 0)
//                 throw new ArgumentException($"{nameof(orderByEntry)} does not contain a property or field named '{orderByEntry}'.");
//             MemberInfo = members[0];

//             SortDirection = Regex.Matches(orderByEntry, " desc[ending]?", RegexOptions.IgnoreCase).Count > 0
//                 ? SortDirections.Descending
//                 : SortDirections.Ascending;
//             Type = (MemberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)MemberInfo).FieldType;
//             MemberExpression = Expression.MakeMemberAccess(entityParameter, MemberInfo);
//         }

//         public string Name { get; set; }
//         public Type Type { get; set; }
//         public MemberInfo MemberInfo { get; set; }
//         public MemberExpression MemberExpression { get; set; }
//         public SortDirections SortDirection { get; set; }
//     }
// }