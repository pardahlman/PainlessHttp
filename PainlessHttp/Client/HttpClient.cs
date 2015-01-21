using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Client.Configuration;
using PainlessHttp.Client.Contracts;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Client
{
	public class HttpClient : IHttpClient
	{
		private readonly HttpClientConfiguration _config;

		public HttpClient(HttpClientConfiguration config)
		{
			_config = config;
		}

		public IHttpResponse Get(string url, ContentType type = ContentType.ApplicationJson, object query = null)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}{1}", _config.BaseUrl, url));
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.UserAgent = "Painless Http Client";
			httpWebRequest.Method = "GET";

			var response = (HttpWebResponse)httpWebRequest.GetResponse();
			var responseStream = response.GetResponseStream();

			string raw;
			using (var reader = new StreamReader(responseStream))
			{
				raw = reader.ReadToEnd();
			}

			return new HttpResponse
			{
				StatusCode = response.StatusCode,
				RawContent = raw
			};
		}

		public IHttpResponse<T> Get<T>(string url, ContentType type, object query = null)
		{
			throw new NotImplementedException();
		}

		public Task<IHttpResponse<T>> GetAsync<T>(string url, ContentType type, object query = null)
		{
			throw new NotImplementedException();
		}
	}
}
