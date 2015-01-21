using System;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Utils;

namespace PainlessHttp.Tests.Utils
{
	public class HttpConverterTests
	{
		[Test]
		public void ShouldThrowExceptionIfContentTypeIsUnknown()
		{
			/* Setup */
			var type = ContentType.Unknown;

			/* Test & Assert */
			Assert.Throws<ArgumentException>(() => HttpConverter.ContentType(type));
		}

		[Test]
		public void ShouldThrowExceptionIfStringDoesNotMatchContentType()
		{
			/* Setup */
			const string nonMatch = "This is not a content type";

			/* Test & Assert */
			Assert.Throws<ArgumentException>(() => HttpConverter.ContentType(nonMatch));
		}

		[TestCase(ContentType.ApplicationJson, ContentTypes.ApplicationJson)]
		[TestCase(ContentType.ApplicationXml, ContentTypes.ApplicationXml)]
		[TestCase(ContentType.TextPlain, ContentTypes.TextPlain)]
		[TestCase(ContentType.TextCsv, ContentTypes.TextCsv)]
		[TestCase(ContentType.TextHtml, ContentTypes.TextHtml)]
		public void ShouldConvertContentTypeEnumsToStrings(ContentType type, string expectedResult)
		{
			/* Setup */
			/* Test */
			var result = HttpConverter.ContentType(type);

			/* Assert */
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase(ContentTypes.ApplicationJson, ContentType.ApplicationJson)]
		[TestCase(ContentTypes.ApplicationXml, ContentType.ApplicationXml)]
		[TestCase(ContentTypes.TextPlain, ContentType.TextPlain)]
		[TestCase(ContentTypes.TextCsv, ContentType.TextCsv)]
		[TestCase(ContentTypes.TextHtml, ContentType.TextHtml)]
		public void ShouldConvertContentTypeStringToEnum(string type, ContentType expectedResult)
		{
			/* Setup */
			/* Test */
			var result = HttpConverter.ContentType(type);

			/* Assert */
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase(HttpMethod.Get, HttpMethods.Get)]
		[TestCase(HttpMethod.Post, HttpMethods.Post)]
		[TestCase(HttpMethod.Put, HttpMethods.Put)]
		[TestCase(HttpMethod.Delete, HttpMethods.Delete)]
		public void ShouldConvertMethodEnumToString(HttpMethod method, string expectedResult)
		{
			/* Setup */
			/* Test */
			var result = HttpConverter.HttpMethod(method);

			/* Assert */
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase(HttpMethods.Get, HttpMethod.Get)]
		[TestCase(HttpMethods.Post, HttpMethod.Post)]
		[TestCase(HttpMethods.Put, HttpMethod.Put)]
		[TestCase(HttpMethods.Delete, HttpMethod.Delete)]
		public void ShouldConvertMethodStringToEnum(string method, HttpMethod expectedResult)
		{
			/* Setup */
			/* Test */
			var result = HttpConverter.HttpMethod(method);

			/* Assert */
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
