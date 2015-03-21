using System.Net;
using NUnit.Framework;
using PainlessHttp.Cache;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class ModifiedSinceTests
	{
		private HttpClient _client;
		private Configuration _config;

		[SetUp]
		public void Setup()
		{
			_config = new Configuration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress,
			};

			_client = new HttpClient(_config);
		}

		[Test]
		public async void Should_Use_In_Memory_Content_Cache()
		{
			/* Setup */
			_config.Advanced.ModifiedSinceCache = new InMemoryCache();
			var firstResponse = await _client.GetAsync<Todo>("api/todos/1");

			/* Test */
			var secondResponse = await _client.GetAsync<Todo>("api/todos/1");
			
			/* Assert */
			Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotModified));
			Assert.That(firstResponse.Body.Id, Is.EqualTo(secondResponse.Body.Id));
		}
	}
}
