using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Utils;

namespace PainlessHttp.Integration
{
	public class ResponseTransformer
	{
		private readonly IEnumerable<IContentSerializer> _serializers;

		public ResponseTransformer(IEnumerable<IContentSerializer> serializers)
		{
			_serializers = serializers;
		}

		public async Task<IHttpResponse<T>> TransformAsync<T>(IHttpWebResponse raw) where T : class
		{
			var rawBody = await ReadBodyAsync(raw);

			var result = new HttpResponse<T>
			{
				StatusCode = raw.StatusCode,
				RawBody = rawBody,
				Body = Deserialize<T>(rawBody, raw)
			};

			return result;
		}

		private T Deserialize<T>(string body, IHttpWebResponse raw) where T : class
		{
			var contentType = ExtractContentTypeFromHeaders(raw.Headers);
			var serializer = _serializers.FirstOrDefault(s => s.ContentType.Contains(contentType));
			if (serializer == null)
			{
				throw new InstanceNotFoundException(string.Format("No registered serializer found for content type '{0}'.", contentType));
			}

			if (typeof(T) == typeof(string))
			{
				return body as T;
			}
			var typedBody = serializer.Deserialize<T>(body);
			return typedBody;
		}

		private ContentType ExtractContentTypeFromHeaders(WebHeaderCollection headers)
		{
			const ContentType fallback = ContentType.ApplicationJson;
			if (headers == null)
			{
				return fallback;
			}

			var contentTypeHeader = headers[HttpResponseHeader.ContentType];
			if (contentTypeHeader == null)
			{
				return fallback;
			}

			var contentType = HttpConverter.ContentType(contentTypeHeader);
			return contentType;
		}

		public static async Task<string> ReadBodyAsync(IHttpWebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			{
				if (responseStream == null)
				{
					throw new ArgumentNullException("response");
				}

				string raw;
				using (var reader = new StreamReader(responseStream))
				{
					raw = await reader.ReadToEndAsync();
				}
				return raw;
			}
		}
	}
}
