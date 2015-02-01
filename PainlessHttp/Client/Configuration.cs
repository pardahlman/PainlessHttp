using System;
using System.Collections.Generic;
using System.Net;
using PainlessHttp.Http;
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
			public IEnumerable<IContentSerializer> Serializers { get; set; }

			/// <summary>
			/// Specify the default content type that the client will use if no
			/// content negotiation has been made.
			/// </summary>
			public ContentType ContentType { get; set; }

			public Action<WebRequest> WebrequestModifier { get; set; }
			public NetworkCredential Credentials { get; set; }

			public AdvancedConfiguration()
			{
				Serializers = new List<IContentSerializer>();
			}
		}
	}
}
