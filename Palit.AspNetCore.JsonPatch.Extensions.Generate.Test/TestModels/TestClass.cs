using System;
using System.Collections.Generic;
using System.Text;

namespace Palit.AspNetCore.JsonPatch.Extensions.Generate.Test.TestModels
{
    public class TestClass
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public List<int> IntList { get; set; }
        public decimal DecimalValue { get; set; }
        public Guid GuidValue { get; set; }
    }
}
