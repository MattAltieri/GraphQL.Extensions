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

        private static MethodInfo s_StringFormat;

        public static MethodInfo StringFormat()
            => (s_StringFormat ??
               (s_StringFormat = typeof(string).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.Name == "Format")
                    .Select(m => new {
                        Method = m,
                        Parameters = m.GetParameters()
                    })
                    .Where(m => m.Parameters.Count() == 2
                        && m.Parameters[0].ParameterType == typeof(string)
                        && m.Parameters[1].GetCustomAttribute(typeof(ParamArrayAttribute), false) != null)
                    .Select(m => m.Method)
                    .First()));

        private static Dictionary<Type, MethodInfo> s_ToStringMethods = new Dictionary<Type, MethodInfo>();

        public static MethodInfo ToString(Type type) {

            if (!PrimitiveTypes.Contains(type))
                throw new ArgumentNullException(type.Name);

            if (!s_ToStringMethods.ContainsKey(type)) {
                s_ToStringMethods.Add(
                    type,
                    type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.Name == "ToString" && m.GetParameters().Count() == 0)
                        .First()
                );
            }
            
            return s_ToStringMethods[type];
        }

        private static PropertyInfo s_DateTimeTicks;

        public static PropertyInfo DateTimeTicks()
            => (s_DateTimeTicks ??
               (s_DateTimeTicks = typeof(DateTime).GetProperty("Ticks")));

        private static Dictionary<Type, PropertyInfo> s_NullableHasValueProperties = new Dictionary<Type, PropertyInfo>();

        public static PropertyInfo NullableHasValue(Type type) {

            if (!NullableTypes.Contains(type))
                throw new ArgumentNullException(type.Name);

            if (!s_NullableHasValueProperties.ContainsKey(type)) {
                s_NullableHasValueProperties.Add(
                    type,
                    type.GetProperty("HasValue")
                );
            }

            return s_NullableHasValueProperties[type];
        }

        private static Dictionary<Type, PropertyInfo> s_NullableValueProperties = new Dictionary<Type, PropertyInfo>();

        public static PropertyInfo NullableValue(Type type) {

            if (!NullableTypes.Contains(type))
                throw new ArgumentNullException(type.Name);

            if (!s_NullableValueProperties.ContainsKey(type)) {
                s_NullableValueProperties.Add(
                    type,
                    type.GetProperty("Value")
                );
            }

            return s_NullableValueProperties[type];
        }

        private static MethodInfo s_StringCompareTo;
        
        public static MethodInfo StringCompareTo()
            => (s_StringCompareTo ??
               (s_StringCompareTo = typeof(string).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "CompareTo")
                    .Where(m => m.GetParameters()[0].ParameterType == typeof(string))
                    .First()));
                    
        private static MethodInfo s_CharCompareTo;

        public static MethodInfo CharCompareTo()
            => (s_CharCompareTo ??
               (s_CharCompareTo = typeof(char).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.Name == "CompareTo")
                    .Where(m => m.GetParameters()[0].ParameterType == typeof(char))
                    .First()));

        private static MethodInfo s_IEnumerableContains;

        public static MethodInfo IEnumerableContains(Type elementType)
            => (s_IEnumerableContains ??
               (s_IEnumerableContains =
                    (from type in AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name == "System.Linq").GetTypes()
                     where type.IsSealed
                     && !type.IsGenericType
                     && !type.IsNested
                     from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     where method.IsDefined(typeof(ExtensionAttribute), false)
                     && method.Name == "Contains"
                     && method.GetParameters().Count() == 2
                     select method)
                     .First().GetGenericMethodDefinition()))
                     .MakeGenericMethod(elementType);

        private static MethodInfo s_StringContains;

        public static MethodInfo StringContains()
            => (s_StringContains ??
               (s_StringContains = typeof(string).GetMethod("Contains", new[] { typeof(string) })));

        private static MethodInfo s_StringStartsWith;

        public static MethodInfo StringStartsWith()
            => (s_StringStartsWith ??
               (s_StringStartsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })));

        private static MethodInfo s_StringEndsWith;

        public static MethodInfo StringEndsWith()
            => (s_StringEndsWith ??
               (s_StringEndsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })));

        private static MethodInfo s_StringIsNullOrEmpty;

        public static MethodInfo StringIsNullOrEmpty()
            => (s_StringIsNullOrEmpty ??
               (s_StringIsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty", BindingFlags.Public | BindingFlags.Static,
                    null, new[] { typeof(string) }, null)));

        private static IEnumerable<Type> PrimitiveTypes
            => new List<Type> {
                typeof(char),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(decimal),
                typeof(float),
                typeof(double),
                typeof(bool)
            };

        private static IEnumerable<Type> NullableTypes
            => new List<Type> {
                typeof(char?),
                typeof(short?),
                typeof(int?),
                typeof(long?),
                typeof(decimal?),
                typeof(float?),
                typeof(double?),
                typeof(bool?),
                typeof(DateTime?),
            };
    }
}