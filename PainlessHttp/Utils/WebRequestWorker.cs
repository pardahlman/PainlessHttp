using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PainlessHttp.Http;

namespace PainlessHttp.Utils
{
	public class WebRequestWorker
	{
		public static async Task<IHttpWebResponse> GetResponse(WebRequestSpecifications spec)
		{
			var request = await PrepareAsync(spec);
			return await ReceiveAsync(request);
		}

		public static async Task<HttpWebRequest> PrepareAsync(WebRequestSpecifications spec)
		{
			var req = (HttpWebRequest)WebRequest.Create(spec.Url);
			req.AllowAutoRedirect = true;
			req.UserAgent = ClientUtils.GetUserAgent();
			req.Method = spec.Method.ToString().ToUpper();
			req.Accept = spec.AcceptHeader;

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

		public async static Task<HttpWebResponse> ReceiveAsync(HttpWebRequest req)
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
