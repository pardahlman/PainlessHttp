using System;
using Newtonsoft.Json;
using NUnit.Framework;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.Serializer.JsonNet.Tests
{
	[TestFixture]
	public class Class1
	{
		private string _payloadAsString;
		private Todo _payload;

		[SetUp]
		public void Setup()
		{
			_payload = new Todo
			{
				Description = "Write a new JsonNet serialzer",
				Id = 1,
				IsCompleted = true,
				UpdateDate = new DateTime(2015,03,15)
			};

			_payloadAsString = JsonConvert.SerializeObject(_payload);
		}

		[Test]
		public void Should_Serialize_Correctly_With_Default_Settings()
		{
			/* Setup */
			var serializer = new PainlessJsonNet();

			/* Test */
			var result = serializer.Serialize(_payload);

			/* Assert */
			Assert.That(result, Contains.Substring("{\"Id\":1,\"Description\":\"Write a new JsonNet serialzer\",\"IsCompleted\":true,\"UpdateDate\":\"2015-03-15T00:00:00\"}"));
		}

		[Test]
		public void Should_Deserialize_Correctly_With_Default_Settings()
		{
			/* Setup */
			var serializer = new PainlessJsonNet();

			/* Test */
			var result = serializer.Deserialize<Todo>(_payloadAsString);

			/* Assert */
			Assert.That(result.Description, Is.EqualTo(_payload.Description));
			Assert.That(result.Id, Is.EqualTo(_payload.Id));
			Assert.That(result.IsCompleted, Is.EqualTo(_payload.IsCompleted));
		}

		[Test]
		public void Should_Use_Overrides_If_Supplied()
		{
			/* Setup */
			var deserializedCalled = false;
			var serializedCalled = false;
			Func<object, string> serialize = o =>
			{
				serializedCalled = true;
				return string.Empty;
			};
			Func<string, Type, object> deserialize = (s, type) =>
			{
				deserializedCalled = true;
				return null;
			};
			var serializer = new PainlessJsonNet(serialize, deserialize);

			/* Test */
			serializer.Serialize(_payload);
			serializer.Deserialize<Todo>(_payloadAsString);

			/* Assert */
			Assert.That(deserializedCalled, Is.True);
			Assert.That(serializedCalled, Is.True);
		}
	}
}
