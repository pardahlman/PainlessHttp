using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Client;
using PainlessHttp.Http;
using PainlessHttp.Resenders;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Utils;

namespace PainlessHttp.Integration
{
	public class WebRequestBuilder
	{
		private readonly WebRequestTools _tools;

		public WebRequestBuilder(IEnumerable<IContentSerializer> serializers, ContentType defaultContentType, Action<WebRequest> webrequestModifier, IEnumerable<Credential> credentials)
		{
			var worker = new WebRequestWorker(webrequestModifier, ClientUtils.CreateCredentials(credentials));
			
			_tools = new WebRequestTools
			{
				RequestWorker = worker,
				Serializers = serializers.ToList(),
				RequestModifier = webrequestModifier,
				Resenders = new List<IRequestResender> { new UnsupportedMediaTypeResender(serializers.ToList(), worker) },
				DefaultContentType = defaultContentType == ContentType.Unknown ? ContentType.ApplicationJson : defaultContentType,
			};
		}

		public FluentWebRequestBuilder WithUrl(string url)
		{
			return new FluentWebRequestBuilder(_tools, url);
		}
	}

	public class FluentWebRequestBuilder
	{
		private readonly WebRequestTools _tools;
		private readonly string _accept = String.Join(",", ContentTypes.ApplicationJson, ContentTypes.ApplicationXml);
		private readonly WebRequestSpecifications _requestSpecs;

		internal FluentWebRequestBuilder(WebRequestTools tools, string url)
		{
			_tools = tools;
			_requestSpecs = new WebRequestSpecifications
			{
				Url = url,
				AcceptHeader = _accept
			};
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
			_requestSpecs.ContentType = type;
			if (type == ContentType.Negotiated)
			{
				_requestSpecs .ContentNegotiation = true;
				_requestSpecs.ContentType = _tools.DefaultContentType;
			}

			_requestSpecs.Data = data;
			var serializer = _tools.Serializers.FirstOrDefault(s => s.ContentType.Contains(_requestSpecs.ContentType));
			if (serializer == null)
			{
				throw new Exception(string.Format("No Serializer registered for {0}", type));
			}
			_requestSpecs.SerializeData = () => serializer.Serialize(data);

			return this;
		}

		public RequestWrapper Prepare()
		{
			return new RequestWrapper(_requestSpecs, _tools);
		}
	}

	public class RequestWrapper
	{
		private readonly WebRequestSpecifications _requestSpecs;
		private readonly WebRequestTools _tools;

		internal RequestWrapper(WebRequestSpecifications requestSpecs, WebRequestTools tools)
		{
			_requestSpecs = requestSpecs;
			_tools = tools;
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var response = await _tools.RequestWorker.GetResponseAsync(_requestSpecs);

			var resender = _tools.Resenders.FirstOrDefault(r => r.IsApplicable(response));
			if (resender == null)
			{
				return response;
			}

			var resendReponse = await resender.ResendRequestAsync(response, _requestSpecs);
			return resendReponse;
		}
	}
}
