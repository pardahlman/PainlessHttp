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
		private readonly ContentType _defaultContentType;
		private readonly List<IContentSerializer> _serializers;
		private readonly string _accept = String.Join(";", ContentTypes.TextHtml, ContentTypes.ApplicationXml, ContentTypes.ApplicationJson);

		public WebRequestWrapper(IEnumerable<IContentSerializer> serializers, ContentType defaultContentType)
		{
			_defaultContentType = defaultContentType;
			_serializers = serializers.ToList();
		}

		public FluentWebRequestBuilder WithUrl(string url)
		{
			return new FluentWebRequestBuilder(_serializers, _accept, _defaultContentType, url);
		}
	}

	public class FluentWebRequestBuilder
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly string _accept;
		private readonly ContentType _defaultContentType;
		private readonly string _url;
		private string _method;
		private ContentType _contentType;
		private object _data;
		private bool _negotiate;

		internal FluentWebRequestBuilder(List<IContentSerializer> serializers, string accept, ContentType defaultContentType, string url)
		{
			_serializers = serializers;
			_accept = accept;
			_defaultContentType = defaultContentType;
			_url = url;
		}

		public FluentWebRequestBuilder WithMethod(HttpMethod method)
		{
			_method = HttpConverter.HttpMethod(method);
			return this;
		}

		public FluentWebRequestBuilder WithPayload(object data, ContentType type)
		{
			if (type == ContentType.Negotiated)
			{
				_negotiate = true;
				type = _defaultContentType;
			}
			
			_data = data;
			_contentType = type;

			
			return this;
		}

		public RequestWrapper Prepare()
		{
			Func<ContentType?, Task<HttpWebRequest>> requestCreateFunc = async (ContentType? overrideType) =>
			{
				string type;
				if (overrideType.HasValue)
				{
					type = HttpConverter.ContentType(overrideType.Value);
				}
				else
				{
					type = HttpConverter.ContentType(_contentType);
				}
				
				var serializer = _serializers.FirstOrDefault(s => s.ContentType.Contains(_contentType));
				if (serializer == null)
				{
					throw new Exception(string.Format("Can not find serializer for content type '{0}'.", _contentType));
				}

				var webReq = (HttpWebRequest) WebRequest.Create(_url);
				webReq.AllowAutoRedirect = true;
				webReq.UserAgent = ClientUtils.GetUserAgent();
				webReq.Method = _method;
				webReq.Accept = _accept;

				if (_data != null)
				{
					_data = serializer.Serialize(_data);
					webReq.ContentType = type;
					var payloadAsBytes = Encoding.UTF8.GetBytes(_data.ToString());
					webReq.ContentLength = payloadAsBytes.Length;

					using (var stream = await Task<Stream>.Factory.FromAsync(webReq.BeginGetRequestStream, webReq.EndGetRequestStream, webReq))
					{
						await stream.WriteAsync(payloadAsBytes, 0, payloadAsBytes.Length);
					}
				}
				return webReq;
			};

			return new RequestWrapper(requestCreateFunc, _negotiate);
		}
	}

	public class RequestWrapper
	{
		private readonly Func<ContentType?, Task<HttpWebRequest>> _createRequestFuncAsync;
		private readonly bool _negotiate;

		public RequestWrapper(Func<ContentType?, Task<HttpWebRequest>> requestFuncAsync, bool negotiate)
		{
			_createRequestFuncAsync = requestFuncAsync;
			_negotiate = negotiate;
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var webReq = await _createRequestFuncAsync(null);

			System.Net.HttpWebResponse result;
			
			try
			{
				result = (System.Net.HttpWebResponse) await Task<WebResponse>.Factory.FromAsync(webReq.BeginGetResponse, webReq.EndGetResponse, webReq);
				return new HttpWebResponse(result);
			}
			catch (WebException e)
			{
				result = (System.Net.HttpWebResponse) e.Response;
				
				if (!_negotiate)
				{
					return new HttpWebResponse(result);
				}
			}

			var accept = result.Headers["Accept"];
			webReq = await _createRequestFuncAsync(HttpConverter.ContentType(accept));
			result = (System.Net.HttpWebResponse)await Task<WebResponse>.Factory.FromAsync(webReq.BeginGetResponse, webReq.EndGetResponse, webReq);
			return new HttpWebResponse(result);
		}
	}
}
