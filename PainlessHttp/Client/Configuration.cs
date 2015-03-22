using System;
using System.Collections.Generic;
using System.Net;
using PainlessHttp.Cache;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Client
{
	public class Configuration
	{
		public string BaseUrl { get; set; }
		public AdvancedConfiguration Advanced { get; set; }

		public Configuration()
		{
			Advanced = new AdvancedConfiguration();
		}

		public class AdvancedConfiguration
		{
			/// <summary>
			/// Override the default content serializers.
			/// </summary>
			public IList<IContentSerializer> Serializers { get; set; }

			/// <summary>
			/// Specify if Content Negotiation should be performed. If set to true, a request with a body will be resent
			/// if the server does not support the current content type.
			/// </summary>
			public bool ContentNegotiation { get; set; }

			/// <summary>
			/// The Cache to be used for 'If-Modified-Since' and 'Last-Modified' header.
			/// Default to a 'NoCache' Implementation, that does not perform any caching.
			/// </summary>
			public IModifiedSinceCache ModifiedSinceCache { get; set; }

			/// <summary>
			/// The time to wait for a response before timing out. Defaults to 10 seconds.
			/// </summary>
			public TimeSpan RequestTimeout { get; set; }

			/// <summary>
			/// Action to be made on the underlying WebRequest before it is sent.
			/// E.g. request => request.Headers.Add("X-Additional-Header", "Additional Value")
			/// </summary>
			public Action<WebRequest> WebrequestModifier { get; set; }
			public NetworkCredential Credentials { get; set; }

			public AdvancedConfiguration()
			{
				Serializers = new List<IContentSerializer>();
				ModifiedSinceCache = new NoCache();
				ContentNegotiation = true;
				WebrequestModifier = (req) => { };
				RequestTimeout = new TimeSpan(days: 0, hours:0, minutes:0, seconds:10);
			}
		}
	}
}
