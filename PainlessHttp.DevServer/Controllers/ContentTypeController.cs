using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Typed;

namespace PainlessHttp.DevServer.Controllers
{
	public class ContentTypeController : ApiController
	{
		private readonly DefaultXmlSerializer _xmlSerializer;
		private readonly DefaultJsonSerializer _jsonSerializer;

		public ContentTypeController()
		{
			_xmlSerializer = new DefaultXmlSerializer();
			_jsonSerializer = new DefaultJsonSerializer();
		}

		[Route("api/content-type")]
		[HttpGet]
		public HttpResponseMessage GetData(string prefered = "")
		{
			var todo = new Todo
			{
				Description = "Respond with a specific content type",
				IsCompleted = true
			};

			
			var content = string.Empty;
			var contentType = string.Empty;
			
			if (string.Equals(prefered, "json", StringComparison.InvariantCultureIgnoreCase))
			{
				content = _jsonSerializer.Serialize(todo);
				contentType = ContentTypes.ApplicationJson;
			} 
			if (string.Equals(prefered, "xml", StringComparison.InvariantCultureIgnoreCase))
			{
				content = _xmlSerializer.Serialize(todo);
				contentType = ContentTypes.ApplicationXml;
			}


			if (string.IsNullOrWhiteSpace(content))
			{
				return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Expect a 'prefered' query parameter with value 'json' or 'xml' in request.");
			}

			var response = new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(content, Encoding.UTF8, contentType)
			};
			return response;
		}

		[Route("api/content-type")]
		[HttpPost]
		public HttpResponseMessage ReciveData(string accept = "")
		{
			var contentType = Request.Content.Headers.ContentType.ToString();
			var acceptHeader = ContentTypes.ApplicationJson;
			var matching = false;
			
			if (string.Equals(accept, "json", StringComparison.InvariantCultureIgnoreCase))
			{
				matching = string.Equals(contentType, ContentTypes.ApplicationJson);
				acceptHeader = ContentTypes.ApplicationJson;
			}
			if (string.Equals(accept, "xml", StringComparison.InvariantCultureIgnoreCase))
			{
				matching = string.Equals(contentType, ContentTypes.ApplicationXml);
				acceptHeader = ContentTypes.ApplicationXml;
			}

			if (matching)
			{
				return Request.CreateResponse(HttpStatusCode.OK, "Accepted");
			}

			var response = new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
			response.Headers.AcceptRanges.Clear();
			response.Headers.Add("Accept", acceptHeader);
			return response;
		}
	}
}
