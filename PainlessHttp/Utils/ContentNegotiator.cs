using System.Collections.Generic;
using System.Linq;
using System.Net;
using PainlessHttp.Http;
using PainlessHttp.Integration;

namespace PainlessHttp.Utils
{
	internal interface IContentNegotiator
	{
		bool IsApplicable(IHttpWebResponse rawResponse);
		ContentType GetSupportedContentType(IHttpWebResponse rawResponse);
	}

	public class ContentNegotiator : IContentNegotiator
	{
		private readonly IEnumerable<ContentType> _supportedContentTypes;
		private readonly AcceptHeaderMapper _mapper;

		public ContentNegotiator(IEnumerable<ContentType> supportedContentTypes)
		{
			_supportedContentTypes = supportedContentTypes;
			_mapper = new AcceptHeaderMapper();
		}

		public bool IsApplicable(IHttpWebResponse rawResponse)
		{
			if (rawResponse == null)
			{
				return false;
			}

			if (rawResponse.StatusCode != HttpStatusCode.UnsupportedMediaType)
			{
				return false;
			}

			var acceptTypes = GetContentTypeFromAcceptHeader(rawResponse).ToList();
			if (!acceptTypes.Any())
			{
				return false;
			}

			return _supportedContentTypes.Any(supported => acceptTypes.Any(accept => accept == supported));
		}

		public ContentType GetSupportedContentType(IHttpWebResponse rawResponse)
		{
			var accepts = GetContentTypeFromAcceptHeader(rawResponse);
			var found = accepts.First(accept => _supportedContentTypes.Contains(accept));
			return found;
		}

		private IEnumerable<ContentType> GetContentTypeFromAcceptHeader(IHttpWebResponse rawResponse)
		{
			var acceptHeader = rawResponse.Headers["Accept"];
			if (acceptHeader == null)
			{
				yield break;
			}

			var headers = _mapper.Map(acceptHeader);
			foreach (var header in headers)
			{
				yield return HttpConverter.ContentTypeOrDefault(header.ContentType);
			}
		}
	}
}