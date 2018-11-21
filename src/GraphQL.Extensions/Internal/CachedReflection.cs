using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GraphQL.Extensions.Internal {
    internal static class CachedReflection {

        private static HashSet<string> linq_Queryable_MethodNames = new HashSet<string> {
            "OrderBy",
            "OrderByDescending",
            "ThenBy",
            "ThenByDescending"
        };
        private static HashSet<string> linq_Expressions_MethodNames = new HashSet<string> {
            "Lambda"
        };

        private static List<MethodInfo> s_ExtensionMethods;

        static CachedReflection() {
            s_ExtensionMethods = new List<MethodInfo>();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                
            s_ExtensionMethods.AddRange(
                (from type in assemblies.SingleOrDefault(a => a.GetName().Name == "System.Linq.Queryable").GetTypes()
                 where type.IsSealed
                 && !type.IsGenericType
                 && !type.IsNested
                 from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                 where method.IsDefined(typeof(ExtensionAttribute), false)
                 && linq_Queryable_MethodNames.Contains(method.Name)
                 select method).ToList()
            );
            s_ExtensionMethods.AddRange(
                (from type in assemblies.SingleOrDefault(a => a.GetName().Name == "System.Linq.Expressions").GetTypes()
                 from method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                 where linq_Expressions_MethodNames.Contains(method.Name)
                 select method).ToList()
            );
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
               (s_OrderByDescending = s_ExtensionMethods
                .Where(m => m.Name == "OrderByDescending" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_ThenBy;

        public static MethodInfo ThenBy(Type TSource, Type TKey)
            => (s_ThenBy ??
               (s_ThenBy = s_ExtensionMethods
                .Where(m => m.Name == "ThenBy" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_ThenByDescending;

        public static MethodInfo ThenByDescending(Type TSource, Type TKey)
            => (s_ThenByDescending ??
               (s_ThenByDescending = s_ExtensionMethods
                .Where(m => m.Name == "ThenByDescending" && m.GetParameters().Count() == 2)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(TSource, TKey);

        private static MethodInfo s_Lambda;

        public static MethodInfo Lambda(Type TSource, Type TKey)
        //  {
            
        //     var temp = s_ExtensionMethods
        //         .Where(m => m.Name == "Lambda")
        //         .Select(m => new {
        //             Method = m,
        //             Parameters = m.GetParameters()
        //         })
        //         .Where(m => m.Parameters.Count() == 2)
        //         .Select(m => $"{m.Parameters[0].ParameterType.ToString()} + {m.Parameters[1].ParameterType.ToString()}")
        //         .ToList();
            
        //     return s_Lambda;
        // }
            => (s_Lambda ??
               (s_Lambda = s_ExtensionMethods
                .Where(m => m.Name == "Lambda")
                .Select(m => new {
                    Method = m,
                    Parameters = m.GetParameters()
                })
                .Where(m => m.Parameters.Count() == 2
                    && m.Parameters[0].ParameterType == typeof(Expression)
                    && m.Parameters[1].GetCustomAttribute(typeof(ParamArrayAttribute), false) != null)
                .Select(m => m.Method)
                .First()
                .GetGenericMethodDefinition()))
                .MakeGenericMethod(typeof(Func<,>).MakeGenericType(TSource, TKey));
    }
}