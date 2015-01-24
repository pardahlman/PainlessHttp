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

namespace PainlessHttp.Utils
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
			var payloadTask = DeserializeAsync<T>(raw);
			return await payloadTask.ContinueWith(task =>
			{
				IHttpResponse<T> result = new HttpResponse<T>
				{
					StatusCode = raw.StatusCode,
					Body = task.Result
				};
				return result;
			});
		}

		private async Task<T> DeserializeAsync<T>(IHttpWebResponse raw) where T : class
		{

			var contentType = ExtractContentTypeFromHeaders(raw.Headers);
			var serializer = _serializers.FirstOrDefault(s => s.ContentType.Contains(contentType));
			if (serializer == null)
			{
				throw new InstanceNotFoundException(string.Format("No registered serializer found for content type '{0}'.", contentType));
			}

			var readTask = ReadBodyAsync(raw);
			var deserializeTask = await readTask.ContinueWith(task =>
			{
				if (typeof(T) == typeof(string))
				{
					return task.Result as T;
				}
				var body = task.Result;
				var typedBody = serializer.Deserialize<T>(body);
				return typedBody;
			});
			return deserializeTask;
		}

		private ContentType ExtractContentTypeFromHeaders(WebHeaderCollection headers)
		{
			var defaultContentType = ContentType.ApplicationJson;
			if (headers == null)
			{
				return defaultContentType;
			}

			var contentTypeHeader = headers[HttpResponseHeader.ContentType];
			if (contentTypeHeader == null)
			{
				return defaultContentType;
			}

			var contentType = HttpConverter.ContentType(contentTypeHeader);
			return contentType;
		}

		public static async Task<string> ReadBodyAsync(IHttpWebResponse response)
		{
			var responseStream = response.GetResponseStream();
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
