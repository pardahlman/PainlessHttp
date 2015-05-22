using System;
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

		[Test]
		public void Should_Throw_Exception_If_Response_Is_In_Acepted_Range_But_Not_Serializable()
		{
			/* Setup */
			const string expectedMessage = "Request successful";

			/* Test */
			/* Assert */
			Assert.Throws<AggregateException>(() => _client.Get<Todo>("api/error-code", new { message = expectedMessage, statusCode = 200 }));
		}
	}
}
