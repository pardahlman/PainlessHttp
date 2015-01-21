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
		public void ShouldConvertEnumsToStrings(ContentType type, string expectedResult)
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
		public void ShouldConvertStringToEnum(string type, ContentType expectedResult)
		{
			/* Setup */
			/* Test */
			var result = HttpConverter.ContentType(type);

			/* Assert */
			Assert.That(result, Is.EqualTo(expectedResult));
		}
	}
}
