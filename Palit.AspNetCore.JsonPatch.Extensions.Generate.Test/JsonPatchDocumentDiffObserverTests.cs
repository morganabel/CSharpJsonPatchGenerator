using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Palit.AspNetCore.JsonPatch.Extensions.Generate;
using Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.TestModels;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.Comparers;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate.Test
{
    public class JsonPatchDocumentDiffObserverTests
    {
        [Fact]
        public void ItExists()
        {
            var instance = new TestClass();
            var observer = new JsonPatchDocumentDiffObserver<TestClass>(instance);
        }

        [Fact]
        public void ItGeneratesNoChangesWithNoDiff()
        {
            var instance = new TestClass
            {
                Id = "id",
                Message = "message",
                GuidValue = Guid.Empty,
                DecimalValue = 1.23m,
                IntList = new List<int> { 1, 2, 3 }
            };

            var observer = new JsonPatchDocumentDiffObserver<TestClass>(instance);
            var patch = observer.Generate();

            Assert.NotNull(patch);
            Assert.Empty(patch.Operations);
        }

        [Fact]
        public void ItGeneratesCorrectDiff()
        {
            var instance = new TestClass
            {
                Id = "id",
                Message = "message",
                GuidValue = Guid.Empty,
                DecimalValue = 1.23m,
                IntList = new List<int> { 1, 2, 3 }
            };

            var observer = new JsonPatchDocumentDiffObserver<TestClass>(instance);
            instance.Id = "new-id";
            instance.Message = "new-message";
            instance.GuidValue = Guid.Parse("64362fd9-a24a-4b4b-97cd-8ba9df24a1b5");
            instance.DecimalValue = 1.89m;
            instance.IntList = new List<int> { 3, 2, 1 };

            var patch = observer.Generate();

            Assert.NotNull(patch);
            Assert.Equal(5, patch.Operations.Count);
        }
    }
}
