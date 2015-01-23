using System.Collections.Generic;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Serializers;

namespace PainlessHttp.Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = new HttpClientConfiguration
			{
				BaseUrl = "http://localhost:1337/"
			};

			var client = new HttpClient(config);

			var tomorrow = new Todo { Description = "Sleep in" };
			var serializer = new DefaultJsonSerializer();
			var tomorrowJson = serializer.Serialize(tomorrow);
			
			var created = client.PostAsync<Todo>("api/todos", tomorrowJson).Result;
		}
	}
}
