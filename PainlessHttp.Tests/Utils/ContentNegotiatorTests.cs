using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Utils;
using HttpWebResponse = PainlessHttp.Integration.HttpWebResponse;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class ContentNegotiatorTests
	{
		private IEnumerable<ContentType> _supported;
		private ContentNegotiator _negotiator;
		private HttpWebResponse _response;

		[SetUp]
		public void Setup()
		{
			_supported = new List<ContentType> {ContentType.ApplicationXml, ContentType.ApplicationJson};
			_negotiator = new ContentNegotiator(_supported);
			_response = new HttpWebResponse
			{
				Headers = new WebHeaderCollection
						{
							{HttpRequestHeader.Accept, "application/json"}
						},
				StatusCode = HttpStatusCode.UnsupportedMediaType
			};
		}

		public class When_Calling_IsApplicable : ContentNegotiatorTests
		{
			[Test]
			public void Should_Be_False_If_Response_Is_Null()
			{
				/* Setup */
				/* Test */
				var result = _negotiator.IsApplicable(null);

				/* Assert */
				Assert.That(result, Is.False);
			}

			[Test]
			public void Should_Be_True_If_Response_Has_Unsupported_Media_Type_And_Accept_Header_With_Supported_Type()
			{
				/* Setup */
				//above

				/* Test */
				var result = _negotiator.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.True);
			}

			[Test]
			public void Should_Be_False_If_No_Accept_Headers_In_Response()
			{
				/* Setup */
				_response.Headers = new WebHeaderCollection();

				/* Test */
				var result = _negotiator.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.False);
			}

			[TestCase(HttpStatusCode.NoContent)]
			[TestCase(HttpStatusCode.Unauthorized)]
			[TestCase(HttpStatusCode.Forbidden)]
			public void Should_Be_False_If_HttpStatusCode_Is_Not_UnsupportedMediaType(HttpStatusCode code)
			{
				/* Setup */
				_response.StatusCode = code;

				/* Test */
				var result = _negotiator.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.False);
			}

			[Test]
			public void Should_Be_False_If_No_Content_Type_Is_Supported()
			{
				/* Setup */
				_response.Headers = new WebHeaderCollection
				{
					{HttpRequestHeader.Accept, "application/unsupported"}
				};

				/* Test */
				var result = _negotiator.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.False);
			}
		}
	}
}
