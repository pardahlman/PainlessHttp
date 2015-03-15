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
		[TestCase(ContentType.ApplicationJson, "json", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationJson, "xml", HttpStatusCode.UnsupportedMediaType)]
		[TestCase(ContentType.ApplicationXml, "xml", HttpStatusCode.OK)]
		[TestCase(ContentType.ApplicationXml, "json", HttpStatusCode.UnsupportedMediaType)]
		public async void Should_Not_Automatically_Negotiate_Content_If_Content_Type_Is_Defined(ContentType clientSends, string endpointAccepts, HttpStatusCode expectedStatusCode)
		{
			/* Setup */
			var config = new Configuration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress,
				Advanced =
				{
					ContentNegotiation = false
				}
			};
			var client = new HttpClient(config);
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await client.PostAsync<string>("/api/content-type", newTodo, new { accept = endpointAccepts }, clientSends);

			/* Assert */
			Assert.That(created.StatusCode, Is.EqualTo(expectedStatusCode));
		}

		[TestCase(ContentType.ApplicationJson, "xml")]
		[TestCase(ContentType.ApplicationXml, "json")]
		public async void Should_Negotiate_Content_As_Default_Behaviour(ContentType clientSends, string endpointAccepts)
		{
			/* Setup */
			var client = new HttpClient(WebApiSetupFixture.BaseAddress);
			var newTodo = new Todo { Description = "Write tests" };

			/* Test */
			var created = await client.PostAsync<string>("/api/content-type", newTodo, new { accept = endpointAccepts }, clientSends);

			Assert.That(created.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}
	}
}
