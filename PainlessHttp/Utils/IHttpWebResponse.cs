using System;
using System.IO;
using System.Linq;
using System.Net;

namespace PainlessHttp.Utils
{
	public interface IHttpWebResponse : IDisposable
	{
		string CharacterSet { get; }
		string ContentEncoding { get; }
		long ContentLength { get; }
		string ContentType { get; }
		CookieCollection Cookies { get; set; }
		WebHeaderCollection Headers { get; }
		bool IsMutuallyAuthenticated { get; }
		DateTime LastModified { get; }
		string Method { get; }
		Version ProtocolVersion { get; }
		Uri ResponseUri { get; }
		string Server { get; }
		HttpStatusCode StatusCode { get; }
		string StatusDescription { get; }
		bool SupportsHeaders { get; }
		void Close();
		string GetResponseHeader(string headerName);
		Stream GetResponseStream();
	}

	public class HttpWebResponse : IHttpWebResponse
	{
		private readonly System.Net.HttpWebResponse _raw;

		public string CharacterSet { get; private set; }
		public string ContentEncoding { get; private set; }
		public string Server { get; private set; }
		public string ContentType { get; private set; }
		public string StatusDescription { get; private set; }
		public string Method { get; private set; }
		public bool IsMutuallyAuthenticated { get; private set; }
		public bool SupportsHeaders { get; private set; }
		public long ContentLength { get; private set; }
		public CookieCollection Cookies { get; set; }
		public WebHeaderCollection Headers { get; private set; }
		public DateTime LastModified { get; private set; }
		public Version ProtocolVersion { get; private set; }
		public Uri ResponseUri { get; private set; }
		public HttpStatusCode StatusCode { get; private set; }

		public HttpWebResponse(System.Net.HttpWebResponse raw = null)
		{
			if (raw == null)
			{
				return;
			}
			_raw = raw;
			InitiateProperties();
		}

		private void InitiateProperties()
		{
			var slave = GetType().GetProperties();
			var master = _raw.GetType().GetProperties();
			foreach (var property in slave)
			{
				var corresponding = master.FirstOrDefault(p => p.Name == property.Name && p.PropertyType == property.PropertyType);
				if (corresponding == null)
					continue;

				property.SetValue(this, corresponding.GetValue(_raw));
			}
		}
		
		public void Close()
		{
			if(_raw != null)
				_raw.Close();
		}

		public string GetResponseHeader(string headerName)
		{
			if (_raw == null)
			{
				throw new NotImplementedException("Can not call method without existing underlying HttpWebResponse");
			}
			return _raw.GetResponseHeader(headerName);
		}

		public Stream GetResponseStream()
		{
			if (_raw == null)
			{
				throw new NotImplementedException("Can not call method without existing underlying HttpWebResponse");
			}
			return _raw.GetResponseStream();
		}

		public void Dispose()
		{
			if (_raw != null)
				_raw.Dispose();
		}
	}
}
