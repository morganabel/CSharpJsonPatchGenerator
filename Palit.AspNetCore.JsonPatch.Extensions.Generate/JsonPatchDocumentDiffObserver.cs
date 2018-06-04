using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate
{
    /// <summary>
    /// Observer that tracks changes to an instance and can generate a JsonPatchDocument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonPatchDocumentDiffObserver<T> where T : class
    {

        /// <summary>
        /// The watched instance
        /// </summary>
        private readonly T _watchedInstance;

        /// <summary>
        /// The original clone
        /// </summary>
        private readonly T _originalClone;

        /// <summary>
        /// The generator
        /// </summary>
        private readonly JsonPatchDocumentGenerator _generator = new JsonPatchDocumentGenerator();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPatchDocumentDiffObserver{T}"/> class.
        /// </summary>
        /// <param name="watchedInstance">The watchedInstance<see cref="T"/></param>
        public JsonPatchDocumentDiffObserver(T watchedInstance)
        {
            _watchedInstance = watchedInstance;
            if (watchedInstance == null)
            {
                _originalClone = default(T);
            }
            else
            {
                _originalClone = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(_watchedInstance), new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            }
        }

        /// <summary>
        /// Generates the patch document.
        /// </summary>
        /// <returns>The <see cref="JsonPatchDocument"/></returns>
        public JsonPatchDocument Generate()
        {
            return _generator.Generate<T>(_originalClone, _watchedInstance);
        }

        /// <summary>
        /// Generates the patch document using the specified json serializer.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <returns>The <see cref="JsonPatchDocument"/></returns>
        public JsonPatchDocument Generate(JsonSerializer jsonSerializer)
        {
            return _generator.Generate<T>(_originalClone, _watchedInstance, jsonSerializer);
        }
    }
}
