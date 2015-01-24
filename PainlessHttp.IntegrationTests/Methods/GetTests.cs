using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
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
			var config = new HttpClientConfiguration
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


	}
}
