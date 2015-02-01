using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PainlessHttp.Http;
using PainlessHttp.Integration;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Utils;

namespace PainlessHttp.Resenders
{
	public class UnsupportedMediaTypeResender : IRequestResender
	{
		private readonly List<IContentSerializer> _serializers;
		private readonly IWebRequestWorker _worker;
		private readonly AcceptHeaderMapper _mapper;

		public UnsupportedMediaTypeResender(List<IContentSerializer> serializers, IWebRequestWorker worker)
		{
			_mapper = new AcceptHeaderMapper();
			_serializers = serializers;
			_worker = worker;
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

			var acceptTypes = GetContentTypeFromAcceptHeader(response).ToList();
			if (!acceptTypes.Any())
			{
				return false;
			}

			var serializer = GetApplicableSerializer(acceptTypes);

			if (serializer == null)
			{
				return false;
			}
			return true;
		}

		public async Task<IHttpWebResponse> ResendRequestAsync(IHttpWebResponse response, WebRequestSpecifications specs)
		{
			if (!specs.ContentNegotiation)
			{
				return response;
			}

			ContentType supportedType;
			var acceptTypes = GetContentTypeFromAcceptHeader(response).ToList();
			var serializer = GetApplicableSerializer(acceptTypes, out supportedType);
			if (serializer == null)
			{
				throw new ArgumentException("Can not find serializer for the type(s) " + acceptTypes.Select(t => string.Format("{0} ", t.ToString())));
			}
			specs.ContentType = supportedType;
			specs.SerializeData = () => serializer.Serialize(specs.Data);
			var newResponse = await _worker.GetResponseAsync(specs);
			return newResponse;
		}

		private IContentSerializer GetApplicableSerializer(IEnumerable<ContentType> contentTypes)
		{
			ContentType type;
			return GetApplicableSerializer(contentTypes, out type);
		}

		private IContentSerializer GetApplicableSerializer(IEnumerable<ContentType> contentTypes, out ContentType found)
		{
			var supportedType = contentTypes.FirstOrDefault(type => _serializers.Any(ser => ser.ContentType.Contains(type)));
			found = supportedType;
			if (supportedType == ContentType.Unknown)
			{
				return null;
			}

			var serializer = _serializers.First(s => contentTypes.Any(ct => ct == supportedType));
			return serializer;
		}

		private IEnumerable<ContentType> GetContentTypeFromAcceptHeader(IHttpWebResponse response)
		{
			var acceptHeader = response.Headers["Accept"];
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
