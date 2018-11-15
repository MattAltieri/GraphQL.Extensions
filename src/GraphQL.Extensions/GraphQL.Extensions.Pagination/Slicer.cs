using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using GraphQL.Extensions.Pagination.Internal;

namespace GraphQL.Extensions.Pagination {
    public class Slicer : ISlicer {
        public string OrderBy { get; set; }
        public int? First { get; set; }
        public string Before { get; set; }
        public string After { get; set; }

        // internal Dictionary<int, OrderByColumn> ParseOrderBy<TEntity>(ParameterExpression param)
        //     where TEntity : class {
            
        //     Dictionary<int, OrderByColumn> results = new Dictionary<int, OrderByColumn>();

        //     int i = 0;
        //     foreach (string segment in OrderBy.Split(',').Select(s => s.Trim())) {
        //         string columnName = Regex.Replace(segment, " (desc[ending]?|asc[ending]?)", "", RegexOptions.IgnoreCase);
        //         SortDirections direction = Regex.Matches(segment, " desc[ending]?", RegexOptions.IgnoreCase).Count > 0
        //             ? SortDirections.Descending
        //             : SortDirections.Ascending;
                
        //         MemberInfo memberInfo = typeof(TEntity).GetMember(columnName)[0];
        //         results.Add(i, new OrderByColumn {
        //             Name = columnName,
        //             Type =  (memberInfo as PropertyInfo)?.PropertyType ?? ((FieldInfo)memberInfo).FieldType,
        //             MemberInfo = memberInfo,
        //             MemberExpression = Expression.MakeMemberAccess(param, memberInfo),
        //             SortDirection = direction
        //         });
        //         i++;
        //     }
        //     return results;
        // }
    }
}