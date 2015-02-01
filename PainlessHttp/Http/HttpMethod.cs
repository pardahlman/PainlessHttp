namespace PainlessHttp.Http
{
	public enum HttpMethod
	{
		Unknown,
		Get,
		Post,
		Put,
		Delete,
		Options
	}

	public class HttpMethods
	{
		public const string Get = "GET";
		public const string Post = "POST";
		public const string Delete = "DELETE";
		public const string Put = "PUT";
		public const string Options = "OPTIONS";
	}

}
