using NUnit.Framework;
using PainlessHttp.Utils;

namespace PainlessHttp.Tests.Utils
{
	[TestFixture]
	public class ClientUtilsTests
	{
		public class When_Calling_GetUserAgent
		{
			[Test]
			public void ShouldReturnExpectedAgentName()
			{
				/* Setup */
				const string version = "0.11.5.0";
				var expected = string.Format("Painless Http Client {0}", version);
				/* Test */
				var agent = ClientUtils.GetUserAgent();

				/* Assert */
				Assert.That(agent, Is.EqualTo(expected));
			}
		}
	}
}
