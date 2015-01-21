using NUnit.Framework;
using PainlessHttp.Utils;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class UrlBuilderTests
	{
		private UrlBuilder _builder;
		private const string _baseUrl = "https://www.github.com";

		[SetUp]
		public void Setup()
		{
			_builder = new UrlBuilder(_baseUrl);
		}

		[Test]
		public void Should_Return_Url_That_Starts_With_Base_Url()
		{
			/* Setup */
			/* Test */
			var result = _builder.Build();

			/* Assert */
			Assert.That(result, Is.StringStarting(_baseUrl));
		}

		[Test]
		public void Should_Add_Relative_Path_To_Base_Url()
		{
			/* Setup */
			const string relative = "/pardahlman";
			var expected = string.Format("{0}{1}", _baseUrl, relative);

			/* Test */
			var result = _builder.Build(relative);

			/* Assert */
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void Should_Add_Query_Parameters()
		{
			/* Setup */
			var query = new {firstParam = "firstValue", secondParam = "secondValue"};
			var expected = string.Format("{0}?firstParam=firstValue&secondParam=secondValue", _baseUrl);

			/* Test */
			var result = _builder.Build(string.Empty, query);

			/* Assert */
			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void Should_Be_Able_To_Handle_Relative_Url_And_Query()
		{
			/* Setup */
			const string relative = "/pardahlman";
			var query = new { id = 1};
			var expected = string.Format("{0}{1}?id=1", _baseUrl, relative);

			/* Test */
			var result = _builder.Build(relative, query);

			/* Assert */
			Assert.That(result, Is.EqualTo(expected));
		}
	}
}
