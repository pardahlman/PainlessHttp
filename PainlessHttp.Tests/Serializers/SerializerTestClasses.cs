using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
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

	/// <summary>
	/// Test class that has a Deserialize method, but no serialize method.
	/// This class is invalid as a Serializer generic argument.
	/// </summary>
	public class NoSerializeMethod
	{
		public static object Deserialize(string data, Type type)
		{
			return new object();
		}
	}

	/// <summary>
	/// Test class that has a Deserialize method, but no serialize method.
	/// This class is invalid as a Serializer generic argument.
	/// </summary>
	public class NoDeserializeMethod
	{
		public static string Serialize(object data)
		{
			return string.Empty;
		}
	}

	/// <summary>
	///  Test class that has the correct method signature, and
	/// therefore is accepted as generic parameter to Serializer.
	/// </summary>
	public class CorrectMethods
	{
		private static Func<object, string> _serialize;
		private static Func<string, Type, object> _deserialize;

		public static void MockMethods(Func<object, string> mockSerialize, Func<string, Type, object> mockDeserialize)
		{
			_serialize = mockSerialize;
			_deserialize = mockDeserialize;
		}
		public static string Serialize(object data)
		{
			if (_serialize == null)
			{
				return string.Empty;
			}
			return _serialize(data);
		}

		public static object Deserialize(string data, Type type)
		{
			if (_deserialize == null)
			{
				return new object();
			}
			return _deserialize(data, type);
		}
	}
}
