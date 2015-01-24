using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Typed
{
	public class DefaultXmlSerializer : IContentSerializer
	{
		private static IDictionary<Type, XmlSerializer> cachedSerializers;

		private readonly IEnumerable<ContentType> _supportedTypes = new List<ContentType> { Http.ContentType.ApplicationJson };

		public IEnumerable<ContentType> ContentType
		{
			get { return _supportedTypes; }
		}

		public DefaultXmlSerializer() : this(new Dictionary<Type, XmlSerializer>())
		{ /* Don't duplicate code here*/ }

		public DefaultXmlSerializer(IDictionary<Type, XmlSerializer> preCached)
		{
			cachedSerializers = preCached;
		}

		public string Serialize(object data)
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

		public XmlSerializer GetSerializer(Type type)
		{
			XmlSerializer serializer;
			if (!cachedSerializers.TryGetValue(type, out serializer))
			{
				serializer = new XmlSerializer(type);
				cachedSerializers.Add(type, serializer);
			}
			return serializer;
		}

		public T Deserialize<T>(string data)
		{
			var serializer = GetSerializer(typeof (T));
			
			T result;
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
			{
				result = (T) serializer.Deserialize(stream);
			}
			return result;
		}
	}
}
