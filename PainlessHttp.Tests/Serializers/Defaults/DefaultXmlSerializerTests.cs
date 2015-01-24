using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Moq;
using NUnit.Framework;
using PainlessHttp.Serializers.Typed;

namespace PainlessHttp.Tests.Serializers.Defaults
{
	[TestFixture]
	public class DefaultXmlSerializerTests
	{
		private DefaultXmlSerializer _serializer;
		private Mock<IDictionary<Type, XmlSerializer>> _cachedSerializers;

		[SetUp]
		public void Setup()
		{
			_cachedSerializers = new Mock<IDictionary<Type, XmlSerializer>>();
			_serializer = new DefaultXmlSerializer(_cachedSerializers.Object);
		}
		[Test]
		public void Should_Save_Serializer_If_Type_Not_Allready_In_Cache()
		{
			/* Setup */
			var obj = new SerializerTestClass { StringProp = Guid.NewGuid().ToString() };
			_cachedSerializers
				.Setup(c => c.Add(
					It.Is<Type>(type => type == typeof(SerializerTestClass)),
					It.IsAny<XmlSerializer>()))
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
			XmlSerializer serializer = null;
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
				ListProp = new List<string> { "One", "Two" },
				ObjectRef = new SerializerTestClass
				{
					StringProp = "Sub-object property"
				}
			};

			/* Test */
			var result = _serializer.Serialize(advanced);

			/* Assert */
			Assert.That(result, Is.StringStarting("<?xml version=\"1.0\"?>"), "Correct xml object opening");
			Assert.That(result, Is.StringContaining("<IntProp>3</IntProp>"));
		}

		[Test]
		public void Should_Be_Able_To_Deserialize_Objects()
		{
			/* Setup */
			const string input = "<?xml version=\"1.0\"?>"+
								"<SerializerTestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">"+
								"<StringProp>Hello, world</StringProp>"+
								"</SerializerTestClass>";
			/* Test */
			var result = _serializer.Deserialize<SerializerTestClass>(input);

			/* Assert */
			Assert.That(result.StringProp, Is.EqualTo("Hello, world"));
		}
	}
}
