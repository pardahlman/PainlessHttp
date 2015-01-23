using System;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Custom
{
	public class SerializeSettings
	{
		public static ConfigureSerialize For(ContentType applicationJson)
		{
			return new ConfigureSerialize(applicationJson);
		}
	}

	public class ConfigureSerialize
	{
		private readonly ContentType _applicationJson;

		public ConfigureSerialize(ContentType applicationJson)
		{
			_applicationJson = applicationJson;
		}

		public ConfigureDeserialize Serialize(Func<object, string> serialize)
		{
			return new ConfigureDeserialize(_applicationJson, serialize);
		}
	}

	public class ConfigureDeserialize
	{
		private readonly ContentType _contentType;
		private readonly Func<object, string> _serialize;

		public ConfigureDeserialize(ContentType contentType, Func<object, string> serialize)
		{
			_contentType = contentType;
			_serialize = serialize;
		}

		public IContentSerializer Deserialize(Func<string, Type, object> deserialize)
		{
			return new CustomSerializer(_contentType, _serialize, deserialize);
		}
	}
}
