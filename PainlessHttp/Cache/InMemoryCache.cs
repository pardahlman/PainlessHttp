using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;

namespace PainlessHttp.Cache
{
	public class InMemoryCache : IModifiedSinceCache
	{
		private readonly Dictionary<string, CachedObject> _cache;

		public InMemoryCache()
		{
			_cache = new Dictionary<string, CachedObject>();
		}
		public CachedObject Get(HttpWebRequest req)
		{
			if (req == null)
			{
				return null;
			}
			if (!string.Equals(req.Method, HttpMethods.Get, StringComparison.InvariantCultureIgnoreCase))
			{
				return null;
			}
			var key = GetCacheKey(req);
			if (!_cache.ContainsKey(key))
			{
				return null;
			}

			return _cache[key];
		}

		public async Task AddAsync(HttpWebRequest rawRequest, IHttpWebResponse rawResponse)
		{
			if (rawResponse == null)
			{
				return;
			}
			if (rawResponse.StatusCode != HttpStatusCode.OK)
			{
				return;
			}
			var key = GetCacheKey(rawResponse);

			var content = string.Empty;
			using (var responseStream = rawResponse.GetResponseStream())
			{
				using (var reader = new StreamReader(responseStream))
				{
					content = await reader.ReadToEndAsync();
				}
			}

			rawResponse.SetResponseStream(new MemoryStream(Encoding.UTF8.GetBytes(content)));
			
			_cache[key] = new CachedObject(() => new MemoryStream(Encoding.UTF8.GetBytes(content)))
						{
							ModifiedDate = rawResponse.LastModified
						};
		}

		private static string GetCacheKey(IHttpWebResponse rawResponse)
		{
			return string.Format("{0}_{1}_{2}", rawResponse.Method, rawResponse.ResponseUri.AbsolutePath, rawResponse.ResponseUri.Query);
		}

		private static string GetCacheKey(WebRequest rawRequest)
		{
			return string.Format("{0}_{1}_{2}", rawRequest.Method, rawRequest.RequestUri.AbsolutePath, rawRequest.RequestUri.Query);
		}
	}
}
