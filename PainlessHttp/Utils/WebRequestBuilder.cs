using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Client.Configuration;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Utils
{
	public class WebRequestBuilder
	{
		private readonly WebRequestTools _tools;

		public WebRequestBuilder(IEnumerable<IContentSerializer> serializers, ContentType defaultContentType, Action<WebRequest> webrequestModifier, IEnumerable<Credential> credentials)
		{
			var credentialMapper = new CredentialMapper();
			var worker = new WebRequestWorker(webrequestModifier, credentialMapper.Map(credentials));
			
			_tools = new WebRequestTools
			{
				DefaultContentType = defaultContentType == ContentType.Unknown ? ContentType.ApplicationJson : defaultContentType,
				Serializers = serializers.ToList(),
				RequestModifier = webrequestModifier,
				Resenders = new List<IRequestResender>
				{
					new UnsupportedMediaTypeResender(serializers.ToList(), worker)
				},
				RequestWorker = worker 
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
		private readonly string _accept = String.Join(";", ContentTypes.TextHtml, ContentTypes.ApplicationXml, ContentTypes.ApplicationJson);
		private readonly WebRequestSpecifications _requestSpecs;

		internal FluentWebRequestBuilder(WebRequestTools tools, string url)
		{
			_tools = tools;
			_requestSpecs = new WebRequestSpecifications
			{
				Url = url,
				AcceptHeader = _accept,
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

			if (type == ContentType.Negotiated)
			{
				_requestSpecs .ContentNegotiation = true;
				type = _tools.DefaultContentType;
				_requestSpecs.ContentType = _tools.DefaultContentType;
			}

			_requestSpecs.ContentType = type;
			_requestSpecs.Data = data;
			var serializer = _tools.Serializers.FirstOrDefault(s => s.ContentType.Contains(type));
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

	internal class WebRequestTools
	{
		public ContentType DefaultContentType { get; set; }
		public List<IContentSerializer> Serializers { get; set; }
		public Action<WebRequest> RequestModifier { get; set; }
		public WebRequestWorker RequestWorker { get; set; }
		public List<IRequestResender> Resenders { get; set; }
	}
}
