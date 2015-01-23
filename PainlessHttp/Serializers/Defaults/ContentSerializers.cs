using System.Collections.Generic;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Serializers.Defaults
{
	public static class ContentSerializers
	{
		public static readonly List<IContentSerializer> Defaults = new List<IContentSerializer>
		{
			new DefaultJsonSerializer()
		};
	}
}
