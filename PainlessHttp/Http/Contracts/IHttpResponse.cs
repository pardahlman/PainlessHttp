using System.Net;

namespace PainlessHttp.Http.Contracts
{
	public interface IHttpResponse<out T>
	{
		HttpStatusCode StatusCode { get;  }
		T Body { get; }
		string RawBody { get; set; }
	}
}