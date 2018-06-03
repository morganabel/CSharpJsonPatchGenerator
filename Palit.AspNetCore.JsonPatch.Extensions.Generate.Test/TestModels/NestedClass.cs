using System;
using System.Collections.Generic;
using System.Text;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.TestModels
{
    public class NestedClass : IEquatable<NestedClass>
    {
        public string NestedId { get; set; }
        public int NestedIntValue { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is NestedClass castedObj))
                return false;

            return Equals(castedObj);
        }

        public bool Equals(NestedClass other)
        {
            if (other == null)
                return false;

            return
                NestedId == other.NestedId
                && NestedIntValue == other.NestedIntValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + NestedId.GetHashCode();
                hash = (hash * 7) + NestedIntValue.GetHashCode();
                return hash;
            }
        }
    }

    public class NestedTestClass : TestClass
    {
        public List<NestedClass> NestedClassList { get; set; }
        public NestedClass NestedClass { get; set; }
    }
}
