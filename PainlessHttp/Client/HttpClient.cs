using System.Threading.Tasks;
using PainlessHttp.Client.Configuration;
using PainlessHttp.Client.Contracts;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Utils;

namespace PainlessHttp.Client
{
	public class HttpClient : IHttpClient
	{
		private readonly HttpClientConfiguration _config;
		private readonly UrlBuilder _urlBuilder;
		private readonly WebRequestWrapper _requestWrapper;

		public HttpClient(HttpClientConfiguration config)
		{
			_config = config;
			_urlBuilder = new UrlBuilder(config.BaseUrl);
			_requestWrapper = new WebRequestWrapper();
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

		public IHttpResponse<T> Post<T>(string url, object data, object query = null) where T : class
		{
			return PostAsync<T>(url, data, query).Result;
		}

		public async Task<IHttpResponse<T>> PostAsync<T>(string url, object data, object query = null) where T : class
		{
			var response = await PerformRequestAsync<T>(HttpMethod.Post, url, query, ContentType.ApplicationJson, data);
			return response;
		}

		private async Task<IHttpResponse<T>> PerformRequestAsync<T>(HttpMethod method, string url, object query, ContentType type = ContentType.Unknown, object data = null)where T : class 
		{
			var fullUrl = _urlBuilder.Build(url, query);

			var request = _requestWrapper
				.WithUrl(fullUrl)
				.WithMethod(method)
				.WithPayload(data, type)
				.Prepare();

			var response = request.Perform();

			var payloadTask = ClientUtils.ParseBodyAsync<T>(response);

			return await payloadTask.ContinueWith(task =>
			{
				IHttpResponse<T> result = new HttpResponse<T>
				{
					StatusCode = response.StatusCode,
					Body = task.Result
				};
				return result;
			});
		}
	}
}
