using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Utils
{
	public class WebRequestWrapper
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly string _accept = String.Join(";", ContentTypes.TextHtml, ContentTypes.ApplicationXml, ContentTypes.ApplicationJson);

		public WebRequestWrapper(IEnumerable<IContentSerializer> serializers)
		{
			_serializers = serializers.ToList();
		}

		public FluentWebRequestBuilder WithUrl(string url)
		{
			return new FluentWebRequestBuilder(_serializers, _accept, url);
		}
	}

	public class FluentWebRequestBuilder
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly string _accept;
		private readonly string _url;
		private string _method;
		private string _contentType;
		private object _data;

		internal FluentWebRequestBuilder(List<IContentSerializer> serializers, string accept, string url)
		{
			_serializers = serializers;
			_accept = accept;
			_url = url;
		}

		public FluentWebRequestBuilder WithMethod(HttpMethod method)
		{
			_method = HttpConverter.HttpMethod(method);
			return this;
		}

		public FluentWebRequestBuilder WithPayload(object data, ContentType type)
		{
			if (data == null)
				return this;

			var serializer = _serializers.FirstOrDefault(s => s.ContentType.Contains(type));
			if (serializer == null)
			{
				throw new Exception(string.Format("Can not find serializer for content type '{0}'.", type));
			}

			_data = serializer.Serialize(data);
			_contentType = HttpConverter.ContentType(type);
			return this;
		}

		public RequestWrapper Prepare()
		{
			Func<Task<HttpWebRequest>> requestCreateFunc = async () =>
			{
				var webReq = (HttpWebRequest) WebRequest.Create(_url);
				webReq.AllowAutoRedirect = true;
				webReq.UserAgent = ClientUtils.GetUserAgent();
				webReq.Method = _method;
				webReq.Accept = _accept;

				if (_data != null)
				{
					webReq.ContentType = _contentType;
					var payloadAsBytes = Encoding.UTF8.GetBytes(_data.ToString());
					webReq.ContentLength = payloadAsBytes.Length;

					using (var stream = await Task<Stream>.Factory.FromAsync(webReq.BeginGetRequestStream, webReq.EndGetRequestStream, webReq))
					{
						await stream.WriteAsync(payloadAsBytes, 0, payloadAsBytes.Length);
					}
				}
				return webReq;
			};

			return new RequestWrapper(requestCreateFunc);
		}
	}

	public class RequestWrapper
	{
		private readonly Func<Task<HttpWebRequest>> _createRequestFuncAsync;

		public RequestWrapper(Func<Task<HttpWebRequest>> requestFuncAsync)
		{
			_createRequestFuncAsync = requestFuncAsync;
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var webReq = await _createRequestFuncAsync();

			var webResp = await Task<WebResponse>.Factory.FromAsync(webReq.BeginGetResponse, webReq.EndGetResponse, webReq);
			return new HttpWebResponse((System.Net.HttpWebResponse)webResp);
		}
	}
}
