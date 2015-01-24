using System.Net;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Data;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class PutTests
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
		public void Should_Be_Able_To_Put_Object()
		{
			/* Setup */
			const string newDescription = "This is the new description";
			var existingTodo = _repo.Get(2);
			existingTodo.Description = newDescription;

			/* Test */
			var result = _client.Put<string>("api/todos", existingTodo);
			
			/* Assert */
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
			Assert.That(_repo.Get(2).Description, Is.EqualTo(newDescription));
		}
	}
}
