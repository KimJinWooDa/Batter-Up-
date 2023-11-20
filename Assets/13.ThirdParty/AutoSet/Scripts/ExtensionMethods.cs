using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoSet.Utils
{
    public static class ExtensionMethods
    {
        public static IEnumerable<(A first, B second)> Zip<A, B>(this IEnumerable<A> enumA, IEnumerable<B> enumB)
        {
            var a = enumA.GetEnumerator();
            var b = enumB.GetEnumerator();
            while (a.MoveNext() && b.MoveNext())
            {
                yield return (a.Current, b.Current);
            }
        }

        public static IEnumerable<KeyValuePair<int, T>> Enumerate<T>(this IEnumerable<T> values)
        {
            var i = 0;
            foreach (var value in values)
            {
                yield return new(i, value);
                i += 1;
            }
        }

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        public static string IfNullOrEmpty(this string str, string other) => str.IsNullOrEmpty() ? other : str;
     
        public static void Deconstruct<TKey, TElement>(
            this IGrouping<TKey, TElement> grouping,
            out TKey key,
            out IEnumerable<TElement> values) =>
            (key, values) = (grouping.Key, grouping);
        
        public static IEnumerable<T> WhereNonNull<T>(this IEnumerable<T> values) where T : class
        {
            foreach (var value in values)
                if (value != null)
                    yield return value;
        }
        
        public static object GetNamedArgument(this CustomAttributeData attr, string memberName)
        {
            return attr.NamedArguments.
                FirstOrDefault(arg => arg.MemberName == memberName).
                TypedValue.Value;
        }

#if UNITY_EDITOR
        public static IEnumerable<UnityEditor.SerializedProperty> GetSerializedProperties(this UnityEditor.SerializedObject serializedObject)
        {
            var property = serializedObject.GetIterator();
            while (true)
            {
                yield return property;

                if (!property.Next(true))
                    break;
            }
        }

        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static FieldInfo GetField(this Type type, UnityEditor.SerializedProperty property)
        {
            var propertyName = property.name;
            while (type != null)
            {
                var field = type.GetField(propertyName, BINDING_FLAGS);
                if (field != null)
                    return field;
                type = type.BaseType;
            }
            return null;
        }
#endif
    }
}
