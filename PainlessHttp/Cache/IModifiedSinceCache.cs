using System.Net;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public interface IModifiedSinceCache
	{
		CachedObject Get(HttpWebRequest req);
		void Add(HttpWebRequest rawRequest, IHttpWebResponse rawResponse);
	}
}