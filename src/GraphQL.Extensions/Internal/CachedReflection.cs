using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GraphQL.Extensions.Internal {
    internal static class CachedReflection {

        private static HashSet<string> methodNames = new HashSet<string> {
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };

        private static IEnumerable<MethodInfo> s_ExtensionMethods;

        static CachedReflection() {
            s_ExtensionMethods =
                (from type in AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name == "System.Linq.Queryable").GetTypes()
                 where type.IsSealed
                 && !type.IsGenericType
                 && !type.IsNested
                 from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                 where method.IsDefined(typeof(ExtensionAttribute), false)
                 && methodNames.Contains(method.Name)
                 select method);
        }

        private static MethodInfo s_OrderBy;

        public static MethodInfo OrderBy(Type TSource, Type TKey)
            => (s_OrderBy ??
               (s_OrderBy = s_ExtensionMethods
                .Where(m => m.Name == "OrderBy" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_OrderByDescending;

        public static MethodInfo OrderByDescending(Type TSource, Type TKey)
            => (s_OrderByDescending ??
               (s_ExtensionMethods
                .Where(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_ThenBy;

        public static MethodInfo ThenBy(Type TSource, Type TKey)
            => (s_ThenBy ??
               (s_ExtensionMethods
                .Where(m => m.Name == "ThenBy" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_ThenByDescending;

        public static MethodInfo ThenByDescending(Type TSource, Type TKey)
            => (s_ThenByDescending ??
               (s_ExtensionMethods
                .Where(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);
    }
}