using System.Collections.Generic;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Model;

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

			var raw = client.GetAsync<Todo>("api/todos/2").Result;

			var tomorrow = new Todo { Description = "Sleep in" };
			var tommorowJson = Newtonsoft.Json.JsonConvert.SerializeObject(tomorrow);

			var created = client.PostAsync<Todo>("api/todos", tommorowJson).Result;
		}
	}
}
