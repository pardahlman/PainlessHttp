using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class PostTests
	{
		private HttpClient _client;

		[SetUp]
		public void Setup()
		{
			var config = new Configuration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress
			};

			_client = new HttpClient(config);
		}

		[Test]
		public void Should_Be_Able_To_Post_Object()
		{
			/* Setup */
			var newTodo = new Todo {Description = "Write tests"};

			/* Test */
			var created = _client.Post<Todo>("/api/todos", newTodo);

			/* Assert */
			Assert.That(created.Body.Description, Is.EqualTo(newTodo.Description));
		}

		[Test]
		public async void Should_Be_Able_To_Post_Object_Async()
		{
			/* Setup */
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await _client.PostAsync<Todo>("/api/todos", newTodo);

			/* Assert */
			Assert.That(created.Body.Description, Is.EqualTo(newTodo.Description));
		}
	}
}
