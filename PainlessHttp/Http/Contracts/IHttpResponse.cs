using System;
using System.Net;

namespace PainlessHttp.Http.Contracts
{
	public interface IHttpResponse<out T>
	{
		HttpStatusCode StatusCode { get;  }
		T Body { get; }
		string RawBody { get; set; }

		/// <summary>
		/// LastModified is a DateTime that represents when the resource was last changed. This
		/// property can be used to set 'If-Modified-Since' headers for caching response payloads
		/// and decrease traffic.
		/// </summary>
		DateTime LastModified { get; set; }
	}
}