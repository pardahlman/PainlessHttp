using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public interface IModifiedSinceCache
	{
		CachedObject Get(HttpWebRequest req);
		Task AddAsync(HttpWebRequest rawRequest, IHttpWebResponse rawResponse);
	}
}