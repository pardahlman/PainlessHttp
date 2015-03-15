using System.Net;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public class NoCache : IModifiedSinceCache
	{
		public CachedObject Get(HttpWebRequest req)
		{
			return null;
		}

		public void Add(HttpWebRequest rawRequest, IHttpWebResponse rawResponse)
		{
			
		}
	}
}
