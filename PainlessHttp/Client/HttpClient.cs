using System.Linq;
using System.Threading.Tasks;
using PainlessHttp.Client.Configuration;
using PainlessHttp.Client.Contracts;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Serializers.Defaults;
using PainlessHttp.Utils;

namespace PainlessHttp.Client
{
	public class HttpClient : IHttpClient
	{
		private readonly UrlBuilder _urlBuilder;
		private readonly WebRequestBuilder _reqBuilder;
		private readonly ResponseTransformer _responseTransformer;
		public HttpClient(string baseUrl) : this(new HttpClientConfiguration{BaseUrl = baseUrl})
		{
			/* Don't duplicate code here.*/
		}
		
		public HttpClient(HttpClientConfiguration config)
		{
			var serializers = config.Advanced.Serializers.Concat(ContentSerializers.Defaults).ToList();

			var defaultContentType = config.Advanced.ContentType == ContentType.Unknown
				? ContentType.ApplicationJson
				: config.Advanced.ContentType;
 
			_urlBuilder = new UrlBuilder(config.BaseUrl);
			_reqBuilder = new WebRequestBuilder(serializers, defaultContentType);
			_responseTransformer = new ResponseTransformer(serializers);
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

		public IHttpResponse<T> Post<T>(string url, object data, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			return PostAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> PostAsync<T>(string url, object data, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Post, url, query, type, data);
			return response;
		}

		public IHttpResponse<T> Put<T>(string url, object data, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			return PutAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> PutAsync<T>(string url, object data, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Put, url, query, type, data);
			return response;
		}

		public IHttpResponse<T> Delete<T>(string url, object data = null, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			return DeleteAsync<T>(url, data, query, type).Result;
		}

		public async Task<IHttpResponse<T>> DeleteAsync<T>(string url, object data = null, object query = null, ContentType type = ContentType.Negotiated) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Delete, url, query, type, data);
			return response;
		}

		private async Task<IHttpResponse<T>> PerformRequestAsync<T>(HttpMethod method, string url, object query, ContentType type = ContentType.Negotiated, object data = null) where T : class 
		{
			var fullUrl = _urlBuilder.Build(url, query);

			var request = _reqBuilder
				.WithUrl(fullUrl)
				.WithMethod(method)
				.WithPayload(data, type)
				.Prepare();

			var response = await request.PerformAsync();

			return await _responseTransformer.TransformAsync<T>(response);
		}
	}
}
