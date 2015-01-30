using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using PainlessHttp.Utils;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class AcceptHeaderMapperTests
	{
		private AcceptHeaderMapper _mapper;

		[SetUp]
		public void Setup()
		{
			_mapper = new AcceptHeaderMapper();
		}

		[TestCase("")]
		[TestCase(null)]
		public void Should_Return_Empty_List_For_Null_Or_Empty(string header)
		{
			/* Setup */
			/* Test */
			var result = _mapper.Map(header).ToList();

			/* Assert */
			Assert.That(result, Is.Empty);
		}

		[TestCase("application/json; q=.8; mxs=5; mxb=100000")]
		[TestCase("application/json; Q=.8; mXs=5; mxB=100000")]
		[TestCase("  application/json   ;  q = .8; mxs = 5;  mxb = 100000  ")]
		public void Should_Handle_Casing_And_Spacing(string header)
		{
			/* Setup */
			/* Test */
			var result = _mapper.Map(header).First();

			/* Assert */
			Assert.That(result.ContentType, Is.EqualTo("application/json").IgnoreCase);
			Assert.That(result.Q, Is.EqualTo(0.8f));
			Assert.That(result.Mxs, Is.EqualTo(5));
			Assert.That(result.Mxb, Is.EqualTo(100000));
		}

		[TestCase("text/plain; q=0.5, text/html, text/x-dvi; q=0.8, text/x-c", 4)]
		[TestCase("text/plain; q=0.5, text/html", 2)]
		[TestCase("text/plain; q=0.5", 1)]
		[TestCase("", 0)]
		public void Should_Handle_Several_Different_Accept_Types(string header, int expectedCount)
		{
			/* Setup */
			/* Test */
			var result = _mapper.Map(header).ToList();

			/* Assert */
			Assert.That(result, Has.Count.EqualTo(expectedCount));
		}
	}
}
