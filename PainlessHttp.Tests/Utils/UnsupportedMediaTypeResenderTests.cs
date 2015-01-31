using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
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
		private Mock<IWebRequestWorker> _worker;
		private WebRequestSpecifications _reqSpecs;

		[SetUp]
		public void Setup()
		{
			_worker = new Mock<IWebRequestWorker>();
			_serializers = new List<IContentSerializer>
			{
				new DefaultJsonSerializer(),
				new DefaultXmlSerializer()
			};
			_resender = new UnsupportedMediaTypeResender(_serializers, _worker.Object);
			_reqSpecs = new WebRequestSpecifications
			{
				ContentNegotiation = true
			};
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

		public class When_Calling_ResendRequestAsync : UnsupportedMediaTypeResenderTests
		{
			[Test]
			public void Should_Throw_Exception_If_No_Serializer_Found()
			{
				/* Setup */
				_response.Headers = new WebHeaderCollection
				{
					{HttpRequestHeader.Accept, "application/unsupported"}
				};

				/* Test & Setup */
				Assert.Throws<ArgumentException>(async () =>  await _resender.ResendRequestAsync(_response, _reqSpecs));
			}

			[Test]
			public void Should_Update_Content_Type_Based_On_Accept_Headers()
			{
				/* Setup */
				_reqSpecs.ContentType = ContentType.TextCsv;
				_worker
					.Setup(w => w.GetResponseAsync(
						It.Is<WebRequestSpecifications>(spec => spec.ContentType == ContentType.ApplicationJson)))
					.Verifiable();

				/* Test */
				_resender.ResendRequestAsync(_response, _reqSpecs);

				/* Assert */
				_worker.VerifyAll();
			}

			[Test]
			public async void Should_Update_Data_Serialization_Method_Based_Out_Accept_Headers()
			{
				/* Setup */
				_reqSpecs.Data = new AcceptHeaderField{ ContentType = "sometype"};
				var expectedSerialized = new DefaultJsonSerializer().Serialize(_reqSpecs.Data);
				var actualSerialized = string.Empty;
				_worker
					.Setup(w => w.GetResponseAsync(It.IsAny<WebRequestSpecifications>()))
					.Callback((WebRequestSpecifications w) => actualSerialized = w.SerializeData());
				
				/* Test */
				_resender.ResendRequestAsync(_response, _reqSpecs);

				/* Assert */
				Assert.That(actualSerialized, Is.EqualTo(expectedSerialized));
			}

			[Test]
			public void Should_Not_Resend_Request_If_Content_Negotiation_Is_Disabled()
			{
				/* Setup */
				_reqSpecs.ContentNegotiation = false;
				Expression<Action<IWebRequestWorker>> performRequest = worker => worker.GetResponseAsync(It.IsAny<WebRequestSpecifications>());
				_worker
					.Setup(performRequest)
					.Verifiable("");

				/* Test */
				_resender.ResendRequestAsync(_response, _reqSpecs);

				/* Assert */
				_worker.Verify(performRequest, Times.Never);
			}
		}
	}
}
