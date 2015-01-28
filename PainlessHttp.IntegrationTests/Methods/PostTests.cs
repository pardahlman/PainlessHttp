using System.Net;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Data;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Http;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class PostTests
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

		[TestCase(ContentType.ApplicationJson, "json", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationJson, "xml", HttpStatusCode.UnsupportedMediaType)]
		[TestCase(ContentType.ApplicationXml, "xml", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationXml, "json", HttpStatusCode.UnsupportedMediaType)]
		public async void Should_Not_Automatically_Negotiate_Content_If_Content_Type_Is_Defined(ContentType clientSends, string endpointAccepts, HttpStatusCode expectedStatusCode)
		{
			/* Setup */
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await _client.PostAsync<string>("/api/content-type", newTodo, new {accept = endpointAccepts}, clientSends);
			
			/* Assert */
			Assert.That(created.StatusCode, Is.EqualTo(expectedStatusCode));
		}

		[TestCase(ContentType.ApplicationJson, "xml")]
		[TestCase(ContentType.ApplicationXml, "json")]
		public async void Should_Negotiate_Content_If_No_Content_Type_Is_Defined(ContentType clientSends, string endpointAccepts)
		{
			/* Setup */
			_client = new HttpClient(new HttpClientConfiguration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress,
				Advanced =
				{
					ContentType = clientSends
				}
			});
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await _client.PostAsync<string>("/api/content-type", newTodo, new { accept = endpointAccepts });

			Assert.That(created.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
	}
}
