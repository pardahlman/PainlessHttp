using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public class NoCache : IModifiedSinceCache
	{
		public CachedObject Get(HttpWebRequest req)
		{
			return null;
		}

		public Task AddAsync(HttpWebRequest rawRequest, IHttpWebResponse rawResponse)
		{
			return Task.FromResult(true);
		}
	}
}
