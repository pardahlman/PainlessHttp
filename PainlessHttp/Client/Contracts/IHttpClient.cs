using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Client.Contracts
{
	interface IHttpClient
	{
		IHttpResponse Get(string url, object query = null);
		IHttpResponse<T> Get<T>(string url, object query = null);
		Task<IHttpResponse<T>> GetAsync<T>(string url, object query = null);
	}
}
