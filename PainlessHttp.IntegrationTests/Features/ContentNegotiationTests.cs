using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Http;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class ContentNegotiationTests
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

		[TestCase(ContentType.ApplicationJson, "json", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationJson, "xml", HttpStatusCode.UnsupportedMediaType)]
		[TestCase(ContentType.ApplicationXml, "xml", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationXml, "json", HttpStatusCode.UnsupportedMediaType)]
		public async void Should_Not_Automatically_Negotiate_Content_If_Content_Type_Is_Defined(ContentType clientSends, string endpointAccepts, HttpStatusCode expectedStatusCode)
		{
			/* Setup */
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await _client.PostAsync<string>("/api/content-type", newTodo, new { accept = endpointAccepts }, clientSends);

			/* Assert */
			Assert.That(created.StatusCode, Is.EqualTo(expectedStatusCode));
		}

		[TestCase(ContentType.ApplicationJson, "xml")]
		[TestCase(ContentType.ApplicationXml, "json")]
		public async void Should_Negotiate_Content_If_No_Content_Type_Is_Defined(ContentType clientSends, string endpointAccepts)
		{
			/* Setup */
			_client = new HttpClient(new Configuration
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
