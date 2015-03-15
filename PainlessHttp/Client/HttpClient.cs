using System.CodeDom;
using System.Linq;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Integration;
using PainlessHttp.Serializers.Defaults;
using PainlessHttp.Utils;

namespace PainlessHttp.Client
{
	public class HttpClient : IHttpClient
	{
		private readonly ResponseTransformer _responseTransformer;
		private readonly WebRequester _webRequester;

		public HttpClient(string baseUrl = "") : this(new Configuration{BaseUrl = baseUrl})
		{
			/* Don't duplicate code here.*/
		}
		
		public HttpClient(Configuration config)
		{
			config.Advanced.Serializers = config.Advanced.Serializers.Concat(ContentSerializers.Defaults).ToList();
 
			_webRequester = new WebRequester(config);
			_responseTransformer = new ResponseTransformer(config.Advanced.Serializers);
		}

		public IHttpResponse<T> Get<T>(string url, object query = null) where T : class 
		{
			return GetAsync<T>(url, query).Result;
		}

		public async Task<IHttpResponse<T>> GetAsync<T>(string url, object query = null) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Get, url, query);
			return response;
		}

		public IHttpResponse<T> Post<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			return PostAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> PostAsync<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Post, url, query, type, data);
			return response;
		}

		public IHttpResponse<T> Put<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			return PutAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> PutAsync<T>(string url, object data, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Put, url, query, type, data);
			return response;
		}

		public IHttpResponse<T> Delete<T>(string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			return DeleteAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> DeleteAsync<T>(string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Delete, url, query, type, data);
			return response;
		}

		public IHttpWebResponse PerformRaw(HttpMethod method, string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson)
		{
			var response = GetRawResponseAsync(method, url, query, type, data).Result;
			return response;
		}

		public Task<IHttpWebResponse> PerformRawAsync(HttpMethod method, string url, object data = null, object query = null, ContentType type = ContentType.ApplicationJson)
		{
			var response = GetRawResponseAsync(method, url, query, type, data);
			return response;
		}

		private async Task<IHttpResponse<T>> PerformRequestAsync<T>(HttpMethod method, string url, object query, ContentType type = ContentType.ApplicationJson, object data = null) where T : class
		{
			var response = await GetRawResponseAsync(method, url, query,type, data);
			return await _responseTransformer.TransformAsync<T>(response);
		}

		private async Task<IHttpWebResponse> GetRawResponseAsync(HttpMethod method, string url, object query, ContentType type, object data)
		{
			var response = await _webRequester
				.WithUrl(url, query)
				.WithMethod(method)
				.WithPayload(data, type)
				.PerformAsync();
			return response;
		}
	}
}
