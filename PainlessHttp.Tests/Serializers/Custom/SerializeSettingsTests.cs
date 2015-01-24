using System;
using NUnit.Framework;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Tests.Serializers.Custom
{
	[TestFixture]
	public class SerializeSettingsTests
	{
		[Test]
		public void Should_Set_Correct_Content_Types()
		{
			/* Setup */
			var expected = new[] {ContentType.ApplicationJson, ContentType.TextCsv};
			
			/* Test */
			var result = SerializeSettings
					.For(expected)
						.Serialize(o => string.Empty)
						.Deserialize((s, t) => new object());

			/* Assert */
			Assert.That(result.ContentType, Is.EqualTo(expected));
		}
	}
}
