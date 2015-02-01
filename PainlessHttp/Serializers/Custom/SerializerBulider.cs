using System;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Custom
{
	public class SerializerBulider
	{
		public static FluentSerializerBuilder For(params ContentType[] applicationJson)
		{
			return new FluentSerializerBuilder(applicationJson);
		}
	}

	public class FluentSerializerBuilder
	{
		private readonly ContentType[] _contentTypes;

		public FluentSerializerBuilder(ContentType[] contentTypes)
		{
			_contentTypes = contentTypes;
		}

		public FluentDeserializeBuilder Serialize(Func<object, string> serialize)
		{
			return new FluentDeserializeBuilder(_contentTypes, serialize);
		}
	}

	public class FluentDeserializeBuilder
	{
		private readonly ContentType[] _contentType;
		private readonly Func<object, string> _serialize;

		public FluentDeserializeBuilder(ContentType[] contentType, Func<object, string> serialize)
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
