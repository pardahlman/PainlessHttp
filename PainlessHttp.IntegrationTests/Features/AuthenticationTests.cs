using System.Net;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class AuthenticationTests
	{
		private HttpClient _client;

		[SetUp]
		public void Setup()
		{
			var config = new HttpClientConfiguration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress
			};

			_client = new HttpClient(config);
		}

		[Test]
		public void Should_Return_Unauthorized_If_No_Credentials_Provided()
		{
			/* Setup */
			/* Test */
			var response = _client.Get<string>("api/authentication");

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}
	}
}
