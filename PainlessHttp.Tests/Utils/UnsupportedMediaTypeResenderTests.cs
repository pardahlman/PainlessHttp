using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Serializers.Typed;
using PainlessHttp.Utils;
using HttpWebResponse = PainlessHttp.Utils.HttpWebResponse;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class UnsupportedMediaTypeResenderTests
	{
		private UnsupportedMediaTypeResender _resender;
		private HttpWebResponse _response;
		private List<IContentSerializer> _serializers;

		[SetUp]
		public void Setup()
		{
			_serializers = new List<IContentSerializer>
			{
				new DefaultJsonSerializer(),
				new DefaultXmlSerializer()
			};
			_resender = new UnsupportedMediaTypeResender(_serializers);
			_response = new HttpWebResponse
			{
				Headers = new WebHeaderCollection
						{
							{HttpRequestHeader.Accept, "application/json"}
						},
				StatusCode = HttpStatusCode.UnsupportedMediaType
			};
		}

		public class When_Calling_IsApplicable : UnsupportedMediaTypeResenderTests
		{
			[Test]
			public void Should_Be_False_If_Response_Is_Null()
			{
				/* Setup */
				/* Test */
				var result = _resender.IsApplicable(null);

				/* Assert */
				Assert.That(result, Is.False);
			}

			[Test]
			public void Should_Be_True_If_Response_Has_Unsupported_Media_Type_And_Accept_Header_With_Supported_Type()
			{
				/* Setup */
				//above

				/* Test */
				var result = _resender.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.True);
			}

			[Test]
			public void Should_Be_False_If_No_Accept_Headers_In_Response()
			{
				/* Setup */
				_response.Headers = new WebHeaderCollection();

				/* Test */
				var result = _resender.IsApplicable(_response);

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
				var result = _resender.IsApplicable(_response);

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
				var result = _resender.IsApplicable(_response);

				/* Assert */
				Assert.That(result, Is.False);
			}
		}


	}
}
