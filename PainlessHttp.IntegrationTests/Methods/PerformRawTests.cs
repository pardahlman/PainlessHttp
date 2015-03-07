using System.Net;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Http;

namespace PainlessHttp.IntegrationTests.Methods
{
	[TestFixture]
	public class PerformRawTests
	{
		private HttpClient _client;

		[SetUp]
		public void Setup()
		{
			_client = new HttpClient(WebApiSetupFixture.BaseAddress);
		}

		[Test]
		public void Should_Be_Able_To_Get_Objects_Syncronious()
		{
			/* Setup */

			/* Test */
			var result = _client.PerformRaw(HttpMethod.Get, "api/todos/1");

			/* Assert */
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			/* Don't need to expect on the payload, as this is an previously
			 * internal method that is being tested in the generic methods. */
		}

		[Test]
		public async void Should_Be_Able_To_Get_Objects_Asyncronious()
		{
			/* Setup */

			/* Test */
			var result = await _client.PerformRawAsync(HttpMethod.Get, "api/todos/1");

			/* Assert */
			Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
			/* Don't need to expect on the payload, as this is an previously
			 * internal method that is being tested in the generic methods. */
		}
	}
}
