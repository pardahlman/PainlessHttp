using System;
using System.Threading.Tasks;
using PainlessHttp.Client.Contracts;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Client
{
	public class HttpClient : IHttpClient
	{
		public IHttpResponse Get(string url, ContentType type, object query = null)
		{
			throw new NotImplementedException();
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
