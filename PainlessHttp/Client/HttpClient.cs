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

		public HttpClient(HttpClientConfiguration config)
		{
			_config = config;
			_urlBuilder = new UrlBuilder(config.BaseUrl);
		}

		public IHttpResponse Get(string url, object query = null)
		{
			var fullUrl = _urlBuilder.Build(url, query);

			var request = WebRequestWrapper
				.WithUrl(fullUrl)
				.WithMethod(HttpMethod.Get)
				.Prepare();

			var response = request.Perform();

			var payload = ClientUtils.ReadBodyAsync(response).Result;

			return new HttpResponse
			{
				StatusCode = response.StatusCode,
				RawContent = payload
			};
		}

		public IHttpResponse<T> Get<T>(string url, object query = null)
		{
			return GetAsync<T>(url, query).Result;
		}

		public async Task<IHttpResponse<T>> GetAsync<T>(string url, object query = null)
		{
			var fullUrl = _urlBuilder.Build(url, query);

			var request = WebRequestWrapper
				.WithUrl(fullUrl)
				.WithMethod(HttpMethod.Get)
				.Prepare();

			var response = request.Perform();

			var payloadTask = ClientUtils.ParseBodyAsync<T>(response);

			return await payloadTask.ContinueWith(task =>
			{
				IHttpResponse<T> result = new HttpResponse<T>
				{
					StatusCode = response.StatusCode,
					Body = payloadTask.Result
				};
				return result;
			});
		}

	}
}
