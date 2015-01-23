using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PainlessHttp.Client;
using PainlessHttp.Client.Configuration;
using PainlessHttp.DevServer.Model;
using PainlessHttp.Http;
using PainlessHttp.Serializer.JsonNet;
using PainlessHttp.Serializers.Contracts;
using PainlessHttp.Serializers.Custom;

namespace PainlessHttp.Sandbox
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = new HttpClientConfiguration
			{
				BaseUrl = "http://localhost:1337/",
				Advanced =
				{
					Serializers = new List<IContentSerializer>
					{
						new Serializer<NewtonSoft>(ContentType.ApplicationJson)
					}
				}
			};

			var tomorrow = new Todo { Description = "Sleep in" };

			// Create a custom serializer
			var custom = SerializeSettings
				.For(ContentType.ApplicationJson)
					.Serialize(NewtonSoft.Serialize)
					.Deserialize(DefaultJson.Deserialize);
			
			var customJson = custom.Serialize(tomorrow);
			var customObj = custom.Deserialize<Todo>(customJson);

			// Configure JsonNet however you want
			var newtonSoftSettings = new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};

			//Register the change
			NewtonSoft.UpdateSettings(new NewtonsoftSettings {Settings = newtonSoftSettings});

			// Go crazy, oh baby!
			var serializer = new Serializer<NewtonSoft>(ContentType.ApplicationJson);
			var tomorrowJson = serializer.Serialize(tomorrow);
			var tommorowObj = serializer.Deserialize<Todo>(tomorrowJson);

			var client = new HttpClient(config);
			var created = client.PostAsync<Todo>("api/todos", tomorrow).Result;
		}
	}

	
}
