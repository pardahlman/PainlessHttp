using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Client.Contracts
{
	interface IHttpClient
	{
		IHttpResponse Get(string url, ContentType type, object query = null);
		IHttpResponse<T> Get<T>(string url, ContentType type, object query = null);
		Task<IHttpResponse<T>> GetAsync<T>(string url, ContentType type, object query = null);
	}
}
