using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.Comparers
{
    /// <summary>
    /// Generic comparer that does a simple deep equality comparison.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{T}" />
    public class GenericDeepEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(T x, T y)
        {
            var props = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var expectedValue = prop.GetValue(x, null);
                var actualValue = prop.GetValue(y, null);

                // Avoid null reference errors.
                if (null == expectedValue && null == actualValue)
                {
                    continue;
                }

                if (expectedValue.GetType() != typeof(string) && expectedValue is IEnumerable expectedEnumerable && actualValue is IEnumerable actualEnumerable)
                {
                    // Gets the generic type of the expected value ienumerable.
                    var genericType = expectedValue.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();
                    var actualGenericType = actualValue.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();

                    if (genericType != actualGenericType)
                    {
                        return false;
                    }

                    var actualEnumerator = actualEnumerable.GetEnumerator();
                    foreach (var val in expectedEnumerable)
                    {
                        actualEnumerator.MoveNext();
                        if (!val.Equals(actualEnumerator.Current))
                        {
                            return false;
                        }
                    }
                }
                else if (!expectedValue.Equals(actualValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
