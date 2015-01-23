using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Serializers.Defaults
{
	public static class DefaultJson
	{
		private static readonly Dictionary<Type, DataContractJsonSerializer> CachedSerializers = new Dictionary<Type, DataContractJsonSerializer>();

		public static IContentSerializer GetSerializer()
		{
			return SerializeSettings
						.For(ContentType.ApplicationJson)
						.Serialize(Serialize)
						.Deserialize(Deserialize);
		}
		
		public static object Deserialize(string data, Type type)
		{
			var serializer = GetSerializer(type);

			object result;
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				result = serializer.ReadObject(stream);
			}
			return result;
		}

		public static string Serialize(object data)
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
			if (!CachedSerializers.TryGetValue(type, out serializer))
			{
				serializer = new DataContractJsonSerializer(type);
				CachedSerializers.Add(type, serializer);
			}
			return serializer;
		}
	}
}
