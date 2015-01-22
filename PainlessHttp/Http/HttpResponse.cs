using System.Net;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Http
{
	public class HttpResponse : IHttpResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public string RawContent { get; set; }
	}

	public class HttpResponse<T> : IHttpResponse<T>
	{
		public HttpStatusCode StatusCode { get; set; }
		public T Body { get; set; }
	}
}
