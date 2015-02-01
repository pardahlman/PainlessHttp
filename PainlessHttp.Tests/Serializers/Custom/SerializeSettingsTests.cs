using System;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Tests.Serializers.Custom
{
	[TestFixture]
	public class SerializeSettingsTests
	{
		[Test]
		public void Should_Set_Correct_Content_Types()
		{
			/* Setup */
			var expected = new[] {ContentType.ApplicationJson, ContentType.TextCsv};
			
			/* Test */
			var result = SerializerBulider
					.For(expected)
						.Serialize(o => string.Empty)
						.Deserialize((s, t) => new object());

			/* Assert */
			Assert.That(result.ContentType, Is.EqualTo(expected));
		}

		[Test]
		public void Should_Call_Provided_Serialize_Func_Upon_Serialize()
		{
			/* Setup */
			var serializedCalled = false;
			var serializer = SerializerBulider
				.For(ContentType.ApplicationJson)
				.Serialize(o =>
							{
								serializedCalled = true;
								return string.Empty;
							})
				.Deserialize((s, t) => new object());
			
			/* Test */
			serializer.Serialize(new object());

			/* Assert */
			Assert.That(serializedCalled, Is.True);
		}

		[Test]
		public void Should_Call_Provided_Deserialize_Func_Upon_Deserialization()
		{
			/* Setup */
			var deserializedCalled = false;
			var serializer = SerializerBulider
				.For(ContentType.ApplicationJson)
				.Serialize(o => string.Empty)
				.Deserialize((s, t) =>
							{
								deserializedCalled = true;
								return new object();
							});

			/* Test */
			serializer.Deserialize<object>(string.Empty);

			/* Assert */
			Assert.That(deserializedCalled, Is.True);
		}
	}
}
