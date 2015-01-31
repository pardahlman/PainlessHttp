using System.Collections.Generic;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Serializers.Typed;

namespace PainlessHttp.Serializers.Defaults
{
	public static class ContentSerializers
	{
		public static readonly List<IContentSerializer> Defaults = new List<IContentSerializer>
		{
			new DefaultJsonSerializer(),
			new DefaultXmlSerializer(),
			new DefaultTextPlainSerializer()
		};
	}
}
