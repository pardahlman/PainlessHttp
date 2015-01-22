﻿using System;
using System.Net;
using PainlessHttp.Http;

namespace PainlessHttp.Utils
{
	public class WebRequestWrapper
	{
		//private readonly string _accept = String.Join(";", ContentTypes.TextHtml, ContentTypes.ApplicationXml, ContentTypes.ApplicationJson);
		private readonly string _accept = ContentTypes.ApplicationJson;
		private readonly string _url;
		private string _method;
		private string _contentType;

		private WebRequestWrapper(string url)
		{
			_url = url;
		}

		public static WebRequestWrapper WithUrl(string url)
		{
			return new WebRequestWrapper(url);
		}

		public WebRequestWrapper WithMethod(HttpMethod method)
		{
			_method = HttpConverter.HttpMethod(method);
			return this;
		}

		public WebRequestWrapper WithPayload(object data, ContentType type)
		{
			_contentType = HttpConverter.ContentType(type);
			return this;
		}

		public RequestWrapper Prepare()
		{
			Func<HttpWebRequest> createFunc = () =>
			{
				var httpWebRequest = (HttpWebRequest) WebRequest.Create(_url);
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.UserAgent = ClientUtils.GetUserAgent();
				httpWebRequest.Method = _method;
				httpWebRequest.Accept = _accept;
				httpWebRequest.ContentType = _contentType;

				return httpWebRequest;
			};

			return new RequestWrapper(createFunc);
		}
	}

	public class RequestWrapper
	{
		private readonly Func<HttpWebRequest> _createFunc;

		public RequestWrapper(Func<HttpWebRequest> createFunc)
		{
			_createFunc = createFunc;
		}

		public HttpWebResponse Perform()
		{
			return (HttpWebResponse)_createFunc().GetResponse();
		}
	}
}