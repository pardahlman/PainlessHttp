using System.Collections.Generic;
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

			public AdvancedConfiguration()
			{
				Serializers = new List<IContentSerializer>();
			}
		}
	}

	
}
