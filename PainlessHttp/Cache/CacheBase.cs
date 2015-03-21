using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public abstract class CacheBase : IModifiedSinceCache
	{
		public abstract CachedObject Get(HttpWebRequest req);
		public abstract Task AddAsync(HttpWebRequest rawRequest, IHttpWebResponse rawResponse);

		protected static string GetCacheKey(IHttpWebResponse rawResponse)
		{
			return string.Format("{0}_{1}_{2}", rawResponse.Method, rawResponse.ResponseUri.AbsolutePath, rawResponse.ResponseUri.Query);
		}

		protected static string GetCacheKey(WebRequest rawRequest)
		{
			return string.Format("{0}_{1}_{2}", rawRequest.Method, rawRequest.RequestUri.AbsolutePath, rawRequest.RequestUri.Query);
		}
	}
}
