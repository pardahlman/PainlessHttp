using System.Net;

namespace PainlessHttp.Http.Contracts
{
	public interface IHttpResponse
	{
		HttpStatusCode StatusCode { get; }
		string RawContent { get; }
	}

	public interface IHttpResponse<out T>
	{
		HttpStatusCode StatusCode { get;  }
		T Body { get; }
	}

}