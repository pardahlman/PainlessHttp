using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Utils
{
	public class WebRequestBuilder
	{
		private readonly ContentType _defaultContentType;
		private readonly List<IContentSerializer> _serializers;

		public WebRequestBuilder(IEnumerable<IContentSerializer> serializers, ContentType defaultContentType)
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
		private readonly WebRequestSpecifications _requestSpecs;
		private readonly List<IRequestResender> _resenders;

		internal FluentWebRequestBuilder(List<IContentSerializer> serializers, ContentType defaultContentType, string url)
		{
			_requestSpecs = new WebRequestSpecifications
			{
				Url = url,
				AcceptHeader = _accept
			};
			_serializers = serializers;
			_resenders = new List<IRequestResender> {new UnsupportedMediaTypeResender(serializers, new WebRequestWorker())};
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
				_requestSpecs .ContentNegotiation = true;
				type = _defaultContentType;
				_requestSpecs.ContentType = _defaultContentType;
			}

			_requestSpecs.ContentType = type;
			_requestSpecs.Data = data;
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
			return new RequestWrapper(_requestSpecs, _resenders);
		}
	}

	public class RequestWrapper
	{
		private readonly WebRequestSpecifications _requestSpecs;
		private readonly List<IRequestResender> _resenders;
		private readonly IWebRequestWorker _worker;

		public RequestWrapper(WebRequestSpecifications requestSpecs, List<IRequestResender> resenders)
		{
			_requestSpecs = requestSpecs;
			_resenders = resenders;
			_worker = new WebRequestWorker();
			
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var response = await _worker.GetResponseAsync(_requestSpecs);

			var resender = _resenders.FirstOrDefault(r => r.IsApplicable(response));
			if (resender == null)
			{
				return response;
			}

			var resendReponse = await resender.ResendRequestAsync(response, _requestSpecs);
			return resendReponse;
		}
	}
}
