using System;
using System.Collections.Generic;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Custom
{
	public class CustomSerializer : IContentSerializer
	{
		private readonly Func<object, string> _serialize;
		private readonly Func<string, Type, object> _deserialize;
		public IEnumerable<ContentType> ContentType { get; private set; }

		public CustomSerializer(ContentType type, Func<object, string> serialize, Func<string, Type, object> deserialize)
		{
			_serialize = serialize;
			_deserialize = deserialize;
			ContentType = new List<ContentType> {type};
		}

		public string Serialize(object data)
		{
			return _serialize(data);
		}

		public T Deserialize<T>(string data)
		{
			var obj = _deserialize(data, typeof(T));
			return (T) obj;
		}
	}
}
