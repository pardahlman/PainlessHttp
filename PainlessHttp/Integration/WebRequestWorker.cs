using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PainlessHttp.Utils;

namespace PainlessHttp.Integration
{
	public interface IWebRequestWorker
	{
		Task<IHttpWebResponse> GetResponseAsync(WebRequestSpecifications spec);
		Task<HttpWebRequest> PrepareAsync(WebRequestSpecifications spec);
		Task<HttpWebResponse> ReceiveAsync(HttpWebRequest req);
	}

	public class WebRequestWorker : IWebRequestWorker
	{
		private readonly Action<WebRequest> _webrequestModifier;
		private readonly NetworkCredential _credentials;

		public WebRequestWorker(Action<WebRequest> webrequestModifier, NetworkCredential credentials)
		{
			_webrequestModifier = webrequestModifier;
			_credentials = credentials;
		}

		public async Task<IHttpWebResponse> GetResponseAsync(WebRequestSpecifications spec)
		{
			var request = await PrepareAsync(spec);
			return await ReceiveAsync(request);
		}

		public async Task<HttpWebRequest> PrepareAsync(WebRequestSpecifications spec)
		{
			var req = (HttpWebRequest)WebRequest.Create(spec.Url);
			
			// Overrideable props up here
			req.AllowAutoRedirect = true;
			req.PreAuthenticate = true;
			req.Accept = spec.AcceptHeader;
			
			if (_webrequestModifier != null)
			{
				_webrequestModifier(req);
			}
			
			// Fixed props down here.
			req.Credentials = _credentials;
			req.UserAgent = ClientUtils.GetUserAgent();
			req.Method = spec.Method.ToString().ToUpper();
			

			if (spec.SerializeData != null)
			{
				var serializedPayload = spec.SerializeData();
				var payloadAsBytes = Encoding.UTF8.GetBytes(serializedPayload);
				req.ContentType = HttpConverter.ContentType(spec.ContentType);
				req.ContentLength = payloadAsBytes.Length;

				using (var stream = await Task<Stream>.Factory.FromAsync(req.BeginGetRequestStream, req.EndGetRequestStream, req))
				{
					await stream.WriteAsync(payloadAsBytes, 0, payloadAsBytes.Length);
				}
			}

			return req;
		}

		public async Task<HttpWebResponse> ReceiveAsync(HttpWebRequest req)
		{
			try
			{
				var response  = await Task<WebResponse>.Factory.FromAsync(req.BeginGetResponse, req.EndGetResponse, req);
				return new HttpWebResponse((System.Net.HttpWebResponse)response);
			}
			catch (WebException e)
			{
				return new HttpWebResponse((System.Net.HttpWebResponse)e.Response);
			}
		}
	}
}
