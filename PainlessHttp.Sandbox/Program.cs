using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;

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
			var raw = client.Get("api/todos");
		}
	}
}
