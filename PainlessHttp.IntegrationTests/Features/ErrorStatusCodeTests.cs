using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Model;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class ErrorStatusCodeTests
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
		public void Should_Return_Raw_Body_Upon_Failed_Request()
		{
			/* Setup */
			const string expectedMessage = "Unable to perform request";

			/* Test */
			var response = _client.Get<Todo>("api/error-code", new { message = expectedMessage });

			/* Assert */
			Assert.That(response.RawBody, Is.EqualTo(expectedMessage));
		}
	}
}
