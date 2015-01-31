using System.Net;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Utils;
using HttpWebResponse = PainlessHttp.Utils.HttpWebResponse;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class UnauthorizedResenderTests
	{
		private UnauthorizedResender _resender;
		private HttpWebResponse _response;

		[SetUp]
		public void Setup()
		{
			_resender = new UnauthorizedResender();
			_response = new HttpWebResponse
			{
				StatusCode = HttpStatusCode.Unauthorized,
				Headers = new WebHeaderCollection
				{
					{HttpResponseHeader.WwwAuthenticate, "Basic"}
				}
			};
		}

		public class When_Calling_IsApplicable : UnauthorizedResenderTests
		{
			[Test]
			public void Should_Be_False_If_Request_Is_Null()
			{
				/* Setup */
				/* Test */
				var result = _resender.IsApplicable(null);
				/* Assert */
			}

			[Test]
			public void Should_Be_True_If_Request_Is_Unauthorized_And_Contains_Authenticate_Header()
			{
				/* Setup */
				//above 

				/* Test */
				var result = _resender.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.True);
			}

			[TestCase(HttpStatusCode.OK)]
			[TestCase(HttpStatusCode.Forbidden)]
			[TestCase(HttpStatusCode.GatewayTimeout)]
			public void Should_Be_False_If_Status_Code_Is_Other_Than_Unauthorized(HttpStatusCode status)
			{
				/* Setup */
				_response.StatusCode = status;

				/* Test */
				var result = _resender.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.False);
			}



		}
	}
}
