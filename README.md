# JsonPatch Generate Extension

Builds on Microsoft.AspNetCore.JsonPatch to generate JSON patch documents by comparing or observing changes to C# objects.

  - Generate JSON patch by comparing two objects
  - Watch an instance of an object for changes and generate a JSON patch representing those changes

## Installation

From the package manager console:

	PM> Install-Package Palit.AspNetCore.JsonPatch.Extensions.Generate

or by simply searching for `Palit.AspNetCore.JsonPatch.Extensions.Generate` in the package manager UI.

## How it's used

Use this library if you want to generate JSON patch documents from C#. I couldn't find a library for this already and it is not built in so I wrote one myself. It can generate the patch document by comparing two objects or by watching changes to an instance.

## Gotchas

This library is currently fairly simple. It will not necessarily generate optimal JSON patch documents. Instead, it generates patch documents containing Add, Remove and Replace commands exclusively. In the future it would be nice to work in all the different operations in a more optimal way.

Internally, this library relies on JSON.net serializer pretty heavily. You can pass in custom serializer settings, but definitely do thorough testing with your unique circumstances. The current tests are pretty simple and do not cover complex situations (like deep nesting).

## How-To generate a JSON patch by comparison

```csharp
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
```

Alternatively, you can pass in your own custom JsonSerializer like so:
```csharp
var jsonSerializer = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore };
var patch = generator.Generate(original, modified, jsonSerializer);
```

## How to generate a patch document with an observer

```csharp
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

// Point in time snapshot of the changes.
var patch = observer.Generate();

Assert.NotNull(patch);
Assert.Equal(5, patch.Operations.Count);
```

License
----

MIT

