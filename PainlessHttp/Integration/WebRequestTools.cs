using System;
using System.Collections.Generic;
using System.Net;
using PainlessHttp.Http;
using PainlessHttp.Resenders;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Utils;

namespace PainlessHttp.Integration
{
	internal class WebRequestTools
	{
		public ContentType DefaultContentType { get; set; }
		public List<IContentSerializer> Serializers { get; set; }
		public Action<WebRequest> RequestModifier { get; set; }
		public WebRequestWorker RequestWorker { get; set; }
		public List<IRequestResender> Resenders { get; set; }
	}
}