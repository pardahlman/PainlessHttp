using System;
using System.Collections.Generic;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Typed
{
	public class DefaultTextPlainSerializer : IContentSerializer
	{
		public IEnumerable<ContentType> ContentType
		{
			get { return new[] {Http.ContentType.TextPlain}; }
		}

		public string Serialize(object data)
		{
			return data.ToString();
		}

		public T Deserialize<T>(string data)
		{
			if (typeof (T) == typeof (string))
			{
				var result = (T) Convert.ChangeType(data, typeof (T));
				return result;
			}

			return default(T);
		}
	}
}
