using System;
using System.IO;
using System.Net;

namespace PainlessHttp.Http.Contracts
{
	public interface IHttpWebResponse : IDisposable
	{
		string CharacterSet { get; }
		string ContentEncoding { get; }
		long ContentLength { get; }
		string ContentType { get; }
		CookieCollection Cookies { get; set; }
		WebHeaderCollection Headers { get; }
		bool IsMutuallyAuthenticated { get; }
		DateTime LastModified { get; }
		string Method { get; }
		Version ProtocolVersion { get; }
		Uri ResponseUri { get; }
		string Server { get; }
		HttpStatusCode StatusCode { get; }
		string StatusDescription { get; }
		bool SupportsHeaders { get; }
		void Close();
		string GetResponseHeader(string headerName);
		Stream GetResponseStream();
		void SetResponseStream(Stream responseStream);
	}
}
