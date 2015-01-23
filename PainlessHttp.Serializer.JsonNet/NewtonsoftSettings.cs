using Newtonsoft.Json;

namespace PainlessHttp.Serializer.JsonNet
{
	public class NewtonsoftSettings
	{
		public Formatting Formatting { get; set; }
		public JsonSerializerSettings Settings { get; set; }
		public JsonConverter[] JsonConverters { get; set; }
	}
}