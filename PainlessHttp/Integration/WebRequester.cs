using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PainlessHttp.Client;
using PainlessHttp.Http;
using PainlessHttp.Http.Contracts;
using PainlessHttp.Utils;
using HttpWebResponse = PainlessHttp.Http.HttpWebResponse;
using Timer = System.Timers.Timer;

namespace PainlessHttp.Integration
{
	public class WebRequester
	{
		private readonly Configuration _config;
		private readonly UrlBuilder _urlBuilder;
		private readonly string _accept = String.Join(",", ContentTypes.ApplicationJson, ContentTypes.ApplicationXml);

		private Func<HttpWebRequest> _requestInit;
		private Action<HttpWebRequest> _methodModifier;
		private Func<HttpWebRequest, Task> _payloadModifilerAsync;
		private Func<ContentType> _contentTypeProvider;
		private readonly IContentNegotiator _contentNegotiator;
		private const string _timeOutResponse = "Oh noes! The request for {0} timed out after {1} ms. The time out can be increased by configuring the client's Request time-out at Configuration.Advanced.RequestTimeout.";

		public WebRequester(Configuration config)
		{
			_config = config;
			_contentNegotiator = new ContentNegotiator(config.Advanced.Serializers.SelectMany(s => s.ContentType));
			_urlBuilder = new UrlBuilder(config.BaseUrl);
			ResetModifiers();
		}

		public WebRequester WithUrl(string url, object query)
		{
			var requestUrl = _urlBuilder.Build(url, query);
			_requestInit = () => (HttpWebRequest) WebRequest.Create(requestUrl);
			return this;
		}

		public WebRequester WithMethod(HttpMethod method)
		{
			_methodModifier = req => req.Method = method.ToString().ToUpper();
			return this;
		}

		public WebRequester WithPayload(object data, ContentType type)
		{
			_contentTypeProvider = () => type;
			_payloadModifilerAsync = async req =>
			{
				if (data == null)
				{
					return;
				}
				var serializer = _config.Advanced.Serializers.FirstOrDefault(s => s.ContentType.Contains(_contentTypeProvider()));
				if (serializer == null)
				{
					throw new Exception(string.Format("No Serializer registered for {0}", type));
				}
				var serializedPayload = serializer.Serialize(data);
				var payloadAsBytes = Encoding.UTF8.GetBytes(serializedPayload);
				req.ContentLength = payloadAsBytes.Length;
				req.ContentType = HttpConverter.ContentType(_contentTypeProvider());
				using (var stream = await Task<Stream>.Factory.FromAsync(req.BeginGetRequestStream, req.EndGetRequestStream, req))
				{
					await stream.WriteAsync(payloadAsBytes, 0, payloadAsBytes.Length);
				}
			};
			return this;
		}

		public async Task<IHttpWebResponse> PerformAsync()
		{
			var rawRequest = await PrepareAsync();
			var rawResponse = await GetResponseAsync(rawRequest);
			
			if (_config .Advanced.ContentNegotiation && _contentNegotiator.IsApplicable(rawResponse))
			{
				_contentTypeProvider = () => _contentNegotiator.GetSupportedContentType(rawResponse);
				rawRequest = await PrepareAsync();
				rawResponse = await GetResponseAsync(rawRequest);
			}

			if (rawResponse.StatusCode == HttpStatusCode.NotModified)
			{
				var cached = _config.Advanced.ModifiedSinceCache.Get(rawRequest);
				rawResponse.SetResponseStream(cached.ResponseStream);
			}
			else
			{
				await _config.Advanced.ModifiedSinceCache.AddAsync(rawRequest, rawResponse);
			}

			ResetModifiers();
			return rawResponse;
		}
		
		private async Task<IHttpWebResponse> GetResponseAsync(WebRequest rawRequest)
		{
			var timer = new Timer(_config.Advanced.RequestTimeout.TotalMilliseconds);
			timer.Elapsed += (sender, args) => rawRequest.Abort();
			try
			{
				timer.Start();
				var response =
					await Task<WebResponse>.Factory.FromAsync(rawRequest.BeginGetResponse, rawRequest.EndGetResponse, rawRequest);
				return new HttpWebResponse((System.Net.HttpWebResponse) response);
			}
			catch (WebException e)
			{
				if (e.Status == WebExceptionStatus.RequestCanceled)
				{
					var timeoutResponse = new HttpWebResponse
					{
						ContentType = ContentTypes.TextPlain
					};
					timeoutResponse.SetResponseStream(new MemoryStream(Encoding.UTF8.GetBytes(string.Format(_timeOutResponse, rawRequest.RequestUri.AbsolutePath, _config.Advanced.RequestTimeout.TotalMilliseconds))));
					return timeoutResponse;
				}
				return new HttpWebResponse((System.Net.HttpWebResponse) e.Response);
			}
			finally
			{
				timer.Close();
				timer.Stop();
				timer.Dispose();
			}
		}

		private async Task<HttpWebRequest> PrepareAsync()
		{
			var req = _requestInit();
			
			// Overrideable props up here
			req.AllowAutoRedirect = true;
			req.PreAuthenticate = true;
			req.Accept = _accept;

			// Modifications from configuration
			_config.Advanced.WebrequestModifier(req);

			// Fixed props down here.
			req.Credentials = _config.Advanced.Credentials;
			req.UserAgent = ClientUtils.GetUserAgent();
			_methodModifier(req);
			await _payloadModifilerAsync(req);

			var cached = _config.Advanced.ModifiedSinceCache.Get(req);
			if (cached != null)
			{
				req.IfModifiedSince = cached.ModifiedDate;
			}

			return req;
		}

		private void ResetModifiers()
		{
			_requestInit = () => { throw new Exception("Request can not be performed without a Url. Set Url by calling the 'WithUrl' method."); };
			_contentTypeProvider = () => ContentType.Unknown;
			_payloadModifilerAsync = request => Task.FromResult(true);
			_methodModifier = request => { };
		}
	}
}