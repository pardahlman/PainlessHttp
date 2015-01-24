using System;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Custom
{
	public class SerializeSettings
	{
		public static ConfigureSerialize For(params ContentType[] applicationJson)
		{
			return new ConfigureSerialize(applicationJson);
		}
	}

	public class ConfigureSerialize
	{
		private readonly ContentType[] _contentTypes;

		public ConfigureSerialize(ContentType[] contentTypes)
		{
			_contentTypes = contentTypes;
		}

		public ConfigureDeserialize Serialize(Func<object, string> serialize)
		{
			return new ConfigureDeserialize(_contentTypes, serialize);
		}
	}

	public class ConfigureDeserialize
	{
		private readonly ContentType[] _contentType;
		private readonly Func<object, string> _serialize;

		public ConfigureDeserialize(ContentType[] contentType, Func<object, string> serialize)
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
