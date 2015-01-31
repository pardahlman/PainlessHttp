using System;
using System.Collections.Generic;
using System.Net;
using PainlessHttp.Http;
using PainlessHttp.Serializers.Contracts;

namespace PainlessHttp.Client.Configuration
{
	public class HttpClientConfiguration
	{
		public string BaseUrl { get; set; }
		public AdvancedConfiguration Advanced { get; set; }

		public HttpClientConfiguration()
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
			public List<Credential> Credentials { get; set; }

			public AdvancedConfiguration()
			{
				Serializers = new List<IContentSerializer>();
				Credentials = new List<Credential>();
			}
		}
	}

	public class Credential
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Domain { get; set; }
		public List<AuthenticationType> AuthTypes { get; set; }

		public Credential()
		{
			AuthTypes = new List<AuthenticationType>();
		}
	}
}
