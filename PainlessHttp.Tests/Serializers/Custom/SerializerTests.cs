using System;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Tests.Serializers.Custom
{
	[TestFixture]
	public class SerializerTests
	{
		[Test]
		public void Should_Throw_Exception_If_Generic_Type_Does_Not_Have_Serialize_Method()
		{
			/* Test, Setup & Assert */
			Assert.Throws<MissingMethodException>(() => new Serializer<NoSerializeMethod>(ContentType.Unknown));
		}

		[Test]
		public void Should_Throw_Exception_If_Generic_Type_Does_Not_Have_Deserialize_Method()
		{
			/* Test, Setup & Assert */
			Assert.Throws<MissingMethodException>(() => new Serializer<NoDeserializeMethod>(ContentType.Unknown));
		}

		[Test]
		public void Should_Throw_Exception_If_Generic_Type_Has_Correct_Methods()
		{
			/* Test, Setup & Assert */
			Assert.DoesNotThrow(() => new Serializer<CorrectMethods>(ContentType.Unknown));
		}

		[Test]
		public void Should_Call_Correct_Methods_On_Serialize()
		{
			/* Setup */
			var serializedCalled = false;

			CorrectMethods.MockMethods(
				serializeMethod =>
				{
					serializedCalled = true;
					return string.Empty;
				},
				null
			);

			var serializer = new Serializer<CorrectMethods>(ContentType.Unknown);

			/* Test */
			serializer.Serialize(string.Empty);

			/* Assert */
			Assert.That(serializedCalled, Is.True);
		}

		[Test]
		public void Should_Call_Correct_Methods_On_Deserialize()
		{
			/* Setup */
			var deserializedCalled = false;

			CorrectMethods.MockMethods(
				null,
				(deserializeMethod, type) =>
				{
					deserializedCalled = true;
					return new object();
				}
			);

			var serializer = new Serializer<CorrectMethods>(ContentType.Unknown);

			/* Test */
			serializer.Deserialize<object>(string.Empty);

			/* Assert */
			Assert.That(deserializedCalled, Is.True);
		}
	}
}
