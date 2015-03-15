using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Integration;

namespace PainlessHttp.Client
{
	public interface IHttpClient
	{
		IHttpResponse<T> Get<T>(string url, object query = null) where T : class;
		Task<IHttpResponse<T>> GetAsync<T>(string url, object query = null) where T : class;

		IHttpResponse<T> Post<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;
		Task<IHttpResponse<T>> PostAsync<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;

		IHttpResponse<T> Put<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;
		Task<IHttpResponse<T>> PutAsync<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;

		IHttpResponse<T> Delete<T>(string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;
		Task<IHttpResponse<T>> DeleteAsync<T>(string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson) where T : class;

		/// <summary>
		/// Use this method if you want to get hold of the 'raw' HttpWebResponse. The response is wrapped in an interface, which makes it suitable for testing.
		/// </summary>
		IHttpWebResponse PerformRaw(HttpMethod method, string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson);
		Task<IHttpWebResponse> PerformRawAsync(HttpMethod method, string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson);
	}
}	
