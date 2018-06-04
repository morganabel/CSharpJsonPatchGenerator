using System;
using System.Collections.Generic;
using Xunit;
using Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.TestModels;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.Comparers;
using Newtonsoft.Json;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate.Test
{
    public class JsonPatchDocumentGeneratorTests
    {
        [Fact]
        public void ItExists()
        {
            var generator = new JsonPatchDocumentGenerator();
        }

        [Fact]
        public void ItGetsNoDiffWithSameInstance()
        {
            var testInstance = new TestClass() { Id = "id" };
            var reference = testInstance;
            reference.Id = "new id";

            var generator = new JsonPatchDocumentGenerator();
            var patch = generator.Generate<TestClass>(testInstance, reference);

            Assert.NotNull(patch);
            Assert.Empty(patch.Operations);
        }

        [Fact]
        public void ItGetsNoDiffWithIdenticalObjects()
        {
            var original = new TestClass()
            {
                Id = "id",
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 }
            };
            var modified = new TestClass()
            {
                Id = "id",
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 }
            };

            var generator = new JsonPatchDocumentGenerator();
            var patch = generator.Generate(original, modified);

            Assert.NotNull(patch);
            Assert.Empty(patch.Operations);
        }

        [Fact]
        public void ItGeneratesSimpleCorrectPatchDocument()
        {
            var original = new TestClass()
            {
                Id = "id",
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 }
            };
            var modified = new TestClass()
            {
                Id = "new-id",
                Message = null,
                DecimalValue = 1.68m,
                GuidValue = Guid.Parse("64362fd9-a24a-4b4b-97cd-8ba9df24a1b5"),
                IntList = new List<int>() { 1, 3, 2 }
            };

            var generator = new JsonPatchDocumentGenerator();
            var patch = generator.Generate(original, modified);

            // Modify original with patch.
            patch.ApplyTo(original);

            Assert.NotNull(patch);
            Assert.Equal(5, patch.Operations.Count);
            Assert.Equal(original, modified, new GenericDeepEqualityComparer<TestClass>());
        }

        [Fact]
        public void ItGeneratesUsingCustomJsonSerializer()
        {
            var original = new TestClass()
            {
                Id = null,
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 }
            };
            var modified = new TestClass()
            {
                Id = "new-id",
                Message = null,
                DecimalValue = 1.68m,
                GuidValue = Guid.Parse("64362fd9-a24a-4b4b-97cd-8ba9df24a1b5"),
                IntList = new List<int>() { 1, 3, 2 }
            };

            var generator = new JsonPatchDocumentGenerator();
            var jsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
            var patch = generator.Generate(original, modified, jsonSerializer);

            // Modify original with patch.
            patch.ApplyTo(original);

            Assert.NotNull(patch);
            Assert.Equal(5, patch.Operations.Count);
            Assert.Contains(patch.Operations, op => op.OperationType == OperationType.Add);
            Assert.Contains(patch.Operations, op => op.OperationType == OperationType.Remove);
            Assert.Equal(original, modified, new GenericDeepEqualityComparer<TestClass>());
        }

        [Fact]
        public void ItGeneratesInheritedCorrectPatchDocument()
        {
            var original = new InheritedClass()
            {
                Id = "id",
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 },
                ExtraIntValue = 23
            };
            var modified = new InheritedClass()
            {
                Id = "new-id",
                Message = null,
                DecimalValue = 1.68m,
                GuidValue = Guid.Parse("64362fd9-a24a-4b4b-97cd-8ba9df24a1b5"),
                IntList = new List<int>() { 1, 3, 2 },
                ExtraIntValue = 34
            };

            var generator = new JsonPatchDocumentGenerator();
            var patch = generator.Generate(original, modified);

            // Modify original with patch.
            patch.ApplyTo(original);

            Assert.NotNull(patch);
            Assert.Equal(6, patch.Operations.Count);
            Assert.Equal(original, modified, new GenericDeepEqualityComparer<InheritedClass>());
        }

        [Fact]
        public void ItGeneratesNestedCorrectPatchDocument()
        {
            var original = new NestedTestClass
            {
                Id = "id",
                Message = "message",
                DecimalValue = 1.43m,
                GuidValue = Guid.Empty,
                IntList = new List<int>() { 1, 2, 3 },
                NestedClass = new NestedClass { NestedId = "nested-id", NestedIntValue = 465 },
                NestedClassList = new List<NestedClass>
                {
                    new NestedClass { NestedId = "1", NestedIntValue = 1 },
                    new NestedClass { NestedId = "2", NestedIntValue = 2 },
                    new NestedClass { NestedId = "3", NestedIntValue = 3 }
                }
            };
            var modified = new NestedTestClass
            {
                Id = "new-id",
                Message = "new-message",
                DecimalValue = 1.40m,
                GuidValue = Guid.Parse("64362fd9-a24a-4b4b-97cd-8ba9df24a1b5"),
                IntList = new List<int>() { 1, 2, 3, 4 },
                NestedClass = new NestedClass { NestedId = "new-nested-id", NestedIntValue = 465 },
                NestedClassList = new List<NestedClass>
                {
                    new NestedClass { NestedId = "1", NestedIntValue = 1 },
                    new NestedClass { NestedId = "2", NestedIntValue = 2 },
                    new NestedClass { NestedId = "345", NestedIntValue = 345 }
                }
            };

            var generator = new JsonPatchDocumentGenerator();
            var patch = generator.Generate(original, modified);

            // Modify original with patch.
            patch.ApplyTo(original);

            Assert.NotNull(patch);
            Assert.Equal(7, patch.Operations.Count);
            Assert.All(patch.Operations, op => Assert.Equal(OperationType.Replace, op.OperationType));
            Assert.Contains(patch.Operations, op => op.path.Equals("/NestedClass/NestedId"));
            Assert.Equal(original, modified, new GenericDeepEqualityComparer<NestedTestClass>());
        }
    }
}
