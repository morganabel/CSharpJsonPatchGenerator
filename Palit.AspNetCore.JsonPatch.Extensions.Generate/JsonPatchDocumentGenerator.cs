using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate
{
    /// <summary>
    /// Defines the <see cref="JsonPatchDocumentGenerator" />
    /// </summary>
    public class JsonPatchDocumentGenerator
    {
        /// <summary>
        /// Generates a JsonPatchDocument by comparing two objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">The original object<see cref="T" /></param>
        /// <param name="b">The modified object<see cref="T" /></param>
        /// <returns>
        /// The <see cref="JsonPatchDocument" />
        /// </returns>
        public JsonPatchDocument Generate<T>(T a, T b) where T : class
        {
            return GeneratePrivate(a, b);
        }

        /// <summary>
        /// Generates a JsonPatchDocument by comparing two objects using the input JsonSerializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">The original object<see cref="T" /></param>
        /// <param name="b">The modified object<see cref="T" /></param>
        /// <param name="jsonSerializer">The jsonSerializer<see cref="JsonSerializer"/></param>
        /// <returns>
        /// The <see cref="JsonPatchDocument" />
        /// </returns>
        public JsonPatchDocument Generate<T>(T a, T b, JsonSerializer jsonSerializer) where T : class
        {
            return GeneratePrivate(a, b, jsonSerializer);
        }

        /// <summary>
        /// Generates a JsonPatchDocument by comparing two objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">The original object<see cref="T" /></param>
        /// <param name="b">The modified object<see cref="T" /></param>
        /// <param name="jsonSerializer">The jsonSerializer<see cref="JsonSerializer"/></param>
        /// <returns>The <see cref="JsonPatchDocument"/></returns>
        private JsonPatchDocument GeneratePrivate<T>(T a, T b, JsonSerializer jsonSerializer = null) where T : class
        {
            var output = new JsonPatchDocument();
            if (ReferenceEquals(a, b))
            {
                return output;
            }

            if (null == jsonSerializer)
            {
                jsonSerializer = JsonSerializer.CreateDefault();
            }

            var originalJson = JObject.FromObject(a, jsonSerializer);
            var modifiedJson = JObject.FromObject(b, jsonSerializer);

            FillJsonPatchValues(originalJson, modifiedJson, output);

            return output;
        }

        /// <summary>
        /// Fills the json patch values.
        /// </summary>
        /// <param name="originalJson">The original json.</param>
        /// <param name="modifiedJson">The modified json.</param>
        /// <param name="patch">The patch.</param>
        /// <param name="currentPath">The current path.</param>
        private static void FillJsonPatchValues(JObject originalJson, JObject modifiedJson, JsonPatchDocument patch, string currentPath = "/")
        {
            var originalPropertyNames = new HashSet<string>(originalJson.Properties().Select(p => p.Name));
            var modifiedPropertyNames = new HashSet<string>(modifiedJson.Properties().Select(p => p.Name));

            // Remove properties not in modified.
            foreach (var propName in originalPropertyNames.Except(modifiedPropertyNames))
            {
                var prop = originalJson.Property(propName);
                patch.Remove(currentPath + prop.Name);
            }

            // Add properties not in original
            foreach (var propName in modifiedPropertyNames.Except(originalPropertyNames))
            {
                var prop = modifiedJson.Property(propName);
                patch.Add(currentPath + prop.Name, prop.Value);
            }

            // Modify properties that exist in both.
            foreach (var propName in originalPropertyNames.Intersect(modifiedPropertyNames))
            {
                var originalProp = originalJson.Property(propName);
                var modifiedProp = modifiedJson.Property(propName);

                if (originalProp.Value.Type != modifiedProp.Value.Type)
                {
                    patch.Replace(currentPath + propName, modifiedProp.Value);
                }
                else if (!string.Equals(originalProp.Value.ToString(Formatting.None), modifiedProp.Value.ToString(Formatting.None)))
                {
                    if (originalProp.Value.Type == JTokenType.Object)
                    {
                        // Recursively fill nested objects.
                        FillJsonPatchValues(originalProp.Value as JObject, modifiedProp.Value as JObject, patch, $"{currentPath}{propName}/");
                    }
                    else
                    {
                        // Simple Replace otherwise to make patches idempotent.
                        patch.Replace(currentPath + propName, modifiedProp.Value);
                    }
                }
            }
        }
    }
}
