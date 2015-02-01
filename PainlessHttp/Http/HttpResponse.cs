using System.Net;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Http
{
	public class HttpResponse<T> : IHttpResponse<T>
	{
		public HttpStatusCode StatusCode { get; set; }
		public T Body { get; set; }
	}
}
