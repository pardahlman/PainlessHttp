using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Typed
{
	public class DefaultJsonSerializer : IContentSerializer
	{
		private static IDictionary<Type, DataContractJsonSerializer> cachedSerializers;
		private readonly IEnumerable<ContentType> _supportedTypes = new List<ContentType> { Http.ContentType.ApplicationJson };

		public DefaultJsonSerializer() : this(new Dictionary<Type, DataContractJsonSerializer>())
		{ /* Do not dublicate code here */}

		public DefaultJsonSerializer(IDictionary<Type, DataContractJsonSerializer> preCachedSerializers)
		{
			cachedSerializers = preCachedSerializers ?? new Dictionary<Type, DataContractJsonSerializer>();
		}

		public IEnumerable<ContentType> ContentType
		{
			get { return _supportedTypes; }
		}

		public string Serialize(object data)
		{
			var type = data.GetType();
			var serializer = GetSerializer(type);
			
			var stream = new MemoryStream();
			serializer.WriteObject(stream, data);
			stream.Position = 0;
			
			string result;
			using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			stream.Dispose();
			return result;
		}

		private static DataContractJsonSerializer GetSerializer(Type type)
		{
			DataContractJsonSerializer serializer;
			if (!cachedSerializers.TryGetValue(type, out serializer))
			{
				serializer = new DataContractJsonSerializer(type, new DataContractJsonSerializerSettings{ DateTimeFormat = new DateTimeFormat("yyyy-MM-ddThh:mm:sszzz")});
				cachedSerializers.Add(type, serializer);
			}
			return serializer;
		}

		public T Deserialize<T>(string data)
		{
			if (string.IsNullOrWhiteSpace(data))
			{
				return default(T);
			}

			var serializer = GetSerializer(typeof (T));

			T result;
			using (var stream	= new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				result = (T) serializer.ReadObject(stream);
			}
			return result;
		}
	}
}
