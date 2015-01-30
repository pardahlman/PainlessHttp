using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Utils
{
	public class WebRequestWrapper
	{
		private readonly ContentType _defaultContentType;
		private readonly List<IContentSerializer> _serializers;

		public WebRequestWrapper(IEnumerable<IContentSerializer> serializers, ContentType defaultContentType)
		{
			_defaultContentType = defaultContentType;
			_serializers = serializers.ToList();
		}

		public FluentWebRequestBuilder WithUrl(string url)
		{
			return new FluentWebRequestBuilder(_serializers, _defaultContentType, url);
		}
	}

	public class FluentWebRequestBuilder
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly string _accept = String.Join(";", ContentTypes.TextHtml, ContentTypes.ApplicationXml, ContentTypes.ApplicationJson);
		private readonly ContentType _defaultContentType;
		private bool _negotiate;
		private readonly WebRequestSpecifications _requestSpecs;

		internal FluentWebRequestBuilder(List<IContentSerializer> serializers, ContentType defaultContentType, string url)
		{
			_requestSpecs = new WebRequestSpecifications
			{
				Url = url,
				AcceptHeader = _accept
			};
			_serializers = serializers;
			_defaultContentType = defaultContentType;
		}

		public FluentWebRequestBuilder WithMethod(HttpMethod method)
		{
			_requestSpecs.Method = method;
			return this;
		}

		public FluentWebRequestBuilder WithPayload(object data, ContentType type)
		{
			if (data == null)
			{
				return this;
			}

			if (type == ContentType.Negotiated)
			{
				_negotiate = true;
				type = _defaultContentType;
				_requestSpecs.ContentType = _defaultContentType;
			}

			_requestSpecs.ContentType = type;
			var serializer = _serializers.FirstOrDefault(s => s.ContentType.Contains(type));
			if (serializer == null)
			{
				throw new Exception(string.Format("No Serializer registered for {0}", type));
			}
			_requestSpecs.SerializeData = () => serializer.Serialize(data);

			return this;
		}

		public RequestWrapper Prepare()
		{
			return new RequestWrapper(_requestSpecs, _negotiate);
		}
	}

	public class RequestWrapper
	{
		private readonly WebRequestSpecifications _requestSpecs;
		private readonly bool _negotiate;

		public RequestWrapper(WebRequestSpecifications requestSpecs, bool negotiate)
		{
			_requestSpecs = requestSpecs;
			_negotiate = negotiate;
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var response = await WebRequestWorker.GetResponse(_requestSpecs);
			
			if (_negotiate && response.StatusCode == HttpStatusCode.UnsupportedMediaType)
			{
				var accept = response.Headers["Accept"];
				_requestSpecs.ContentType = HttpConverter.ContentType(accept);
				var negotiatedresponse = await WebRequestWorker.GetResponse(_requestSpecs);
				return negotiatedresponse;
			}

			return response;
		}
	}
}
