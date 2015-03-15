namespace PainlessHttp.Http
{
	public enum ContentType
	{
		Unknown,
		ApplicationJson,
		ApplicationXml,
		TextPlain,
		TextCsv,
		TextHtml
	}

	public static class ContentTypes
	{
		public const string TextPlain = "text/plain";
		public const string TextHtml = "text/html";
		public const string TextCsv = "text/csv";
		public const string ApplicationJson = "application/json";
		public const string ApplicationXml = "application/xml";
	}
}