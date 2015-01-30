using System.Collections.Generic;
using System.Linq;
using System.Net;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Utils
{
	public class UnsupportedMediaTypeResender : IRequestResender
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly AcceptHeaderMapper _mapper;

		public UnsupportedMediaTypeResender(List<IContentSerializer> serializers)
		{
			_mapper = new AcceptHeaderMapper();
			_serializers = serializers;
		}

		public bool IsApplicable(IHttpWebResponse response)
		{
			if (response == null)
			{
				return false;
			}

			if (response.StatusCode != HttpStatusCode.UnsupportedMediaType)
			{
				return false;
			}

			var acceptHeader = response.Headers[HttpRequestHeader.Accept];
			if (string.IsNullOrWhiteSpace(acceptHeader))
			{
				return false;
			}

			var headers  = _mapper.Map(acceptHeader);
			var contentType = headers.Select(h => HttpConverter.ContentTypeOrDefault(h.ContentType));

			var serializer = _serializers
				.FirstOrDefault(s =>
					contentType.Any(ct => s.ContentType.Contains(ct))
				);

			if (serializer == null)
			{
				return false;
			}
			return true;
		}
	}

	public interface IRequestResender
	{
		bool IsApplicable(IHttpWebResponse response);
	}
}
