using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Moq;
using NUnit.Framework;
using PainlessHttp.Serializers.Typed;

namespace PainlessHttp.Tests.Serializers.Defaults
{
	[TestFixture]
	public class DefaultJsonSerializerTests
	{
		private DefaultJsonSerializer _serializer;
		private Mock<IDictionary<Type, DataContractJsonSerializer>> _cachedSerializers;

		[SetUp]
		public void Setup()
		{
			_cachedSerializers = new Mock<IDictionary<Type, DataContractJsonSerializer>>();
			_serializer = new DefaultJsonSerializer(_cachedSerializers.Object);
		}

		[Test]
		public void Should_Save_Serializer_If_Type_Not_Allready_In_Cache()
		{
			/* Setup */
			var obj = new SerializerTestClass { StringProp = Guid.NewGuid().ToString() };
			_cachedSerializers
				.Setup(c => c.Add(
					It.Is<Type>(type => type == typeof(SerializerTestClass)),
					It.IsAny<DataContractJsonSerializer>()))
				.Verifiable();

			/* Test */
			_serializer.Serialize(obj);

			/* Assert */
			_cachedSerializers.VerifyAll();
		}

		[Test]
		public void Should_Look_For_Existing_Serializer_Before_Creating_New()
		{
			/* Setup */
			var obj = new SerializerTestClass { StringProp = Guid.NewGuid().ToString() };
			DataContractJsonSerializer serializer = null;
			_cachedSerializers
				.Setup(c => c.TryGetValue(
					It.Is<Type>(type => type == typeof(SerializerTestClass)),
					out serializer))
				.Verifiable();

			/* Test */
			_serializer.Serialize(obj);

			/* Assert */
			_cachedSerializers.VerifyAll();
		}

		[Test]
		public void Should_Be_Able_To_Serialize_Advanced_Objects()
		{
			/* Setup */
			var advanced = new AdvancedTestClass
			{
				IntProp = 3,
				ListProp = new List<string> { "One", "Two"},
				ObjectRef = new SerializerTestClass
				{
					StringProp = "Sub-object property"
				}
			};

			/* Test */
			var result = _serializer.Serialize(advanced);

			/* Assert */
			Assert.That(result, Is.StringStarting("{"), "Correct json object opening");
			Assert.That(result, Is.StringEnding("}"), "Correct json closing");
			Assert.That(result, Is.StringContaining("\"IntProp\":3"));
			Assert.That(result, Is.StringContaining("\"ObjectRef\":{\"StringProp\":\"Sub-object property\"}"));
			Assert.That(result, Is.StringContaining("\"ListProp\":[\"One\",\"Two\"]"));
		}
	}
}
