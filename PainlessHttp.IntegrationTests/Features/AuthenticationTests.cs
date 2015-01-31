using System;
using System.Linq;
using System.Net;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Data;
using PainlessHttp.Http;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class AuthenticationTests
	{
		private HttpClient _client;
		private string _testRequestId;
		private RequestRepo _requestRepo;

		[SetUp]
		public void Setup()
		{
			_testRequestId = Guid.NewGuid().ToString();
			_requestRepo = RequestRepo.Instance;
			var config = new HttpClientConfiguration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress,
				Advanced =
				{
					WebrequestModifier = w => w.Headers.Add("X-Request-Id", _testRequestId),
					Credentials =
					{
						new Credential
						{
							UserName = "pardahlman",
							Password = "password",
							Domain = WebApiSetupFixture.BaseAddress,
							AuthTypes = { AuthenticationType.Basic }
						}
					}
				}
			};

			_client = new HttpClient(config);
		}

		[Test]
		public void Should_Return_Unauthorized_If_No_Credentials_Provided()
		{
			/* Setup */
			_client = new HttpClient(WebApiSetupFixture.BaseAddress);

			/* Test */
			var response = _client.Get<string>("api/authentication");

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
		}

		[Test]
		public async void Should_Return_Forbidden_If_Authentication_Failed()
		{
			/* Setup */

			/* Test */
			var response = await _client.GetAsync<string>("api/authentication");

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
			}

		[Test]
		public async void Should_Return_Ok_If_Authentication_Succeeded()
		{
			/* Setup */

			/* Test */
			var response = await _client.GetAsync<string>("api/authentication", new { user = "pardahlman", pwd = "password"});

			/* Assert */
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		[Test]
		public async void Should_Only_Authenticate_Once_Per_Domain()
		{
			/* Setup */

			/* Test */
			await _client.GetAsync<string>("api/authentication", new { user = "pardahlman", pwd = "password" });
			await _client.GetAsync<string>("api/authentication", new { user = "pardahlman", pwd = "password" });

			/* Asserts*/
			var requests = _requestRepo.GetRequestForSession(_testRequestId).ToList();
			Assert.That(requests, Has.Count.EqualTo(3));
			Assert.That(requests[0].Headers.Authorization, Is.Null, "No Auth header in first request");
			Assert.That(requests[1].Headers.Authorization, Is.Not.Null, "Second request contains auth headers.");
			Assert.That(requests[2].Headers.Authorization, Is.Not.Null, "Second request contains auth headers.");
		}
	}
}
