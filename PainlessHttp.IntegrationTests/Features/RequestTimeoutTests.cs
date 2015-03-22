using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PainlessHttp.Client;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.IntegrationTests.Features
{
	[TestFixture]
	public class RequestTimeoutTests
	{
		private HttpClient _client;
		private Configuration _config;

		[SetUp]
		public void Setup()
		{
			_config = new Configuration
			{
				BaseUrl = WebApiSetupFixture.BaseAddress,
				Advanced =
				{
					RequestTimeout = new TimeSpan(0,0,0,0,1500)
				}
			};

			_client = new HttpClient(_config);
		}

		[Test]
		public async void Should_Timeout_If_Response_Time_Is_Above_Configuration_Threshhold()
		{
			/* Setup */
			/* Test */
			var response = await _client.GetAsync<Todo>("api/timeout", new {delay = 2001});

			/* Assert */
			Assert.That(response.Body, Is.Null);
			Assert.That(response.StatusCode, Is.Not.EqualTo(HttpStatusCode.OK));
			Assert.That(response.RawBody, Contains.Substring("The time out can be increased by configuring the client's Request time-out at Configuration.Advanced.RequestTimeout"));
		}

		[Test]
		public async void Should_Not_Timeout_If_Response_Time_Is_Below_Configuration_Threshhold()
		{
			/* Setup */
			/* Test */
			var response = await _client.GetAsync<string>("api/timeout", new { delay = 1 });

			/* Assert */
			Assert.That(response.RawBody, Is.Not.StringContaining("The time out can be increased by configuring the client's Request time-out at Configuration.Advanced.RequestTimeout"));
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}

		[TestCase(new []{ 1 })]
		[TestCase(new []{ 1, 10 })]
		[TestCase(new []{ 1, 10, 100 })]
		[TestCase(new []{ 1, 10, 100, 200 })]
		[TestCase(new []{ 1, 10, 100, 200, 300 })]
		[TestCase(new []{ 1, 10, 100, 200, 300, 400 })]
		public async void Should_Not_Get_Performance_Issues_When_Performing_Several_Requests(int[] delays)
		{
			/* Warm up */
			await _client.GetAsync<string>("api/timeout", new { delay = 1 });

			/* Setup */
			var responses = new List<IHttpResponse<string>>();
			var maxResponseTime = delays.Aggregate(0, (accumilated, increment) => accumilated + increment)  + delays.Length*20; // accumulated delays + 20 ms overhead/request
			var stopWatch = new Stopwatch();
			/* Test */
			stopWatch.Start();
			foreach (var delay in delays)
			{
				var response = await _client.GetAsync<string>("api/timeout", new { delay = delay });
				responses.Add(response);
			}
			stopWatch.Stop();
			
			/* Assert */
			var errorResponses = responses.Where(r => r.StatusCode != HttpStatusCode.OK).ToList();
			Assert.That(errorResponses, Is.Empty);
			Assert.That(stopWatch.ElapsedMilliseconds, Is.LessThanOrEqualTo(maxResponseTime));
		}


	}
}
