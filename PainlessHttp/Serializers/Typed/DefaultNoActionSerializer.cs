using System.Collections.Generic;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Typed
{
	public class DefaultNoActionSerializer : IContentSerializer
	{
		public IEnumerable<ContentType> ContentType
		{
			get { return new[] {Http.ContentType.TextPlain, Http.ContentType.TextCsv, Http.ContentType.TextHtml}; }
		}

		public string Serialize(object data)
		{
			return data.ToString();
		}

		public T Deserialize<T>(string data)
		{
			return default(T);
		}
	}
}
