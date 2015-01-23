using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Serializers.Defaults
{
	public class DefaultXml
	{
		private static readonly Dictionary<Type, XmlSerializer> CachedSerializers = new Dictionary<Type, XmlSerializer>();

		public static IContentSerializer GetSerializer()
		{
			return SerializeSettings
						.For(ContentType.ApplicationXml)
						.Serialize(Serialize)
						.Deserialize(Deserialize);
		}

		public static string Serialize(object data)
		{
			var type = data.GetType();
			var serializer = GetSerializer(type);

			var stream = new MemoryStream();
			serializer.Serialize(stream, data);
			stream.Position = 0;

			string result;
			using (var reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			stream.Dispose();
			return result;
		}

		public static XmlSerializer GetSerializer(Type type)
		{
			XmlSerializer serializer;
			if (!CachedSerializers.TryGetValue(type, out serializer))
			{
				serializer = new XmlSerializer(type);
				CachedSerializers.Add(type, serializer);
			}
			return serializer;
		}

		public static object Deserialize(string data, Type type)
		{
			var serializer = GetSerializer(type);

			object result;
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				result = serializer.Deserialize(stream);
			}
			return result;
		}
	}
}
