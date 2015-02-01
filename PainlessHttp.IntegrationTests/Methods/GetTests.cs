using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Data;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class GetTests
	{
		private HttpClient _client;
		private InMemoryTodoRepo _repo;

		[SetUp]
		public void Setup()
		{
			var config = new Configuration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress
			};

			_client = new HttpClient(config);
			_repo = InMemoryTodoRepo.Instance;
		}

		[Test]
		public void Should_Be_Able_To_Get_Typed_Object()
		{
			/* Setup */
			var expected = _repo.Get(1);

			/* Test */
			var result = _client.Get<Todo>("api/todos/1");

			/* Assert */
			Assert.That(result.Body.Id, Is.EqualTo(expected.Id));
			Assert.That(result.Body.Description, Is.EqualTo(expected.Description));
			Assert.That(result.Body.IsCompleted, Is.EqualTo(expected.IsCompleted));
		}

		[Test]
		public async void Should_Be_Able_To_Get_Typed_Object_Async()
		{
			/* Setup */
			var expected = _repo.Get(1);

			/* Test */
			var result = await _client.GetAsync<Todo>("api/todos/1");

			/* Assert */
			Assert.That(result.Body.Id, Is.EqualTo(expected.Id));
			Assert.That(result.Body.Description, Is.EqualTo(expected.Description));
			Assert.That(result.Body.IsCompleted, Is.EqualTo(expected.IsCompleted));
		}

		[Test]
		public void Should_Be_Able_To_Get_Raw_Payload()
		{
			/* Setup */

			/* Test */
			var result = _client.Get<string>("api/todos/1");

			/* Assert */
			Assert.That(result.Body, Is.StringStarting("{"));
			Assert.That(result.Body, Is.StringContaining("\"Id\":1"));
			Assert.That(result.Body, Is.StringEnding("}"));
		}

		[Test]
		public void Should_Be_Able_To_Affect_Response_With_Query()
		{
			/* Setup */
			var allTodos = _repo.GetAll();

			/* Test */
			var result = _client.Get<List<Todo>>("api/todos", new {includeCompeted = false});

			/* Assert */
			Assert.That(allTodos.Any(t => t.IsCompleted), Is.True, "There should at leaste be one competed todo in order to be sure this works");
			Assert.That(result.Body, Is.Not.Empty, "There must be at leaste one todo in the response");
			Assert.That(result.Body.Any(t => t.IsCompleted), Is.False, "All completed todos should be filtered");
		}

		[TestCase("json")]
		[TestCase("xml")]
		public void Should_Be_Able_To_Parse_Json_And_Xml(string format)
		{
			/* Setup */
			/* Test */
			/* Assert */
			Assert.DoesNotThrow(() =>_client.Get<Todo>("api/content-type", new {prefered = format}));
		}
	}
}
