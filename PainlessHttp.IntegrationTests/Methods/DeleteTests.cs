using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Data;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class DeleteTests
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
		public void Should_Be_Able_To_Perform_Delete()
		{
			/* Setup */
			var todo = new Todo { Description = "Do the dishes" };
			var saved = _repo.Add(todo);

			/* Test */
			var response = _client.Delete<string>(string.Format("api/todos/{0}", saved.Id));

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(_repo.Get(saved.Id), Is.Null, "Todo should have been removed");
		}

		[Test]
		public async void Should_Be_Able_To_Perform_Delete_Async()
		{
			/* Setup */
			var todo = new Todo { Description = "Do the dishes" };
			var saved = _repo.Add(todo);

			/* Test */
			var response = await _client.DeleteAsync<string>(string.Format("api/todos/{0}", saved.Id));

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(_repo.Get(saved.Id), Is.Null, "Todo should have been removed");
		}
	}
}
