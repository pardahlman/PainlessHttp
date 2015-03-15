using System;
using System.IO;

namespace PainlessHttp.Cache
{
	public class CachedObject
	{
		private readonly Lazy<Stream> _resposeStream;
		public DateTime ModifiedDate { get; set; }
		public Stream ResponseStream { get { return _resposeStream.Value;  }}

		public CachedObject(Func<Stream> resposeStream = null)
		{
			_resposeStream = new Lazy<Stream>(resposeStream);
		}
	}
}