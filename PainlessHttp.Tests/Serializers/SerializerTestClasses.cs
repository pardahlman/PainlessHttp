using System.Collections.Generic;
using NUnit.Framework;

namespace PainlessHttp.Tests.Serializers
{
	public class SerializerTestClass
	{
		public string StringProp { get; set; }
	}

	public class AdvancedTestClass
	{
		public int IntProp { get; set; }
		public List<string> ListProp { get; set; }
		public SerializerTestClass ObjectRef { get; set; }
	}
}
