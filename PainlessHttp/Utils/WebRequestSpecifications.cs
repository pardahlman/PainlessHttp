using System;
using PainlessHttp.Http;

namespace PainlessHttp.Utils
{
	public class WebRequestSpecifications
	{
		public string Url { get; set; }
		public string AcceptHeader { get; set; }

		public Func<string> SerializeData { get; set; }
		public object Data { get; set; }
		public ContentType ContentType { get; set; }
		public bool ContentNegotiation { get; set; }
		
		public HttpMethod Method { get; set; }
	}
}