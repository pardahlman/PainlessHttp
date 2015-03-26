using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializer.JsonNet
{
	public class PainlessJsonNet : IContentSerializer
	{
		private readonly Func<object, string> _serialize;
		private readonly Func<string, Type, object> _deserialize;

		public PainlessJsonNet(Func<object,string> serialize = null, Func<string, Type, object> deserialize = null  )
		{
			_serialize = serialize ?? JsonConvert.SerializeObject;
			_deserialize = deserialize ?? JsonConvert.DeserializeObject;
			ContentType = new List<ContentType> {Http.ContentType.ApplicationJson};
		}
		public IEnumerable<ContentType> ContentType { get; private set; }
		
		public string Serialize(object data)
		{
			return _serialize(data);
		}

		public T Deserialize<T>(string data)
		{
			var result = _deserialize(data, typeof (T));
			return (T)result;
		}
	}
}
